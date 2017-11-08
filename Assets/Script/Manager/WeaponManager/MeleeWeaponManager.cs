using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponManager : WeaponManager {

	private List<Transform> enemysInRange = new List<Transform>();  // 在近战攻击范围内的敌人
	private List<Transform> suffers = new List<Transform>();  // 本次近战攻击到的敌人
	private bool inDamaging;  // 是否处于可造成伤害的阶段

	new void Awake()
	{
		base.Awake();

	}

	new void Start()
	{
		base.Start();
	}

	void Update()
	{

	}
	void LateUpdate()
	{
		if (inDamaging) {
			foreach (Transform enemy in enemysInRange) {
				if (!suffers.Contains(enemy)) {
					suffers.Add(enemy);
				}
			}
		}
	}

	new void OnEnable()
	{
		base.OnEnable();
		// 伤害开始和结束事件
		I_BodyAnimEvents.MeleeHurtEvent += new BodyAnimEvents.MeleeDamageEventHandler(MeleeHurtEventFunc);
	}
	new void OnDisable()
	{
		base.OnDisable();
		I_BodyAnimEvents.MeleeHurtEvent -= MeleeHurtEventFunc;
	}

	/*--------------------- HurtEvent ---------------------*/
		/*------------ Self -> Manager ------------*/
	void MeleeHurtEventFunc(bool canDamage)
	{
		inDamaging = canDamage;
		if (canDamage) {
			suffers.RemoveRange(0, suffers.Count);
		}
		else {
			// 计算伤害
			if (owner) {
				BasicHurt(owner, suffers);
			}
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	void OnTriggerEnter(Collider other)
	{
		Transform suffer = GetSuffer(owner, other);
		if(suffer && !enemysInRange.Contains(suffer)) {
			enemysInRange.Add(suffer);
		}
	}

	void OnTriggerExit(Collider other)
	{
		Transform suffer = GetSuffer(owner, other);
		if (suffer) {
			enemysInRange.Remove(suffer);
		}
	}

	Transform GetSuffer(Transform owner, Collider other)
	{
		Transform suffer = null;
		if (owner.tag == "Player") {
			List<string> sufferTags = new List<string>();
			foreach (string tag in Constant.TAGS.Attackable) {
				if (tag != "Player" && !other.isTrigger) {  // 必须碰到Enemy的身体，而不是Trigger
					sufferTags.Add(tag);
				}
			}
			suffer = Utils.GetOwner(other.transform, sufferTags);
		}
		else if (owner.tag == "Enemy") {
			List<string> sufferTags = new List<string>();
			foreach (string tag in Constant.TAGS.Attackable) {
				if (tag != "Enemy" && !other.isTrigger) {
					sufferTags.Add(tag);
				}
			}
			suffer = Utils.GetOwner(other.transform, sufferTags);
		}
		return suffer;
	}

	/*----------------- Tool --------------------*/
	// 攻击范围内存在敌人
	protected bool HasEnemyInRange()
	{
		return enemysInRange.Count > 0;
	}
}
