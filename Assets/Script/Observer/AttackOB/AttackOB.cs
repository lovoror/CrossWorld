using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer
{
	public delegate void PlayerChangeWeaponNotifyEventHandler(Transform player);
	public static event PlayerChangeWeaponNotifyEventHandler PlayerChangeWeaponNotifyEvent;

	private static bool isRegisted = false;
	protected new void OnEnable()
	{
		base.OnEnable();
		if (isRegisted) return;
		Messenger.HurtDeclarationEvent += new Messenger.HurtDeclarationEventHandler(HurtDeclarationEventFunc);
		FuncRButton.PlayerChangeWeaponEvent += new FuncRButton.PlayerChangeWeaponEventHandler(PlayerChangeWeaponEventFunc);
		isRegisted = true;
	}

	protected void OnDisable()
	{
		base.OnDisable();
		Messenger.HurtDeclarationEvent -= HurtDeclarationEventFunc;
		FuncRButton.PlayerChangeWeaponEvent -= PlayerChangeWeaponEventFunc;
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
		var attackerData = Utils.GetBaseData(attacker);
		WeaponNameType atkWeaponName = WeaponNameType.unknown;
		if (attackerData != null) {
			atkWeaponName = attackerData.curWeaponName;
		}
		float damage = Constant.GetBaseDamage(atkWeaponName);
		if (damage < 0) return;
		foreach (Transform suffer in suffers) {
			bool isDead = GamerHurt(suffer, damage);
			var sufferData = Utils.GetBaseData(suffer);
			if (isDead) {
				if (DeadNotifyEvent != null) {
					DeadNotifyEvent(attacker, suffer, attacker.GetComponent<Manager>().GetCurWeaponName());
				}
				sufferData.isDead = true;
			}
			// 武器能量改变
			WeaponEnergyChangeDeal(attacker, suffers, damage);
		}
	}

		/*----------- Observer -> Messenger -----------*/
	public delegate void HurtNotifyEventHandler(Transform attacker, Transform suffer);
	public static event HurtNotifyEventHandler HurtNotifyEvent;
	protected static void HurtNotify(Transform attacker, List<Transform> suffers)
	{
		// 分别通知各个suffer
		foreach (Transform suffer in suffers) {
			if (HurtNotifyEvent != null) {
				HurtNotifyEvent(attacker, suffer);
			}
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public delegate void DeadNotifyEventHandler(Transform killer, Transform dead, WeaponNameType weapon);
	public static event DeadNotifyEventHandler DeadNotifyEvent;   // 通知死亡目标
	/*--------------------- DeadEvent ---------------------*/

	/*------------ WeaponEnergyChangeEvent -------------*/
	//public delegate void WeaponEnergyChangeNotifyEventHandler(Transform target);
	//public static event WeaponEnergyChangeNotifyEventHandler WeaponEnergyChangeNotifyEvent;
	private static float increaseRate = 0.6f;
	private static float decreaseRate = -0.6f;
	protected static void WeaponEnergyChangeDeal(Transform shooter, List<Transform> suffers, float damage)
	{
		ChangeEnergy(shooter, increaseRate * damage);

		foreach (Transform suffer in suffers) {
			ChangeEnergy(suffer, decreaseRate * damage);
		}
	}

	protected static void ChangeEnergy(Transform target, float delta)
	{
		var targetData = Utils.GetBaseData(target);
		if (targetData != null) {
			targetData.curWeaponEnergy += delta;
		}
		//if (WeaponEnergyChangeNotifyEvent != null) {
		//	WeaponEnergyChangeNotifyEvent(target);
		//}
	}
	/*------------ WeaponEnergyChangeEvent -------------*/

	/*------------ PlayerChangeWeaponEvent --------------*/
	void PlayerChangeWeaponEventFunc(Transform player, WeaponNameType weaponName)
	{
		PlayerData.Instance.curWeaponName = weaponName;
		if (PlayerChangeWeaponNotifyEvent != null) {
			PlayerChangeWeaponNotifyEvent(player);
		}
	}
	/*------------ PlayerChangeWeaponEvent --------------*/

	public static new void StageEnd()
	{
		Observer.StageEnd();
		isRegisted = false;
	}
}
