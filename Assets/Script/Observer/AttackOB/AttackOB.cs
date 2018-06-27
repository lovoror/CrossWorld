﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer
{
	private static bool isRegisted = false;
	protected static InitData I_InitData;
	static AttackOB Instance;

	public delegate void AddScoreEventHandler();
	public static event AddScoreEventHandler AddScoreEvent;

	static int birthPointNum
	{
		get
		{
			return I_InitData.birthPoints.Count;
		}
	}

	protected void Awake()
	{
		Instance = this;
	}
	protected new void OnEnable()
	{
		base.OnEnable();
		if (isRegisted) return;
		Messenger.HurtDeclarationEvent += new Messenger.HurtDeclarationEventHandler(HurtDeclarationEventFunc);
		isRegisted = true;
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		Messenger.HurtDeclarationEvent -= HurtDeclarationEventFunc;
	}

	protected new void Start(){
		base.Start();
		I_InitData = transform.GetComponent<InitData>();
	}

	/*--------------------- HurtEvent ---------------------*/
		/*----------- Messenger -> Observer -----------*/
	// 通知对象受伤。
	protected static void HurtDeclarationEventFunc(Transform attacker, List<Transform> suffers) {
		HurtDeal(attacker, suffers);
		HurtNotify(attacker, suffers);
	}

	protected static void HurtDeal(Transform attacker, List<Transform> suffers)
	{
		var attackerData = Utils.GetBaseData(attacker);
		WeaponNameType atkWeaponName = WeaponNameType.unknown;
		if (attackerData != null) {
			atkWeaponName = attackerData.curWeaponName;
		}
		float damage = Constant.GetBaseDamage(atkWeaponName);
		float damageRate = Utils.GetWeaponDamageRate(atkWeaponName);
		if (damageRate >= 0) {
			damage *= damageRate;
		}
		if (damage < 0) return;
		foreach (Transform suffer in suffers) {
			bool isDead = GamerHurt(suffer, damage);
			var sufferData = Utils.GetBaseData(suffer);
			if (isDead) {
				sufferData.isDead = true;
				if (!sufferData.isPlayer) {
					NewEnemy(suffer);
					AddScore(suffer);
				}
				if (DeadNotifyEvent != null) {
					DeadNotifyEvent(attacker, suffer, attacker.GetComponent<Manager>().GetCurWeaponName());
				}
			}
			// 武器能量改变
			WeaponEnergyChangeDeal(attacker, suffers, damage);
		}
	}

	protected static void NewEnemy(Transform dead)
	{
		BaseData deadData = Utils.GetBaseData(dead);
		if (!deadData.isPlayer) {
			string args = dead.name.Substring(10, 1);
			int index = Mathf.RoundToInt(Random.Range(-0.49f, birthPointNum - 1 + 0.49f));
			args = args + "-" + index;
			Instance.StartCoroutine("NewEnemyCoroutine", args);
			// 显示重生点标记
			BirthPointManager.ShowBirthPoint(index, true);
		}
	}

	IEnumerator NewEnemyCoroutine(string args)
	{
		yield return new WaitForSeconds(3);

		string[] sArray = args.Split('-');
		int enemyType = int.Parse(sArray[0]);
		int index = int.Parse(sArray[1]);
		I_InitData.NewEnemy(enemyType, index);
	}

	static void AddScore(Transform dead)
	{
		BaseData deadData = Utils.GetBaseData(dead);
		if (!deadData.isPlayer) {
			GlobalData.Instance.AddScore(10);
			if (AddScoreEvent != null) {
				AddScoreEvent();
			}
		}	
	}

		/*----------- Observer -> Messenger -----------*/
	public delegate void HurtNotifyEventHandler(Transform attacker, Transform suffer);
	public static event HurtNotifyEventHandler HurtNotifyEvent;
	protected static void HurtNotify(Transform attacker, List<Transform> suffers)
	{
		// 分别通知各个suffer
		foreach (Transform suffer in suffers) {
			if (HurtNotifyEvent != null) {
				HurtNotifyEvent(attacker, suffer);
			}
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public delegate void DeadNotifyEventHandler(Transform killer, Transform dead, WeaponNameType weapon);
	public static event DeadNotifyEventHandler DeadNotifyEvent;   // 通知死亡目标
	/*--------------------- DeadEvent ---------------------*/

	/*------------ WeaponEnergyChangeEvent -------------*/
	//public delegate void WeaponEnergyChangeNotifyEventHandler(Transform target);
	//public static event WeaponEnergyChangeNotifyEventHandler WeaponEnergyChangeNotifyEvent;
	private static float increaseRate = 0.2f;
	private static float decreaseRate = -2.5f;
	protected static void WeaponEnergyChangeDeal(Transform shooter, List<Transform> suffers, float damage)
	{
		ChangeEnergy(shooter, increaseRate * damage);

		foreach (Transform suffer in suffers) {
			ChangeEnergy(suffer, decreaseRate * damage);
		}
	}

	protected static void ChangeEnergy(Transform target, float delta)
	{
		var targetData = Utils.GetBaseData(target);
		if (targetData != null) {
			targetData.curWeaponEnergy += delta;
		}
		//if (WeaponEnergyChangeNotifyEvent != null) {
		//	WeaponEnergyChangeNotifyEvent(target);
		//}
	}
	/*------------ WeaponEnergyChangeEvent -------------*/

	public static new void StageEnd()
	{
		Observer.StageEnd();
		isRegisted = false;
	}
}
