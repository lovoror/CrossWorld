using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BehaviorTreeUpdate : MonoBehaviour {
	public delegate void EnemyAlertStateEventHandler(Transform enemy, bool isAlert);
	public static event EnemyAlertStateEventHandler EnemyAlertStateEvent;

	public BehaviorTree behaviorTree;
	public GameObject patrolPoints;

	Pathfinding.AIPathAgent pathAgent;
	Manager I_Manager;
	SharedFloat baseSpeed;
	SharedFloat curSpeed;
	BaseData I_BaseData;
	Transform deadEnemys;
	Transform body
	{
		get
		{
			return I_BaseData.curBodyTransform;
		}
	}
	Transform leg;
	GameObject player;
	Animator legAnim;
	Animator bodyAnim;
	//AnimatorStateInfo bodyAnimInfo;
	SpriteRenderer bodyRender;
	bool isAttacking = false;
	bool isDead = false;
	bool curAlertState = false;
	static int sortingOrder = 1;

	float attackSpeedRate
	{
		get
		{
			return I_BaseData.curWeaponSpeed;
		}
	}

	void Awake()
	{
		pathAgent = transform.GetComponent<Pathfinding.AIPathAgent>();
		player = GameObject.FindGameObjectWithTag("Player");
		behaviorTree.SetVariable("Player", (SharedGameObject)player);
		leg = transform.Find("Leg");
		legAnim = leg.GetComponent<Animator>();
		I_Manager = transform.GetComponent<Manager>();
		deadEnemys = GameObject.Find("DeadEnemys").transform;
	}

	void OnEnable()
	{
	}

	void OnDisable()
	{
	}

	void Start ()
	{
		behaviorTree.SetVariable("PatrolPoints", (SharedGameObject)patrolPoints);
		I_BaseData = I_Manager.I_BaseData;
		bodyAnim = body.GetComponent<Animator>();
		bodyRender = body.GetComponent<SpriteRenderer>();
		baseSpeed = (float)behaviorTree.GetVariable("RunSpeed").GetValue();
		// 初始化AlertState
		if (EnemyAlertStateEvent != null) {
			EnemyAlertStateEvent(transform, false);
		}
	}

	float curTime = 0;
	const float deltaTime = 0.3f;
	Vector3? prePos;
	bool firstDead = true;
	void Update()
	{
		isDead = (bool)behaviorTree.GetVariable("IsDead").GetValue();
		if (isDead) {
			if (firstDead) {
				legAnim.SetBool("Dead", true);
				transform.parent = deadEnemys.transform;
				DestroyOneEnemy();
				int deadState = Mathf.RoundToInt(Random.Range(0.49f, 7.49f));
				bodyAnim.SetInteger("DeadState", deadState);
				bodyAnim.SetBool("Dead", true);
				this.enabled = false;
				Collider selfCollider = transform.GetComponent<Collider>();
				Collider playerCollider = player.transform.GetComponent<Collider>();
				Physics.IgnoreCollision(playerCollider, selfCollider);
				bodyRender.sortingLayerName = "Default";
				bodyRender.sortingOrder = sortingOrder++;
				// 逐渐腐烂
				ColorTo(body.gameObject, new Color(0, 1, 1), 30);
				firstDead = false;
				// 重置Alert
				if (EnemyAlertStateEvent != null) {
					curAlertState = false;
					EnemyAlertStateEvent(transform, false);
				}
				// 垃圾回收
				StartCoroutine("DelayClose");
			}
			return;
		}
		// 设置状态机
		isAttacking = (bool)behaviorTree.GetVariable("Attack").GetValue();
		bodyAnim.SetBool("Attack", isAttacking);
		bodyAnim.SetFloat("AttackSpeed", attackSpeedRate);

		// 状态动画速度变更
		curTime += Time.deltaTime;
		if (curTime >= deltaTime) {
			curTime -= deltaTime;
			if (prePos == null) {
				prePos = transform.position;
				curSpeed = pathAgent.Velocity().magnitude;
			}
			else {
				Vector3 V = transform.position - prePos.Value;
				curSpeed = V.magnitude / deltaTime;
				prePos = transform.position;
			}
		
			bodyAnim.SetFloat("Speed", curSpeed.Value / baseSpeed.Value);
			legAnim.SetFloat("Speed", curSpeed.Value / baseSpeed.Value);
		}

		// Alert状态变化时进行通知
		bool alert = (bool)behaviorTree.GetVariable("IsAlert").GetValue();
		if (alert != curAlertState) {
			curAlertState = alert;
			if (EnemyAlertStateEvent != null) {
				EnemyAlertStateEvent(transform, curAlertState);
			}
		}
	}

	// 死亡后垃圾回收
	IEnumerator DelayClose()
	{
		yield return new WaitForSeconds(0.3f);

		// 删除target点
		GameObject target = GameObject.Find(transform.name + " target");
		if (target != null) {
			Destroy(target);
		}
		Destroy(transform.GetComponent<EnemyDataManager>());
		Destroy(transform.GetComponent<EnemyManager>());
		Destroy(transform.GetComponent<EnemyMessenger>());
		Destroy(transform.GetComponent<Pathfinding.AIPathAgent>());
		Destroy(transform.GetComponent<Pathfinding.SimpleSmoothModifier>());
		Destroy(transform.GetComponent<Pathfinding.FunnelModifier>());
		Destroy(transform.GetComponent<Seeker>());
		Destroy(transform.GetComponent<BehaviorTree>());
		Destroy(transform.GetComponent<SphereCollider>());
		Destroy(transform.Find("Weapons").gameObject);
		Destroy(transform.Find("Leg").gameObject);
		Transform bodys = transform.Find("Bodys");
		Destroy(bodys.GetComponent<AnimEventsManager>());
		Transform knife = bodys.Find("Knife");
		Destroy(knife.GetComponent<BodyAnimEvents>());
	}

	void ColorTo(GameObject obj, Color c, float t)
	{
		//键值对儿的形式保存iTween所用到的参数  
		Hashtable args = new Hashtable();

		//颜色值
		args.Add("color", c);
		//动画的时间  
		args.Add("time", t);

		//这里是设置类型，iTween的类型又很多种，在源码中的枚举EaseType中  
		args.Add("easeType", iTween.EaseType.linear);
		//三个循环类型 none loop pingPong (一般 循环 来回)     
		args.Add("loopType", "none");
		// 结束回调函数
		args.Add("oncomplete", "AnimationEnd");
		args.Add("oncompletetarget", gameObject);
		iTween.ColorTo(obj, args);
	}

	int maxEnemyCount = 150;
	void AnimationEnd()
	{
		StaticBatchingUtility.Combine(deadEnemys.gameObject);
		Destroy(transform.GetComponent<BehaviorTreeUpdate>());
	}

	void DestroyOneEnemy()
	{
		// 性能优化
		if (deadEnemys.childCount > maxEnemyCount) {
			Destroy(deadEnemys.GetChild(0).gameObject);
		}
	}
}



/*----------------------- iTween -----------------
        //键值对儿的形式保存iTween所用到的参数  
        Hashtable args = new Hashtable();
 
        //颜色值
        args.Add("color",new Color(0,0,0,0));
        //单一色值
        args.Add("r", 0);
        args.Add("g", 0);
        args.Add("b", 0);
        args.Add("a", 0);
        //是否包括子对象
        args.Add("includechildren",true);
        //当效果是应用在renderer(渲染器)组件上时,此参数确定具体应用到那个以命名颜色值上
        args.Add("namedcolorvalue", iTween.NamedValueColor._Color);
        
        //动画的时间  
        args.Add("time", 10f);
        //延迟执行时间  
        args.Add("delay", 0.1f);
 
        //这里是设置类型，iTween的类型又很多种，在源码中的枚举EaseType中  
        args.Add("easeType", iTween.EaseType.easeInOutExpo);
        //三个循环类型 none loop pingPong (一般 循环 来回)     
        //args.Add("loopType", "none");  
        //args.Add("loopType", "loop");    
        args.Add("loopType", iTween.LoopType.pingPong);
 
        //处理动画中的事件。  
        //开始发生动画时调用AnimationStart方法，5.0表示它的参数  
        args.Add("onstart", "AnimationStart");
        args.Add("onstartparams", 5.0f);
        //设置接受方法的对象，默认是自身接受，这里也可以改成别的对象接受，  
        //那么就得在接收对象的脚本中实现AnimationStart方法。  
        args.Add("onstarttarget", gameObject);
 
 
        //动画结束时调用，参数和上面类似  
        args.Add("oncomplete", "AnimationEnd");
        args.Add("oncompleteparams", "end");
        args.Add("oncompletetarget", gameObject);
 
        //动画中调用，参数和上面类似  
        args.Add("onupdate", "AnimationUpdate");
        args.Add("onupdatetarget", gameObject);
        args.Add("onupdateparams", true);
 
        iTween.ColorTo(btnBegin, args);
 ----------------------- iTween -----------------*/
