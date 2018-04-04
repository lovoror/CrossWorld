
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

	/*-------------------- FocusTargetChangeEvent --------------------*/
	void FocusTargetChangeEventFunc()
	{
		Transform target = PlayerData.Instance.GetFocusTarget();
		I_PlayerManager.I_PlayerController.SetFocusTarget(target);
		preFocusTarget = target;
	}
	/*-------------------- FocusTargetChangeEvent --------------------*/

	/*-------------------- FocusTargetsChangeEvent --------------------*/
	public delegate void FocusTargetsChangeEventHandler();
	public static event FocusTargetsChangeEventHandler FocusTargetsChangeEvent;
	public void FocusTragetsChange()
	{
		Transform playerFocusTarget = I_PlayerManager.I_PlayerController.GetFocusTarget();
		Transform focusTarget = I_PlayerManager.I_PlayerData.GetFocusTarget();
		if (focusTarget != playerFocusTarget) {
			I_PlayerManager.I_PlayerController.SetFocusTarget(focusTarget);
		}
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
			I_PlayerManager.I_PlayerController.SetFocusTarget(null);
			preFocusTarget = null;
		}
	}
	/*-------------------- DeadNotifyEvent --------------------*/
}
