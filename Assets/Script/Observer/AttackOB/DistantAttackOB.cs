using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantAttackOB : AttackOB {

	new void Start () {
		base.Start();
	}

	protected new void OnEnable()
	{
		base.OnEnable();
		BulletController.BulletHitEvent += new BulletController.BulletHitEventHandler(BulletHitEventFunc);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		BulletController.BulletHitEvent -= BulletHitEventFunc;
	}

	/*----------------- BulletHitEvent -----------------*/
	// Bullet --> Observer
	void BulletHitEventFunc(Transform shooter, Transform suffer, float damage)
	{
		List<Transform> suffers = new List<Transform>() { suffer };
		HurtDeal(shooter, suffers);
		HurtNotify(shooter, suffers);
	}
	/*----------------- BulletHitEvent -----------------*/
}
