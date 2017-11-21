using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{

	protected class GamerInfo  // Player & Enemy Info
	{
		public GamerInfo(int weapon, bool isPlayer, float health = 0, float maxHealth = 0, bool isDead = false)
		{
			this.isPlayer = isPlayer;
			this.isDead = isDead;
			this.health = health;
			this.maxHealth = maxHealth;
			this.curWeaponName = weapon;
		}

		public float health;
		public float maxHealth;
		public int curWeaponName;
		public bool isPlayer;
		public bool isDead;

		/*------------- 武器能量信息 -------------*/
		/* 需要增加拥有能量槽可升级的武器，还需要改变以下变量：
		 * Constant.MAX_WEAPON_ENERGY / Constant.WEAPON_SPEED_RATE /  
		 */
		private Dictionary<int, float> weaponEnergyInfos = new Dictionary<int, float>() {
			{(int)Constant.WEAPON_NAME.M16, 0},
			{(int)Constant.WEAPON_NAME.Knife, 0},
		};

		protected void OnEnable()
		{

		}

		protected void OnDisable()
		{

		}

		public void ChangeCurWeapon(int weaponName)
		{
			curWeaponName = weaponName;
		}
		public void ChangeEnergy(float delta)
		{
			if (!weaponEnergyInfos.ContainsKey(curWeaponName)) return;
			float preEnergy = weaponEnergyInfos[curWeaponName];
			float maxEnergy = Constant.MAX_WEAPON_ENERGY[curWeaponName][Constant.MAX_WEAPON_ENERGY[curWeaponName].Count - 1];
			preEnergy += delta;
			preEnergy = preEnergy < 0 ? 0 : preEnergy;
			preEnergy = preEnergy > maxEnergy ? maxEnergy : preEnergy;
			weaponEnergyInfos[curWeaponName] = preEnergy;
		}
		public float GetTotalEnergy()
		{
			if (!weaponEnergyInfos.ContainsKey(curWeaponName)) return -1;
			if (weaponEnergyInfos.ContainsKey(curWeaponName)) {
				return weaponEnergyInfos[curWeaponName];
			}
			return -1;
		}
		public float GetEnergy()
		{
			float leftEnergy = weaponEnergyInfos[curWeaponName];
			List<float> energyList = Constant.MAX_WEAPON_ENERGY[curWeaponName];
			if (energyList != null) {
				for (int i = 0; i <= energyList.Count; ++i) {
					if (i == energyList.Count) {
						leftEnergy = energyList[i - 1] - energyList[i - 2];
						break;
					}
					if (leftEnergy < energyList[i]) {
						leftEnergy -= energyList[i - 1];
						break;
					}
				}
			}
			return leftEnergy;
		}
		public int GetWeaponLevel()
		{
			int level = 1;
			float leftEnergy = weaponEnergyInfos[curWeaponName];
			List<float> energyList = Constant.MAX_WEAPON_ENERGY[curWeaponName];
			if (energyList != null) {
				for (int i = 0; i < energyList.Count; ++i) {
					if (i == energyList.Count - 1) {
						level = i;
						break;
					}
					if (leftEnergy < energyList[i]) {
						level = i;
						break;
					}
				}
			}
			return level;
		}
	}

		/*------------- 武器能量信息 -------------*/
	protected static Dictionary<string, GamerInfo> gamerInfos = new Dictionary<string, GamerInfo>();
	protected static Dictionary<int, float> baseDamage = new Dictionary<int, float>() {
		{ (int)Constant.WEAPON_NAME.Knife, 40 },
		{ (int)Constant.WEAPON_NAME.M16, 30 },
	};

	protected void Start () {
		InitGamerInfo();
	}
	
	private static bool isInited = false; // 信息是否已经初始化
	// 初始化Player和Enemys的信息
	static void InitGamerInfo () {
		if (isInited) return;
		// 初始化Gamers信息
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		List<GameObject> gamers = new List<GameObject>(enemys);
		gamers.Add(player);
		foreach (GameObject gamer in gamers) {
			DataManager gamerData = gamer.transform.GetComponent<DataManager>();
			GamerInfo enemyInfo = new GamerInfo(gamerData.curWeaponName, false, gamerData.health, gamerData.maxHealth);
			gamerInfos.Add(gamerData.name, enemyInfo);
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
