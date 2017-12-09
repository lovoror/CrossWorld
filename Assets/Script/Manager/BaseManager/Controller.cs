using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	[HideInInspector]
	public Manager I_Manager;

	protected Rigidbody rb;
	protected Transform self;
	protected Transform body;
	protected Transform leg;
	protected SphereCollider bodyCollider;
	protected Animator legAnim;
	protected Animator bodyAnim;
	protected Camera mainCamera;
	protected bool canControl;

	private AnimatorStateInfo bodyAnimInfo;

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
		self = transform;
	}

	protected void Start ()
	{
		rb = transform.GetComponent<Rigidbody>();
		body = transform.Find("Body");
		leg = transform.Find("Leg");
		bodyCollider = transform.GetComponent<SphereCollider>();
		legAnim = leg.GetComponent<Animator>();
		bodyAnim = body.GetComponent<Animator>();
		bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	protected void OnEnable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent += new Messenger.DeadNotifyEventHandler(DeadNotifyEventFunc);
		// WeaponNoiseEvent
		I_Manager.I_Messenger.WeaponNoiseNotifyEvent += new Messenger.WeaponNoiseDeclarationEventHandler(WeaponNoiseNotifyEventFunc);
	}

	protected void OnDisable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent -= DeadNotifyEventFunc;
		I_Manager.I_Messenger.WeaponNoiseNotifyEvent -= WeaponNoiseNotifyEventFunc;
	}

	protected void Update()
	{
		bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		if (bodyAnimInfo.IsName("Attack")) {
			bodyAnim.speed = I_Manager.I_DataManager.attackSpeedRate;
		}
		else {
			bodyAnim.speed = 1;
		}
	}

	/*-------------------- DeadEvent ---------------------*/
	protected virtual void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		if (transform == killer) {

		}
		else if (transform == dead) {
			// 设置死亡状态
			ShowDeadAnim(true);
			canControl = false;
			if (rb) {
				rb.useGravity = false;
			}
			bodyCollider.enabled = false;
			I_Manager.SetPlayerDead(true);
		}
	}
	/*-------------------- DeadEvent ---------------------*/

	/*----------------- WeaponNoiseEvent ------------------*/
	protected virtual void WeaponNoiseNotifyEventFunc(Transform source, float radius)
	{
		
	}
	/*----------------- WeaponNoiseEvent ------------------*/

	/*------------------ 状态机 ------------------*/
	protected void ShowWalkAnim(bool show)
	{
		legAnim.SetBool("Walk", show);
		bodyAnim.SetBool("Walk", show);
	}

	protected void ShowAttackAnim(bool show)
	{
		bodyAnim.SetBool("Attack", show);
	}

	protected void ShowDeadAnim(bool show)
	{
		legAnim.SetBool("Dead", show);
		bodyAnim.SetBool("Dead", show);
	}
	/*------------------ 状态机 ------------------*/
}
