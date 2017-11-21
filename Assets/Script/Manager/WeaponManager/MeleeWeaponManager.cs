using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponManager : WeaponManager {

	private List<Transform> enemysInRange = new List<Transform>();  // 在近战攻击范围内的敌人
	private List<Transform> suffers  = new List<Transform>();  // 本次近战攻击到的敌人
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
		// 攻击状态开始时，suffers记录此刻攻击范围内的敌人。
		if (canDamage) {
			foreach (Transform suffer in enemysInRange) {
				if (!suffers.Contains(suffer)) {
					suffers.Add(suffer);
				}
			}
		}
		else {
			// 计算伤害
			if (self) {
				BasicHurt(self, suffers);
			}
			suffers.RemoveRange(0, suffers.Count);
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	// 将dead从enemysInRange中排除出去
	public override void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		if (killer == self && enemysInRange.Contains(dead)) {
			enemysInRange.Remove(dead);
		}
	}
	/*--------------------- DeadEvent ---------------------*/

	protected new void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		Transform suffer = GetSuffer(self, other);
		// enemysInRange始终记录当前攻击氛围内的敌人
		if (suffer && !enemysInRange.Contains(suffer)) {
			enemysInRange.Add(suffer);
		}
		// 当处于攻击状态时，suffers记录下新进入攻击范围内的人
		if (inDamaging) {
			if (suffer && !suffers.Contains(suffer)) {
				suffers.Add(suffer);
			}
		}
	}

	protected new void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);
		Transform suffer = GetSuffer(self, other);
		if (suffer) {
			if (enemysInRange.Contains(suffer)) {
				enemysInRange.Remove(suffer);
			}
		}
	}

	Transform GetSuffer(Transform self, Collider other)
	{
		if (other.isTrigger) return null;
		Transform suffer = null;
		if (self.tag == "Player") {
			List<string> sufferTags = new List<string>();
			foreach (string tag in Constant.TAGS.Attackable) {
				if (tag != "Player") {  // 必须碰到Enemy的身体，而不是Trigger
					sufferTags.Add(tag);
				}
			}
			suffer = Utils.GetOwner(other.transform, sufferTags);
		}
		else if (self.tag == "Enemy") {
			List<string> sufferTags = new List<string>();
			foreach (string tag in Constant.TAGS.Attackable) {
				if (tag != "Enemy") {
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
		return suffers.Count > 0;
	}
}
