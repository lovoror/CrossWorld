using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOB : Observer
{
	private static bool isRegisted = false;
	protected static InitData I_InitData;
	static AttackOB Instance;

	public delegate void AddScoreEventHandler();
	public static event AddScoreEventHandler AddScoreEvent;

	public delegate void AddHealthEventHandler(Transform player, float health);
	public static event AddHealthEventHandler AddHealthEvent;

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
		float damageRate = 0;
		if (attacker == PlayerData.Instance.target) {
			damageRate = Utils.GetWeaponDamageRate(atkWeaponName) / GlobalData.diffRate;
		}
		else {
			damageRate = Utils.GetWeaponDamageRate(atkWeaponName);
		}
		if (damageRate >= 0) {
			damage *= damageRate;
		}
		if (damage < 0) return;
		foreach (Transform suffer in suffers) {
			var sufferData = Utils.GetBaseData(suffer);
			float trueDamage = Mathf.Min(damage, sufferData.curHealth);
			bool isDead = GamerHurt(suffer, damage);
			if (isDead) {
				sufferData.isDead = true;
				if (!sufferData.isPlayer) {
					SetGlobalInfo(suffer);
					NewEnemy(suffer);
				}
				if (DeadNotifyEvent != null) {
					DeadNotifyEvent(attacker, suffer, attacker.GetComponent<Manager>().GetCurWeaponName());
				}
			}
			// 武器能量改变
			WeaponEnergyChangeDeal(attacker, suffers, damage);
			// Player生命回复
			PlayerHealthRestoreDeal(attacker, trueDamage);
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
			// 一定杀敌数后添加敌人
			int killedNum = GlobalData.Instance.GetKilledNum();
			if (killedNum == Constant.firstAdd) {
				int subIndex = Mathf.RoundToInt(Random.Range(-0.49f, birthPointNum - 1 + 0.49f));
				while (subIndex == index) {
					subIndex = Mathf.RoundToInt(Random.Range(-0.49f, birthPointNum - 1 + 0.49f));
				}
				string subArgs = "4-" + subIndex;
				Instance.StartCoroutine("AddEnemyCoroutine", subArgs);
				// 显示重生点标记
				BirthPointManager.ShowBirthPoint(subIndex, true);
			}
			if (killedNum == Constant.secondAdd) {
				int subIndex = Mathf.RoundToInt(Random.Range(-0.49f, birthPointNum - 1 + 0.49f));
				while (subIndex == index) {
					subIndex = Mathf.RoundToInt(Random.Range(-0.49f, birthPointNum - 1 + 0.49f));
				}
				string subArgs = "5-" + subIndex;
				Instance.StartCoroutine("AddEnemyCoroutine", subArgs);
				// 显示重生点标记
				BirthPointManager.ShowBirthPoint(subIndex, true);
			}
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

	IEnumerator AddEnemyCoroutine(string args)
	{
		yield return new WaitForSeconds(3);

		string[] sArray = args.Split('-');
		int enemyType = int.Parse(sArray[0]);
		int index = int.Parse(sArray[1]);
		I_InitData.AddEnemy(enemyType, index);
	}


	static void SetGlobalInfo(Transform dead)
	{
		BaseData deadData = Utils.GetBaseData(dead);
		if (!deadData.isPlayer) {
			GlobalData.Instance.AddScore(10);
			GlobalData.Instance.AddKilledEnemy(1);
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
	protected static void WeaponEnergyChangeDeal(Transform shooter, List<Transform> suffers, float damage)
	{
		float increaseRate = Constant.increaseRate;
		float decreaseRate = Constant.decreaseRate;
		ChangeEnergy(shooter, increaseRate * damage);
		foreach (Transform suffer in suffers) {
			if (suffer == PlayerData.Instance.target) {
				ChangeEnergy(suffer, decreaseRate * GlobalData.diffRate * damage);
			}
			else {
				ChangeEnergy(suffer, decreaseRate * damage);
			}
		}
	}

	// Player生命回复
	protected static void PlayerHealthRestoreDeal(Transform attacker, float damage)
	{
		Transform player = GetPlayer();
		if (attacker != player) return;
		BaseData data = Utils.GetBaseData(player);
		float rate = Utils.GetWeaponStrengthRestoreRate(data.curWeaponName, data.curWeaponLevel);
		if (rate <= 0) return;
		data.AddHealth(damage * rate);
		// 下发通知
		if (AddHealthEvent != null) {
			AddHealthEvent(player, damage * rate);
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
