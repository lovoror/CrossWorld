using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*---------------------------
 * Someone与Observer间的通信类。
 * Someone->Observer
 *--------------------------*/
public class Messenger : MonoBehaviour {

	protected Transform owner;

	void Start()
	{
		List<string> tags = new List<string>{"Player", "Enemy"};
		owner = Utils.GetOwner(transform, tags);
	}

	/*--------------------- 调用: Self --> OB ---------------------*/
	// 通知Observer伤害了某（些）人
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

	// 自己受伤事件
	protected void SelfHurtedFunc(Transform attacker, Transform suffer, float damage)
	{

	}


	/*--------------------- 事件: OB --> Self ---------------------*/
	void OnEnable()
	{
		// 受伤事件
		AttackOB.HurtEvent += new AttackOB.HurtEventHandler(HurtNotified);
	}

	void OnDisable()
	{
		AttackOB.HurtEvent -= HurtNotified;
	}

	// 得到通知被某（些）人伤害了
	void HurtNotified(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		int index = suffers.IndexOf(owner);
		if (owner != attacker && (index >= 0)) {
			float damage = index < damages.Count ? damages[index] : damages[0];
			SelfHurtedFunc(attacker, owner, damage);
		}
	}
}
