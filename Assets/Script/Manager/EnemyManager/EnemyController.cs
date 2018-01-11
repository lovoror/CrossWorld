using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections;

public class EnemyController : Controller {

	private BehaviorTree behaviorTree;
	//private bool isMoving = false;

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
	}

	/*-------------------- DeadEvent ---------------------*/
	protected override void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		base.DeadNotifyEventFunc(killer, dead);
		if (dead == self) {
			//isMoving = false;
			if (rb) {
				rb.velocity = Vector3.zero;
			}
		}
	}
	/*-------------------- DeadEvent ---------------------*/
}
