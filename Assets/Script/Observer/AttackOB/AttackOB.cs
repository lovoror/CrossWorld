using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer {

	/*--------------------- 事件: OB --> Player/Enemy ---------------------*/
	// 通知对象受伤。
	public delegate void HurtEventHandler(Transform attacker, List<Transform> suffers, List<float> damages);
	public static event HurtEventHandler HurtEvent;
	public static void HurtNotify(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		if (HurtEvent != null) {
			HurtEvent(attacker, suffers, damages);
		}
	}

	/*--------------------- 事件: Player/Enemy --> OB ---------------------*/
	// 通知对象受伤。
	public static void HurtDeclaration(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		HurtNotify(attacker, suffers, damages);
	}
	public static void HurtDeclaration(Transform attacker, List<Transform> suffers, float damage)
	{
		List<float> damages = new List<float>();
		damages.Add(damage);
		HurtNotify(attacker, suffers, damages);
	}
	public static void HurtDeclaration(Transform attacker, Transform suffer, float damage)
	{
		List<Transform> suffers = new List<Transform>();
		List<float> damages = new List<float>();
		suffers.Add(suffer);
		damages.Add(damage);
		HurtNotify(attacker, suffers, damages);
	}
}
