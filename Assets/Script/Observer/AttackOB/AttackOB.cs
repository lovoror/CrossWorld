using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer
{
	private static bool isRegisted = false;
	protected void OnEnable()
	{
		if (isRegisted) return;
		isRegisted = true;
		Messenger.HurtDeclarationEvent += new Messenger.HurtDeclarationEventHandler(HurtDeclarationEventFunc);
	}

	protected void OnDisable()
	{
		Messenger.HurtDeclarationEvent -= HurtDeclarationEventFunc;
	}

	protected new void Start(){
		base.Start();
	}

	/*--------------------- HurtEvent ---------------------*/
		/*----------- Messenger -> Observer -----------*/
	// 通知对象受伤。
	protected static void HurtDeclarationEventFunc(Transform attacker, List<Transform> suffers) {
		HurtDeal(attacker, suffers);
		HurtNotify(attacker, suffers);
	}

	protected static void HurtDeal(Transform attacker, List<Transform> suffers)
	{
		WeaponNameType atkWeaponName = GameData.GetCurWeaponName(attacker.name);
		float damage = GameData.GetBaseDamage(atkWeaponName);
		if (damage < 0) return;
		foreach (Transform suffer in suffers) {
			bool isDead = GamerHurt(suffer.name, damage);
			if (isDead) {
				if (DeadNotifyEvent != null) {
					DeadNotifyEvent(attacker, suffer, attacker.GetComponent<Manager>().GetWeaponName());
				}
				GameData.GamerDead(suffer.name);
			}
			// 武器能量改变
			WeaponEnergyChangeDeal(attacker, suffers);
		}
	}

		/*----------- Observer -> Messenger -----------*/
	public delegate void HurtNotifyEventHandler(Transform attacker, Transform suffer, float damage);
	public static event HurtNotifyEventHandler HurtNotifyEvent;
	protected static void HurtNotify(Transform attacker, List<Transform> suffers)
	{
		// 分别通知各个suffer
		foreach (Transform suffer in suffers) {
			if (HurtNotifyEvent != null) {
				float health = GameData.GetHealth(suffer.name);
				if (health >= 0) {
					HurtNotifyEvent(attacker, suffer, health);
				}
			}
		}

	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public delegate void DeadNotifyEventHandler(Transform killer, Transform dead, WeaponNameType weapon);
	public static event DeadNotifyEventHandler DeadNotifyEvent;   // 通知死亡目标
	/*--------------------- DeadEvent ---------------------*/

	/*------------ WeaponEnergyChangeEvent -------------*/
	public delegate void WeaponEnergyChangeNotifyEventHandler(Transform target, int level, float energy);
	public static event WeaponEnergyChangeNotifyEventHandler WeaponEnergyChangeNotifyEvent;
	private static float increase = 10;
	private static float decrease = -8;
	protected static void WeaponEnergyChangeDeal(Transform shooter, List<Transform> suffers)
	{
		ChangeEnergy(shooter, increase);

		foreach (Transform suffer in suffers) {
			ChangeEnergy(suffer, decrease);
		}
	}

	protected static void ChangeEnergy(Transform target, float delta)
	{
		float preEnergy = GameData.GetTotalEnergy(target.name);
		GameData.ChangeEnergy(target.name, delta);
		float curEnergy = GameData.GetTotalEnergy(target.name);
		if (preEnergy != curEnergy) {
			int level = GameData.GetWeaponLevel(target.name);
			float leftEnergy = GameData.GetWeaponEnergy(target.name);
			if (WeaponEnergyChangeNotifyEvent != null && leftEnergy >= 0) {
				WeaponEnergyChangeNotifyEvent(target, level, leftEnergy);
			}
		}
	}
	/*------------ WeaponEnergyChangeEvent -------------*/
}
