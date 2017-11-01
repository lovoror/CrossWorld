using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponManager : WeaponManager {

	public float damage = 10;

	private List<Transform> enemysInRange = new List<Transform>();  // 在近战攻击范围内的敌人
	private List<Transform> suffers = new List<Transform>();  // 本次近战攻击到的敌人
	private bool inDamaging;  // 是否处于可造成伤害的阶段
	private Transform owner;   // 此近战武器的拥有者
	private PlayerManager PlayerMG;  // owner的PlayerManager管理类

	private List<string> ownerTags = new List<string> { "Player", "Enemy" };

	void Start()
	{
		owner = Utils.GetOwner(transform, ownerTags);
		if (owner) {
			PlayerMG = owner.GetComponent<PlayerManager>();
		}
	}

	void Update()
	{

	}
	void LateUpdate()
	{
		if (inDamaging) {
			foreach (Transform enemy in enemysInRange) {
				if (suffers.IndexOf(enemy) < 0) {
					suffers.Add(enemy);
				}
			}
		}
	}

	void OnEnable()
	{
		// 伤害开始和结束事件
		BodyAnimEvents.MeleeHurtEvent += new BodyAnimEvents.MeleeDamageEventHandler(MeleeHurtEventFunc);
	}
	void OnDisable()
	{
		BodyAnimEvents.MeleeHurtEvent -= MeleeHurtEventFunc;
	}
	/*--------------------- 事件: BodyAnimEvents --> Self ---------------------*/
	void MeleeHurtEventFunc(bool canDamage)
	{
		inDamaging = canDamage;
		if (canDamage) {
			suffers.RemoveRange(0, suffers.Count);
		}
		else {
			// 计算伤害
			if (owner) {
				BasicHurt(owner, suffers, damage);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Transform suffer;
		if (owner.tag == "Player") {
			suffer = Utils.GetOwner(other.transform, "Enemy");
		}
		else {
			suffer = Utils.GetOwner(other.transform, "Player");
		}
		if(suffer && enemysInRange.IndexOf(suffer) < 0 ){
			enemysInRange.Add(suffer);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		Transform suffer;
		if (owner.tag == "Player") {
			suffer = Utils.GetOwner(other.transform, "Enemy");
		}
		else {
			suffer = Utils.GetOwner(other.transform, "Player");
		}
		if (suffer) {
			enemysInRange.Remove(suffer);
		}
	}

	//void OnCollisionEnter2D(Collision2D other)
	//{
	//	print("OnCollisionEnter2D");
	//}
}
