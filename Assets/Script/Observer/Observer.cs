using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
	
	protected class GamerInfo  // Player & Enemy Info
	{
		public float health;
		public float maxHealth;
		public string curWeapon;
		public bool isPlayer;
		public bool isDead;
		public GamerInfo(string weapon, bool isPlayer, float health = 0, float maxHealth = 0, bool isDead = false)
		{
			this.isPlayer = isPlayer;
			this.isDead = isDead;
			this.health = health;
			this.maxHealth = maxHealth;
			this.curWeapon = weapon;
		}
	}

	protected static Dictionary<string, GamerInfo> gamerInfos = new Dictionary<string, GamerInfo>();
	protected static Dictionary<string, float> baseDamage = new Dictionary<string, float>() {
		{ "Knife", 40 },
		{ "M16", 30 },
	};


	protected void Start () {
		InitGamerInfo();
	}
	
	private static bool isInited = false; // 信息是否已经初始化
	// 初始化Player和Enemys的信息
	void InitGamerInfo () {
		if (isInited) return;
		// 初始化Player信息
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		PlayerDataManager playerData = player.transform.GetComponent<PlayerDataManager>();
		GamerInfo playerInfo = new GamerInfo(playerData.weaponName, true, playerData.health, playerData.maxHealth);
		gamerInfos.Add(playerData.name, playerInfo);

		// 初始化Enemys信息
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemys) {
			EnemyDataManager enemyData = enemy.transform.GetComponent<EnemyDataManager>();
			GamerInfo enemyInfo = new GamerInfo(enemyData.weaponName, false, enemyData.health, enemyData.maxHealth);
			gamerInfos.Add(enemyData.name, enemyInfo);
		}
		isInited = true;
	}

	// Gamer受伤血量同步
	protected static bool GamerHurt(string name, float damage)
	{
		if (damage < 0) return false;
		GamerInfo info = gamerInfos[name];
		if (info != null) {
			info.health -= damage;
			info.health = info.health < 0 ? 0 : info.health;
			if (info.health == 0) info.isDead = true;
		}
		return info.isDead;
	}
}
