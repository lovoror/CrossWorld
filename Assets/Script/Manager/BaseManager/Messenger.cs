using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*---------------------------
 * Someone与Observer间的通信类。
 * Someone->Observer
 *--------------------------*/
public class Messenger : MonoBehaviour {

	protected Transform self;
	protected Manager I_Manager;

	protected void Awake()
	{
		self = transform;
		I_Manager = self.GetComponent<Manager>();
	}

	protected void Start()
	{

	}

	protected void OnEnable()
	{
		// HurtEvent
		AttackOB.HurtNotifyEvent += new AttackOB.HurtNotifyEventHandler(HurtNotifyEventFunc);
		// DeadEvent
		AttackOB.DeadNotifyEvent += new AttackOB.DeadNotifyEventHandler(DeadNotifyEventFunc);
	}

	protected void OnDisable()
	{
		AttackOB.HurtNotifyEvent -= HurtNotifyEventFunc;
		AttackOB.DeadNotifyEvent -= DeadNotifyEventFunc;
	}


	/*--------------------- HurtEvent ---------------------*/
		/*----------- Messenger -> Observer -----------*/
	public delegate void HurtDeclarationEventHandler(Transform attacker, List<Transform> suffers);
	public static event HurtDeclarationEventHandler HurtDeclarationEvent;
	public void HurtDeclaration(Transform attacker, List<Transform> suffers)
	{
		if (HurtDeclarationEvent != null) {
			HurtDeclarationEvent(attacker, suffers);
		}
	}

		/*----------- Observer -> Messenger -----------*/	
	protected virtual void HurtNotifyEventFunc(Transform attacker, Transform suffer)
	{
		if (self != attacker && suffer == self) {
			HurtNotifyEventDeal(attacker, self);
		}
	}

	protected void HurtNotifyEventDeal(Transform attacker, Transform suffer)
	{
		if (I_Manager != null) {
			I_Manager.HurtNotifyEventDeal(attacker, suffer);
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public delegate void DeadNotifyEventHandler(Transform dead, Transform killer);
	public DeadNotifyEventHandler DeadNotifyEvent;
	protected virtual void DeadNotifyEventFunc(Transform killer, Transform dead, WeaponNameType weaponName)
	{
		if (self == dead || self == killer) {
			if (DeadNotifyEvent != null) {
				DeadNotifyEvent(killer, dead);
			}
			if (self == dead) {
				I_Manager.SetKilledWeapon(weaponName);
			}
		}
	}
	/*--------------------- DeadEvent ---------------------*/
}
