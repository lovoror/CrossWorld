using UnityEngine;
using System.Collections;

public class EnemyController : Controller {

	private PathMover I_PathMover;
	private bool isMoving = false;
	private Transform atkTarget = null;  // 发现所要攻击的目标

	protected new void Start()
	{
		base.Start();
		I_PathMover = self.GetComponent<PathMover>();
	}

	protected new void OnEnable()
	{
		base.OnEnable();
		// 敌人（对Enemy来说就是Player）进入攻击范围事件
		I_Manager.I_WeaponManager.EnemyInATKRangeEvent += new WeaponManager.EnemyInATKRangeEventHandler(EnemyInATKRangeEventFunc);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		I_Manager.I_WeaponManager.EnemyInATKRangeEvent -= EnemyInATKRangeEventFunc;
	}

	protected new void Update()
	{
		base.Update();
		if (isMoving) {
			Vector3 velocity = I_PathMover.GetVelocity();
			velocity.y = 0;
			// 设置状态机
			ShowWalkAnim(true);
			// 改变Leg的朝向
			leg.eulerAngles = new Vector3(90, 0, Utils.GetAngle(Vector3.right, velocity));
			// 人物转向
			body.eulerAngles = new Vector3(90, 0, Utils.GetAngle(Vector3.right, velocity));
		}
		else {
			// 设置状态机
			ShowWalkAnim(false);
		}
	}

	/*-------------------- DeadEvent ---------------------*/
	protected override void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		base.DeadNotifyEventFunc(killer, dead);
		if (dead == self) {
			isMoving = false;
			atkTarget = null;
			rb.velocity = Vector3.zero;
			ShowAttackAnim(false);
		}
		if (dead == atkTarget) {
			atkTarget = null;
			ShowAttackAnim(false);
		}
	}
	/*-------------------- DeadEvent ---------------------*/

	/*----------------- WeaponNoiseEvent ------------------*/
	void OnTargetReached()
	{
		isMoving = false;
	}
	protected override void WeaponNoiseNotifyEventFunc(Transform source, float radius)
	{
		// 敌人枪声 && 自己没有死亡 && 没有发现目标 --> 追踪到声音发乎地点
		if (source.tag == "Player" && !I_Manager.IsPlayerDead() && atkTarget == null) {
			Vector3 delta = source.position - transform.position;
			if (delta.magnitude <= radius) {
				isMoving = true;
				I_PathMover.MoveTo(source.position, OnTargetReached);
			}
		}
	}
	/*----------------- WeaponNoiseEvent ------------------*/

	/*----------------- EnemyInATKRangeEvent ------------------*/
	void EnemyInATKRangeEventFunc(Transform enemy, bool isInRange)
	{
		if (isInRange) {
			// Test
			atkTarget = enemy;
			ShowAttackAnim(true);
		}
		else {
			// Test
			atkTarget = null;
			ShowAttackAnim(false);
		}
	}
	/*----------------- EnemyInATKRangeEvent ------------------*/

}
