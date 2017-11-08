using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	protected Rigidbody rb;
	protected Transform body;
	protected Transform leg;
	protected GameObject bodyCollider;
	protected Animator legAnim;
	protected Animator bodyAnim;
	protected Camera mainCamera;
	protected bool canControl;

	protected void Start ()
	{
		rb = transform.GetComponent<Rigidbody>();
		body = transform.Find("Body");
		leg = transform.Find("Leg");
		bodyCollider = Utils.FindChildRecursively(transform, "BodyCollider").gameObject;
		legAnim = leg.GetComponent<Animator>();
		bodyAnim = body.GetComponent<Animator>();
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	public void DeadEventFunc(Transform target)
	{
		if (transform == target) {
			// 设置死亡状态
			ShowDeadAnim(true);
			canControl = false;
			bodyCollider.SetActive(false);
		}
	}

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
