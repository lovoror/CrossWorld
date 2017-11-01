using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	private Transform owner;   // 此近战武器的拥有者
	private PlayerManager PlayerMG;  // owner的PlayerManager管理类

	private List<string> ownerTags = new List<string> { "Player", "Enemy" };

	// Use this for initialization
	void Start () {
		owner = Utils.GetOwner(transform, ownerTags);
		if (owner) {
			PlayerMG = owner.GetComponent<PlayerManager>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 通知PlayerManager伤害了某（些）人
	protected void BasicHurt(Transform owner, List<Transform> suffers, List<float> damages)
	{
		PlayerMG.HurtDeclaration(owner, suffers, damages);
	}
	protected void BasicHurt(Transform owner, List<Transform> suffers, float damage)
	{
		PlayerMG.HurtDeclaration(owner, suffers, damage);
	}
	protected void BasicHurt(Transform owner, Transform suffer, float damage)
	{
		PlayerMG.HurtDeclaration(owner, suffer, damage);
	}
}
