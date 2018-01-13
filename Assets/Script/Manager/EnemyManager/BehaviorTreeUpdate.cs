using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BehaviorTreeUpdate : MonoBehaviour {
	public BehaviorTree behaviorTree;
	public GameObject patrolPoints;

	Pathfinding.AIPathAgent pathAgent;
	Manager I_Manager;
	SharedFloat baseSpeed;
	SharedFloat curSpeed;
	Transform body;
	Transform leg;
	GameObject player;
	Animator legAnim;
	Animator bodyAnim;
	AnimatorStateInfo bodyAnimInfo;
	SpriteRenderer bodyRender;
	bool isAttacking = false;
	bool isDead = false;
	float attackSpeedRate = 1.0f;

	void Awake()
	{
		pathAgent = transform.GetComponent<Pathfinding.AIPathAgent>();
		player = GameObject.FindGameObjectWithTag("Player");
		behaviorTree.SetVariable("Player", (SharedGameObject)player);
		behaviorTree.SetVariable("PatrolPoints", (SharedGameObject)patrolPoints);
		body = transform.Find("Body");
		leg = transform.Find("Leg");
		legAnim = leg.GetComponent<Animator>();
		bodyAnim = body.GetComponent<Animator>();
		bodyRender = body.GetComponent<SpriteRenderer>();
		I_Manager = transform.GetComponent<Manager>();
	}

	void OnEnable()
	{
		I_Manager.I_DataManager.AttackSpeedChangeEvent += new DataManager.AttackSpeedChangeEventHandler(AttackSpeedChangeEventFunc);
	}

	void OnDisable()
	{
		I_Manager.I_DataManager.AttackSpeedChangeEvent -= AttackSpeedChangeEventFunc;
	}

	void Start ()
	{
		baseSpeed = (float)behaviorTree.GetVariable("RunSpeed").GetValue();
	}

	float curTime = 0;
	const float deltaTime = 0.3f;
	Vector3? prePos;
	void Update ()
	{
		isDead = (bool)behaviorTree.GetVariable("IsDead").GetValue();
		if (isDead) {
			legAnim.SetBool("Dead", true);
			//bodyAnim.SetInteger("DeadState", I_Manager.GetKilledWeapon());
			bodyAnim.SetBool("Dead", true);
			this.enabled = false;
			Collider playerCollider = player.transform.GetComponent<Collider>(); ;
			Collider selfCollider = transform.GetComponent<Collider>();
			Physics.IgnoreCollision(playerCollider, selfCollider);
			bodyRender.sortingLayerName = "Default";
			return;
		}
		// 设置状态机
		isAttacking = (bool)behaviorTree.GetVariable("Attack").GetValue();
		bodyAnim.SetBool("Attack", isAttacking);

		// 状态动画速度变更
		curTime += Time.deltaTime;
		if (curTime >= deltaTime) {
			curTime -= deltaTime;
			bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
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
	}

	/*-------------------- AttackSpeedChangeEvent ---------------------*/
	void AttackSpeedChangeEventFunc(float rate)
	{
		attackSpeedRate = rate;
		bodyAnim.SetFloat("AttackSpeed", rate);
	}
	/*-------------------- AttackSpeedChangeEvent ---------------------*/
}
