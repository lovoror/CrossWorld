
/*---------------------------
 * Player与Observer间的通信类。
 * Player->Observer
 *--------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMessenger : Messenger {

	protected PlayerManager I_PlayerManager;

	protected new void OnEnable()
	{
		base.OnEnable();
		FuncRButton.PlayerReloadEvent += new FuncRButton.PlayerReloadEventHandler(PlayerReloadEventFunc);
		AttackOB.PlayerChangeWeaponNotifyEvent += new AttackOB.PlayerChangeWeaponNotifyEventHandler(PlayerChangeWeaponNotifyEventFunc);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		FuncRButton.PlayerReloadEvent -= PlayerReloadEventFunc;
		//FuncLButton.PlayerChangeWeaponEvent -= PlayerChangeWeaponEventFunc;
	}

	new void Awake()
	{
		base.Awake();
		if (self) {
			I_PlayerManager = self.GetComponent<PlayerManager>();
		}
	}

	/*------------ PlayerReloadEvent --------------*/
	void PlayerReloadEventFunc(Transform player)
	{
		if (player == self) {

		}
	}
	/*------------ PlayerReloadEvent --------------*/

	/*------------ PlayerChangeWeaponEvent --------------*/
	void PlayerChangeWeaponNotifyEventFunc(Transform player)
	{
		if (player == self) {
			I_PlayerManager.ChangeWeapon();
		}
	}
	/*------------ PlayerChangeWeaponEvent --------------*/
}
