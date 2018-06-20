using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitData : MonoBehaviour {
	public delegate void NewEnemyEventHandler(Transform enemy);
	public static event NewEnemyEventHandler NewEnemyEvent;

	[System.Serializable]
	public class SerializableDictionary
	{
		public WeaponNameType weaponName;
		public Transform body;
		public Transform weapon;
	}

	[System.Serializable]
	public class PlayerInitData
	{
		public Transform player;
		public float playerMaxHealth = 0;
		public List<SerializableDictionary> playerWeaponInfos;
	}

	[System.Serializable]
	public class EnemyInitData
	{
		public Transform enemy;
		public float enemyMaxHealth = 0;
		public List<SerializableDictionary> enemyWeaponInfos;
	}

	public Transform enemyPrefab;
	public PlayerInitData playerData;
	public List<EnemyInitData> enemysData;
	public Transform birthPoint;
	public List<Transform> birthPoints;

	void Awake()
	{
		InitPlayerData();
		InitEnemysData();
		InitGlobalData();
		foreach (Transform point in birthPoint) {
			birthPoints.Add(point);
		}
		Random.InitState((int)Time.time);
	}

	void InitGlobalData()
	{
		GlobalData.Instance.Init();
	}

	void InitPlayerData()
	{
		Dictionary<WeaponNameType, Transform> d_PlayerWeapons = new Dictionary<WeaponNameType, Transform>();
		Dictionary<WeaponNameType, Transform> d_PlayerBodys = new Dictionary<WeaponNameType, Transform>();
		WeaponNameType curWeaponName = WeaponNameType.unknown;
		if (playerData.playerWeaponInfos.Count > 0) {
			foreach (var info in playerData.playerWeaponInfos) {
				d_PlayerWeapons.Add(info.weaponName, info.weapon);
				d_PlayerBodys.Add(info.weaponName, info.body);
				if (info.weapon.gameObject.activeSelf) curWeaponName = info.weaponName;
			}
		}
		PlayerData.Instance.Init(playerData.player, curWeaponName, playerData.playerMaxHealth, d_PlayerWeapons, d_PlayerBodys);
	}

	void InitEnemysData()
	{
		foreach (var enemyData in enemysData) {
			Dictionary<WeaponNameType, Transform> d_EnemyWeapons = new Dictionary<WeaponNameType, Transform>();
			Dictionary<WeaponNameType, Transform> d_EnemyBodys = new Dictionary<WeaponNameType, Transform>();
			WeaponNameType curWeaponName = WeaponNameType.unknown;
			foreach (var info in enemyData.enemyWeaponInfos) {
				d_EnemyWeapons.Add(info.weaponName, info.weapon);
				d_EnemyBodys.Add(info.weaponName, info.body);
				if (info.weapon.gameObject.activeSelf) curWeaponName = info.weaponName;
			}
			EnemysData.Instance.AddEnemyData(enemyData.enemy, curWeaponName, enemyData.enemyMaxHealth, d_EnemyWeapons, d_EnemyBodys);
		}
		EnemysData.Instance.OnInitEnd();
	}

	public void NewEnemy(int enemyType) {
		Transform instance = Instantiate(enemyPrefab.gameObject).transform;
		instance.parent = GameObject.Find("Enemys").transform;
		instance.position = birthPoints[Mathf.RoundToInt(Random.Range(0, birthPoints.Count))].position;
		instance.name = "Colombian-" + enemyType;
		Dictionary<WeaponNameType, Transform> d_EnemyWeapons = new Dictionary<WeaponNameType, Transform>();
		Dictionary<WeaponNameType, Transform> d_EnemyBodys = new Dictionary<WeaponNameType, Transform>();
		Transform body = instance.Find("Bodys").Find("Knife");
		Transform knife = instance.Find("Weapons").Find("Knife");
		d_EnemyWeapons.Add(WeaponNameType.Knife, knife);
		d_EnemyBodys.Add(WeaponNameType.Knife, body);
		instance.GetComponent<BehaviorTreeUpdate>().patrolPoints = GameObject.Find("PatrolPoint-" + enemyType);
		EnemysData.Instance.AddEnemyData(instance, WeaponNameType.Knife, 150, d_EnemyWeapons, d_EnemyBodys);
		HeadBarDisplay.AddEnemyBar(instance);
		if (NewEnemyEvent != null) {
			NewEnemyEvent(instance);
		}
	}
}
