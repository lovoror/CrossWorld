using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
	public class GamerInfo  // Player & Enemy Info
	{
		public GamerInfo(Transform gamer, int weapon, bool isPlayer, float health = 0, float maxHealth = 0, bool isDead = false)
		{
			this.gamer = gamer;
			this.isPlayer = isPlayer;
			this.isDead = isDead;
			this.health = health;
			this.maxHealth = maxHealth;
			this.curWeaponName = weapon;
		}

		public Transform gamer;
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

		public void SetDead(bool isDead)
		{
			this.isDead = isDead;
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
		public float GetWeaponEnergy()
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
		public float GetMaxWeaponEnergy()
		{
			int weaponLevel = GetWeaponLevel();
			if (Constant.MAX_WEAPON_ENERGY.ContainsKey(curWeaponName) &&
				Constant.MAX_WEAPON_ENERGY[curWeaponName].Count > weaponLevel) {
				List<float> maxList = Constant.MAX_WEAPON_ENERGY[curWeaponName];
				return maxList[weaponLevel] - maxList[weaponLevel - 1];
			}
			return -1;
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
	public static Dictionary<string, GamerInfo> GamerInfos = new Dictionary<string, GamerInfo>();
	public static Dictionary<int, float> BaseDamage = new Dictionary<int, float>() {
		{ (int)Constant.WEAPON_NAME.Knife, 40 },
		{ (int)Constant.WEAPON_NAME.M16, 30 },
	};

	public static void AddGamerInfo(Transform obj, int weapon, bool isPlayer = false, float health = 100, float maxHealth = 100, bool isDead = false)
	{
		GamerInfo gamerInfo = new GamerInfo(obj, weapon, isPlayer, health, maxHealth, isDead);
		GamerInfos.Add(obj.name, gamerInfo);
	}

	// Gamer受伤血量同步
	public static bool GamerHurt(string name, float damage)
	{
		if (damage < 0) return false;
		GamerInfo info = GamerInfos[name];
		if (info != null) {
			info.health -= damage;
			info.health = info.health < 0 ? 0 : info.health;
			if (info.health == 0) info.isDead = true;
		}
		return info.isDead;
	}

	public static void GamerDead(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return;
		info.SetDead(true);
	}

	/*------------------- 获取Gamer信息 -------------------*/
	public static float GetTotalEnergy(string name)
	{
		GamerInfo info = GamerInfos[name];
		if(info == null) return -1;
		return info.GetTotalEnergy();
	}
	public static void ChangeEnergy(string name, float delta)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return;
		info.ChangeEnergy(delta);
	}
	public static int GetWeaponLevel(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return -1;
		return info.GetWeaponLevel();
	}
	public static float GetWeaponEnergy(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return -1;
		return info.GetWeaponEnergy();
	}
	public static float GetMaxWeaponEnergy(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return -1;
		return info.GetMaxWeaponEnergy();
	}
	public static float GetHealth(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return -1;
		return info.health;
	}
	public static float GetMaxHealth(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return -1;
		return info.maxHealth;
	}
	public static int GetCurWeaponName(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return -1;
		return info.curWeaponName;
	}
	public static Transform GetGamerByName(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return null;
		return info.gamer;
	}
	public static bool IsGamerDead(string name)
	{
		GamerInfo info = GamerInfos[name];
		if (info == null) return true;
		return info.isDead;
	}
	/*------------------- 获取Gamer信息 -------------------*/


	/*------------------- 获取武器信息 -------------------*/
	public static float GetBaseDamage(int weaponName)
	{
		if (BaseDamage.ContainsKey(weaponName)) {
			return BaseDamage[weaponName];
		}
		return -1;
	}
	/*------------------- 获取武器信息 -------------------*/


}
