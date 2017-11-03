using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	[HideInInspector]
	public Transform owner;   // 此近战武器的拥有者
	protected PlayerManager I_PlayerManager;  // attacker的PlayerManager管理类

	protected void Awake()
	{
		owner = Utils.GetOwner(transform, Constant.TAGS.Attacker);
		if (owner) {
			I_PlayerManager = owner.GetComponent<PlayerManager>();
		}
	}

	protected void Start () {

	}

	/*--------------------- 事件: Self --> Player/Enemy ---------------------*/
	// 受伤事件:WeaponManager通知attacker的Manager
	public delegate void HurtDeclarationEventHandler(Transform attacker, List<Transform> suffers, List<float> damages);
	public event HurtDeclarationEventHandler HurtDeclarationEvent;
	

	// 通知PlayerManager伤害了某（些）人
	protected void BasicHurt(Transform attacker, List<Transform> suffers, List<float> damages)
	{
		if (HurtDeclarationEvent != null) {
			HurtDeclarationEvent(attacker, suffers, damages);
		}
	}
	protected void BasicHurt(Transform attacker, List<Transform> suffers, float damage)
	{
		List<float> damages = new List<float> { damage };
		BasicHurt(attacker, suffers, damages);
	}
	protected void BasicHurt(Transform attacker, Transform suffer, float damage)
	{
		List<float> damages = new List<float> { damage };
		List<Transform> suffers = new List<Transform> { suffer };
		BasicHurt(attacker, suffers, damages);
	}
}
