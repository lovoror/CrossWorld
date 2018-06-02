using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitData : MonoBehaviour {
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

	public PlayerInitData playerData;
	public List<EnemyInitData> enemysData;

	void Awake()
	{
		InitPlayerData();
		InitEnemysData();
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
}
