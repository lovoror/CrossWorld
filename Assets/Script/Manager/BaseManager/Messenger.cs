using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*---------------------------
 * Someone与Observer间的通信类。
 * Someone->Observer
 *--------------------------*/
public class Messenger : MonoBehaviour {

	protected Transform owner;
	protected Manager I_Manager;

	protected void Awake()
	{
		owner = Utils.GetOwner(transform, Constant.TAGS.Attacker);
		if (owner) {
			I_Manager = owner.GetComponent<Manager>();
		}
	}

	protected void Start()
	{

	}

	protected void OnEnable()
	{
		// 受伤事件
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
	public void HurtDeclaration(Transform attacker, List<Transform> suffers)
	{
		AttackOB.HurtDeclaration(attacker, suffers);
	}
	public void HurtDeclaration(Transform attacker, Transform suffer)
	{
		AttackOB.HurtDeclaration(attacker, suffer);
	}

		/*----------- Observer -> Messenger -----------*/	
	void HurtNotifyEventFunc(Transform attacker, Transform suffer, float health)
	{
		if (owner != attacker && suffer == owner) {
			HurtNotifyEventDeal(attacker, owner, health);
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
	void DeadNotifyEventFunc(Transform target, bool isDead)
	{
		if (target == owner) {
			DeadNotifyEventDeal(target, isDead);
		}
	}

	protected void DeadNotifyEventDeal(Transform target, bool isDead)
	{
		if (I_Manager != null) {
			I_Manager.DeadNotifyEventDeal(target, isDead);
		}
	}
	/*--------------------- DeadEvent ---------------------*/
}
