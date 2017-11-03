using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer {

	/*--------------------- HurtEvent ---------------------*/
		/*----------- Messenger -> Observer -----------*/
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

		/*----------- Observer -> Messenger -----------*/
	public delegate void HurtNotifyEventHandler(Transform attacker, List<Transform> suffers, List<float> damages);
	public static event HurtNotifyEventHandler HurtNotifyEvent;
	public static void HurtNotify(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		if (HurtNotifyEvent != null) {
			HurtNotifyEvent(attacker, suffers, damages);
		}
	}
	/*--------------------- HurtEvent ---------------------*/
}
