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
	void Update ()
	{
		isDead = (bool)behaviorTree.GetVariable("IsDead").GetValue();
		if (isDead) {
			if (firstDead) {
				legAnim.SetBool("Dead", true);
				int deadState = Mathf.RoundToInt(Random.Range(0.49f, 7.49f));
				bodyAnim.SetInteger("DeadState", deadState);
				bodyAnim.SetBool("Dead", true);
				this.enabled = false;
				Collider selfCollider = transform.GetComponent<Collider>();
				Collider playerCollider = player.transform.GetComponent<Collider>();
				Physics.IgnoreCollision(playerCollider, selfCollider);
				bodyRender.sortingLayerName = "Default";
				bodyRender.sortingOrder = sortingOrder++;
				firstDead = false;
				// 重置Alert
				if (EnemyAlertStateEvent != null) {
					curAlertState = false;
					EnemyAlertStateEvent(transform, false);
				}
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
}
