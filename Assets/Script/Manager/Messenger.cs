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

	void OnEnable()
	{
		// 受伤事件
		AttackOB.HurtNotifyEvent += new AttackOB.HurtNotifyEventHandler(HurtNotifyEventFunc);
	}

	void OnDisable()
	{
		AttackOB.HurtNotifyEvent -= HurtNotifyEventFunc;
	}


	/*--------------------- HurtEvent ---------------------*/
		/*----------- Messenger -> Observer -----------*/
	public void HurtDeclaration(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		AttackOB.HurtDeclaration(attacker, suffers, damages);
	}
	public void HurtDeclaration(Transform attacker, List<Transform> suffers, float damage)
	{
		AttackOB.HurtDeclaration(attacker, suffers, damage);
	}
	public void HurtDeclaration(Transform attacker, Transform suffer, float damage)
	{
		AttackOB.HurtDeclaration(attacker, suffer, damage);
	}

		/*----------- Observer -> Messenger -----------*/	
	void HurtNotifyEventFunc(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		int index = suffers.IndexOf(owner);
		if (owner != attacker && (index >= 0)) {
			float damage = index < damages.Count ? damages[index] : damages[0];
			HurtNotifyEventDeal(attacker, owner, damage);
		}
	}

	protected void HurtNotifyEventDeal(Transform attacker, Transform suffer, float damage)
	{
		if (I_Manager != null) {
			I_Manager.HurtNotifyEventDeal(attacker, suffer, damage);
		}
	}
	/*--------------------- HurtEvent ---------------------*/
}
