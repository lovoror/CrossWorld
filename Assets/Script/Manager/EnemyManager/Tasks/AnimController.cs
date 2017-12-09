using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class AnimController : Action
	{
		public bool Attack = false;
		public bool Walk = false;
		public bool Dead = false;

		Transform body;
		Transform leg;
		Animator legAnim;
		Animator bodyAnim;

		private AnimatorStateInfo bodyAnimInfo;

		public override void OnAwake()
		{
			base.OnAwake();
			body = transform.Find("Body");
			leg = transform.Find("Leg");
			legAnim = leg.GetComponent<Animator>();
			bodyAnim = body.GetComponent<Animator>();
			//bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		}

		public override void OnStart()
		{
			base.OnStart();
			// 设置状态机
			legAnim.SetBool("Walk", Walk);
			bodyAnim.SetBool("Walk", Walk);
			bodyAnim.SetBool("Attack", Attack);
			legAnim.SetBool("Dead", Dead);
			bodyAnim.SetBool("Dead", Dead);
		}

		public override void OnEnd()
		{
			base.OnEnd();
		}

		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
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
}
