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
	protected AnimatorStateInfo bodyAnimInfo;

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
		self = transform;
		rb = transform.GetComponent<Rigidbody>();
		body = transform.Find("Body");
		leg = transform.Find("Leg");
		bodyCollider = transform.GetComponent<SphereCollider>();
		legAnim = leg.GetComponent<Animator>();
		bodyAnim = body.GetComponent<Animator>();
		bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	protected void Start ()
	{

	}

	protected void OnEnable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent += new Messenger.DeadNotifyEventHandler(DeadNotifyEventFunc);
	}

	protected void OnDisable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent -= DeadNotifyEventFunc;
	}

	protected void Update()
	{

	}

	/*-------------------- DeadEvent ---------------------*/
	protected virtual void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		if (transform == killer) {

		}
		else if (transform == dead) {
			// 设置死亡状态
			ShowDeadAnim(true);
		}
	}
	/*-------------------- DeadEvent ---------------------*/

	/*------------------ 状态机 ------------------*/
	protected void ShowWalkAnim(float speedRate)
	{
		legAnim.SetFloat("Speed", speedRate);
		bodyAnim.SetBool("Walk", speedRate > 0.1);
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
