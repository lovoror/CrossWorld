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
		if (self) {
			I_Manager = self.GetComponent<Manager>();
		}
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
		// WeaponEnergyChangeEvent
		AttackOB.WeaponEnergyChangeNotifyEvent += new DistantAttackOB.WeaponEnergyChangeNotifyEventHandler(WeaponEnergyChangeNotifyEventFunc);
		// WeaponNoiseEvent
		AttackOB.WeaponNoiseNotifyEvent += new AttackOB.WeaponNoiseNotifyEventHandler(WeaponNoiseNotifyEventFunc);
	}

	protected void OnDisable()
	{
		AttackOB.HurtNotifyEvent -= HurtNotifyEventFunc;
		AttackOB.DeadNotifyEvent -= DeadNotifyEventFunc;
		AttackOB.WeaponEnergyChangeNotifyEvent -= WeaponEnergyChangeNotifyEventFunc;
		AttackOB.WeaponNoiseNotifyEvent -= WeaponNoiseNotifyEventFunc;

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
	void HurtNotifyEventFunc(Transform attacker, Transform suffer, float health)
	{
		if (self != attacker && suffer == self) {
			HurtNotifyEventDeal(attacker, self, health);
		}
	}

	protected void HurtNotifyEventDeal(Transform attacker, Transform suffer, float health)
	{
		if (I_Manager != null) {
			I_Manager.HurtNotifyEventDeal(attacker, suffer, health);
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public delegate void DeadNotifyEventHandler(Transform dead, Transform killer);
	public DeadNotifyEventHandler DeadNotifyEvent;
	void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		if (self == dead || self == killer) {
			if (DeadNotifyEvent != null) {
				DeadNotifyEvent(killer, dead);
			}
		}
	}
	/*--------------------- DeadEvent ---------------------*/


	/*------------- WeaponEnergyChangeEvent ---------------*/
	public delegate void WeaponEnergyChangeNotifyEventHandler(Transform target, int level, float energy);
	public event WeaponEnergyChangeNotifyEventHandler WeaponEnergyChangeNotifyEvent;
	void WeaponEnergyChangeNotifyEventFunc(Transform target, int level, float energy)
	{
		if (target == self) {
			if (WeaponEnergyChangeNotifyEvent != null) {
				WeaponEnergyChangeNotifyEvent(target, level, energy);
			}
		}
	}
	/*------------- WeaponEnergyChangeEvent ---------------*/

	/*----------------- WeaponNoiseEvent ------------------*/
	public delegate void WeaponNoiseDeclarationEventHandler(Transform source, float radius);
	public static event WeaponNoiseDeclarationEventHandler WeaponNoiseDeclarationEvent;
	public void WeaponNoiseDeclaration(Transform source, float radius)
	{
		if (WeaponNoiseDeclarationEvent != null) {
			WeaponNoiseDeclarationEvent(source, radius);
		}
	}

	public delegate void WeaponNoiseNotifyEventHandler(Transform source, float radius);
	public event WeaponNoiseDeclarationEventHandler WeaponNoiseNotifyEvent;
	void WeaponNoiseNotifyEventFunc(Transform source, float radius)
	{
		if (source == self) return;
		if (WeaponNoiseNotifyEvent != null) {
			WeaponNoiseNotifyEvent(source, radius);
		}
	}

	/*----------------- WeaponNoiseEvent ------------------*/
}
