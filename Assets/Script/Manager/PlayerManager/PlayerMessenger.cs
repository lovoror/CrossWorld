
/*---------------------------
 * Player与Observer间的通信类。
 * Player->Observer
 *--------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMessenger : Messenger {

	protected PlayerManager I_PlayerManager;

	Transform preFocusTarget;

	protected new void OnEnable()
	{
		base.OnEnable();
		MoboController.FocusTargetChangeEvent += new MoboController.FocusTargetChangeHandler(FocusTargetChangeEventFunc);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		MoboController.FocusTargetChangeEvent -= FocusTargetChangeEventFunc;
	}

	new void Awake()
	{
		base.Awake();
		if (self) {
			I_PlayerManager = self.GetComponent<PlayerManager>();
		}
	}

	/*-------------------- FocusTargetChangedEvent --------------------*/
	void FocusTargetChangeEventFunc()
	{
		Transform target = PlayerData.Instance.GetFocusTarget();
		I_PlayerManager.I_PlayerController.FocusTarget(target);
		preFocusTarget = target;
	}
	/*-------------------- FocusTargetChangedEvent --------------------*/

	/*-------------------- FocusTargetsChangeEvent --------------------*/
	public delegate void FocusTargetsChangeEventHandler();
	public static event FocusTargetsChangeEventHandler FocusTargetsChangeEvent;
	public void FocusTragetsChange()
	{
		if (FocusTargetsChangeEvent != null) {
			FocusTargetsChangeEvent();
		}
	}
	/*-------------------- FocusTargetsChangeEvent --------------------*/

	/*-------------------- DeadNotifyEvent --------------------*/
	protected override void DeadNotifyEventFunc(Transform killer, Transform dead, WeaponNameType weaponName)
	{
		base.DeadNotifyEventFunc(killer, dead, weaponName);
		if (preFocusTarget == dead) {
			I_PlayerManager.I_PlayerController.FocusTarget(null);
			preFocusTarget = null;
		}
	}
	/*-------------------- DeadNotifyEvent --------------------*/
}
