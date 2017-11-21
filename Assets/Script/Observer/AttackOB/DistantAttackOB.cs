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


	///*------------ WeaponEnergyChangeEvent -------------*/
	//public delegate void WeaponEnergyChangeNotifyEventHandler(Transform target, int level, float energy);
	//public static event WeaponEnergyChangeNotifyEventHandler WeaponEnergyChangeNotifyEvent;
	//private float increase = 10;
	//private float decrease = -8;
	//void WeaponEnergyChangeDeal(Transform shooter, List<Transform> suffers)
	//{
	//	ChangeEnergy(shooter, increase);

	//	foreach (Transform suffer in suffers) {
	//		ChangeEnergy(suffer, decrease);
	//	}
	//}

	//void ChangeEnergy(Transform target, float delta)
	//{
	//	GamerInfo gamerInfo = gamerInfos[target.name];
	//	float preEnergy = gamerInfo.GetTotalEnergy();
	//	gamerInfo.ChangeEnergy(delta);
	//	float curEnergy = gamerInfo.GetTotalEnergy();
	//	if (preEnergy != curEnergy) {
	//		int level = gamerInfo.GetWeaponLevel();
	//		float leftEnergy = gamerInfo.GetEnergy();
	//		if (WeaponEnergyChangeNotifyEvent != null) {
	//			WeaponEnergyChangeNotifyEvent(target, level, leftEnergy);
	//		}
	//	}
	//}
	///*------------ WeaponEnergyChangeEvent -------------*/
}
