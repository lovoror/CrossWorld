using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer {

	protected new void Start(){
		base.Start();
	}

	/*--------------------- HurtEvent ---------------------*/
		/*----------- Messenger -> Observer -----------*/
	// 通知对象受伤。
	public static void HurtDeclaration(Transform attacker, List<Transform> suffers)
	{
		HurtDeal(attacker, suffers);
		HurtNotify(attacker, suffers);
	}
	public static void HurtDeclaration(Transform attacker, Transform suffer)
	{
		List<Transform> suffers = new List<Transform>();
		suffers.Add(suffer);
		HurtDeclaration(attacker, suffers);
	}

	protected static void HurtDeal(Transform attacker, List<Transform> suffers)
	{
		string atkWeapon = gamerInfos[attacker.name].curWeapon;
		float damage = baseDamage[atkWeapon];
		foreach (Transform suffer in suffers) {
			bool isDead = GamerHurt(suffer.name, damage);
			if (isDead) {
				if (DeadNotifyEvent != null) {
					DeadNotifyEvent(suffer, true);
				}
			}
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
				float health = gamerInfos[suffer.name].health;
				HurtNotifyEvent(attacker, suffer, health);
			}
		}

	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public delegate void DeadNotifyEventHandler(Transform target, bool isDead);
	public static event DeadNotifyEventHandler DeadNotifyEvent;

	/*--------------------- DeadEvent ---------------------*/

}
