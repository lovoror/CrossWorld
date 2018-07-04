using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData
{
	public Transform target { get; set; }
	public Transform legTransform { get; set; }
	public float curHealth { get; set; }
	public float maxHealth { get; set; }
	public bool isDead { get; set; }
	public bool isPlayer { get; set; }
	public float curStrength { get; set; }
	public WeaponNameType curWeaponName = WeaponNameType.unknown;

	public float GetCurStrength()
	{
		return curStrength;
	}

	public void ChangeCurStrength(float delta)
	{
		curStrength += delta;
		curStrength = Mathf.Clamp(curStrength, 0, 100);
	}

	public WeaponType curWeaponType
	{
		get
		{
			return Utils.GetWeaponTypeByName(curWeaponName);
		}
	}
	public float curWeaponEnergy
	{
		get
		{
			return GetWeaponEnergy(curWeaponName);
		}
		set
		{
			SetCurWeaponEnergy(value);
		}
	}
	public float curWeaponLeftEnergy
	{
		get
		{
			return GetWeaponLeftEnergy(curWeaponName);
		}
	}
	public float curWeaponEnergyGap
	{
		get
		{
			return GetWeaponEnergyGap(curWeaponName);
		}
	}
	public float curWeaponLeftEnergyPercent
	{
		get
		{
			return curWeaponLeftEnergy / curWeaponEnergyGap * 100;
		}
	}
	public float curWeaponSpeed
	{
		get
		{
			return GetWeaponSpeed(curWeaponName);
		}
	}
	public int curWeaponLevel
	{
		get
		{
			return GetWeaponLevel(curWeaponName);
		}
	}
	public Transform curWeaponTransform
	{
		get
		{
			return GetWeaponTransformByName(curWeaponName);
		}
	}
	public Transform curBodyTransform
	{
		get
		{
			return GetBodyTransformByName(curWeaponName);
		}
	}
	public Transform[] weaponTransforms
	{
		get
		{
			return GetWeaponTransforms();
		}
	}
	public Transform[] bodyTransforms
	{
		get
		{
			return GetBodyTransforms();
		}
	}
	public Dictionary<WeaponNameType, Transform> d_Bodys = new Dictionary<WeaponNameType, Transform>();
	public Dictionary<WeaponNameType, Transform> d_Weapons = new Dictionary<WeaponNameType, Transform>();
	public Dictionary<WeaponNameType, int> d_LeftBullets = new Dictionary<WeaponNameType, int>();
	Dictionary<WeaponNameType, float> d_WeaponEnergy = new Dictionary<WeaponNameType, float>();
	Dictionary<WeaponNameType, float> d_WeaponLeftEnergy = new Dictionary<WeaponNameType, float>();
	Dictionary<WeaponNameType, float> d_WeaponEnergyGap = new Dictionary<WeaponNameType, float>();
	Dictionary<WeaponNameType, float> d_WeaponSpeed = new Dictionary<WeaponNameType, float>();
	Dictionary<WeaponNameType, int> d_WeaponLevel = new Dictionary<WeaponNameType, int>();

	protected void Init()
	{
		isDead = false;
		curStrength = 100;
		foreach (WeaponNameType weaponName in Enum.GetValues(typeof(WeaponNameType))) {
			if (weaponName != WeaponNameType.unknown) {
				d_WeaponEnergy.Add(weaponName, 0);
				d_WeaponLeftEnergy.Add(weaponName, 0);
				d_WeaponEnergyGap.Add(weaponName, 99999);
				d_WeaponSpeed.Add(weaponName, 1);
				d_WeaponLevel.Add(weaponName, 1);
				WeaponType weaponType = Utils.GetWeaponTypeByName(weaponName);
				if (weaponType == WeaponType.autoDistant || weaponType == WeaponType.singleLoader) {
					int count = 0;
					if (Constant.MagazineSize.ContainsKey(weaponName)) {
						count = Constant.MagazineSize[weaponName];
					}
					d_LeftBullets.Add(weaponName, count);
				}
			}
			else {
				d_WeaponEnergy.Add(weaponName, -1);
				d_WeaponLeftEnergy.Add(weaponName, -1);
				d_WeaponEnergyGap.Add(weaponName, -1);
				d_WeaponSpeed.Add(weaponName, -1);
				d_WeaponLevel.Add(weaponName, -1);
				d_LeftBullets.Add(weaponName, -1);
			}
		}
	}

	public void AddHealth(float delta)
	{
		if (delta <= 0) return;
		curHealth = Mathf.Clamp(curHealth + delta, 0, maxHealth);
	}

	public float GetWeaponEnergy(WeaponNameType weaponName)
	{
		float energy = -1;
		if (d_WeaponEnergy.ContainsKey(weaponName)) {
			energy = d_WeaponEnergy[weaponName];
		}
		return energy;
	}
	public void SetWeaponEnergy(float energy, WeaponNameType weaponName)
	{
		energy = energy < 0 ? 0 : energy;
		float maxEnergy = -1;
		if (Constant.MAX_WEAPON_ENERGY.ContainsKey(weaponName)) {
			List<float> energyList = Constant.MAX_WEAPON_ENERGY[weaponName];
			maxEnergy = energyList[energyList.Count - 1];
		}
		if (maxEnergy < 0) return;
		energy = energy < maxEnergy ? energy : maxEnergy;
		if (d_WeaponEnergy.ContainsKey(weaponName)) {
			d_WeaponEnergy[weaponName] = energy;
		}
		else if (weaponName != WeaponNameType.unknown) {
			d_WeaponEnergy.Add(weaponName, energy);
		}
		SetWeaponLevel(weaponName);
		SetWeaponLeftEnergy(weaponName);
		SetWeaponEnergyGap(weaponName);
		SetWeaponSpeed(weaponName);
	}
	protected void SetCurWeaponEnergy(float energy)
	{
		SetWeaponEnergy(energy, curWeaponName);
	}
	public void AddWeaponEnergy(float delta, WeaponNameType weaponName)
	{
		float curEnergy = GetWeaponEnergy(weaponName);
		curEnergy = curEnergy < 0 ? 0 : curEnergy;
		SetWeaponEnergy(delta + curEnergy, weaponName);
	}
	public void AddCurWeaponEnergy(float delta)
	{
		AddWeaponEnergy(delta, curWeaponName);
	}

	protected void SetWeaponLevel(WeaponNameType weaponName)
	{
		float energy = GetWeaponEnergy(weaponName);
		if (energy >= 0) {
			int curLevel = 1;
			if (d_WeaponEnergy.ContainsKey(weaponName) && Constant.MAX_WEAPON_ENERGY.ContainsKey(weaponName)) {
				List<float> energyList = Constant.MAX_WEAPON_ENERGY[weaponName];
				if (energyList != null) {
					for (int i = 0; i < energyList.Count; ++i) {
						if (i == energyList.Count - 1) {
							curLevel = i;
							break;
						}
						if (energy < energyList[i]) {
							curLevel = i;
							break;
						}
					}
				}
			}
			if (d_WeaponLevel.ContainsKey(weaponName)) {
				d_WeaponLevel[weaponName] = curLevel;
			}
			else {
				d_WeaponLevel.Add(weaponName, curLevel);
			}
		}
	}

	public int GetWeaponLevel(WeaponNameType weaponName)
	{
		int level = -1;
		if (d_WeaponLevel.ContainsKey(weaponName)) {
			level = d_WeaponLevel[weaponName];
		}
		return level;
	}
	// 必须在SetWeaponLevel后
	protected void SetWeaponLeftEnergy(WeaponNameType weaponName)
	{
		int weaponLevel = GetWeaponLevel(weaponName);
		if (weaponLevel >= 0) {
			float totalEnergy = GetWeaponEnergy(weaponName);
			List<float> energyList = Constant.MAX_WEAPON_ENERGY[weaponName];
			float leftEnergy = totalEnergy - energyList[weaponLevel - 1];
			if (d_WeaponLeftEnergy.ContainsKey(weaponName)) {
				d_WeaponLeftEnergy[weaponName] = leftEnergy;
			}
			else {
				d_WeaponLeftEnergy.Add(weaponName, leftEnergy);
			}
		}
	}
	public float GetWeaponLeftEnergy(WeaponNameType weaponName)
	{
		float leftEnergy = -1;
		if (d_WeaponLeftEnergy.ContainsKey(weaponName)) {
			leftEnergy = d_WeaponLeftEnergy[weaponName];
		}
		return leftEnergy;
	}
	// 必须在SetWeaponLevel后
	// 获得武器当前等级的升级能量值
	protected void SetWeaponEnergyGap(WeaponNameType weaponName)
	{
		int curLevel = GetWeaponLevel(weaponName);
		if (curLevel > 0) {
			List<float> energyList = Constant.MAX_WEAPON_ENERGY[weaponName];
			float gap = energyList[curLevel] - energyList[curLevel - 1];
			if (d_WeaponEnergyGap.ContainsKey(weaponName)) {
				d_WeaponEnergyGap[weaponName] = gap;
			}
			else {
				d_WeaponEnergyGap.Add(weaponName, gap);
			}
		}
	}
	public float GetWeaponEnergyGap(WeaponNameType weaponName)
	{
		float gap = -1;
		if (d_WeaponEnergyGap.ContainsKey(weaponName)) {
			gap = d_WeaponEnergyGap[weaponName];
		}
		return gap;
	}
	protected void SetWeaponSpeed(WeaponNameType weaponName)
	{
		float speed = -1;
		int level = GetWeaponLevel(weaponName);
		if (level > 0) {
			if (Constant.WEAPON_SPEED_RATE.ContainsKey(weaponName) &&
				Constant.WEAPON_SPEED_RATE[weaponName].Count >= level) {
				speed = Constant.WEAPON_SPEED_RATE[weaponName][level - 1];
			}
		}
		if (speed >= 0) {
			if (d_WeaponSpeed.ContainsKey(weaponName)) {
				d_WeaponSpeed[weaponName] = speed;
			}
			else {
				d_WeaponSpeed.Add(weaponName, speed);
			}
		}
	}

	public float GetWeaponSpeed(WeaponNameType weaponName)
	{
		float speed = -1;
		if (d_WeaponSpeed.ContainsKey(weaponName)) {
			speed = d_WeaponSpeed[weaponName];
		}
		return speed;
	}

	public void AddToWeapons(WeaponNameType weaponName, Transform weapon)
	{
		if (d_Weapons.ContainsKey(weaponName)) {
			d_Weapons[weaponName] = weapon;
		}
		else {
			d_Weapons.Add(weaponName, weapon);
		}
	}

	public Transform GetWeaponTransformByName(WeaponNameType weaponName)
	{
		if (d_Weapons.ContainsKey(weaponName)) {
			return d_Weapons[weaponName];
		}
		return null;
	}
	protected Transform[] GetWeaponTransforms()
	{
		Transform[] transforms = new Transform[d_Weapons.Count];
		d_Weapons.Values.CopyTo(transforms, 0);
		return transforms;
	}

	public void ReloadWeapon(WeaponNameType weaponName = WeaponNameType.unknown)
	{
		weaponName = weaponName == WeaponNameType.unknown ? curWeaponName : weaponName;
		int count = -1;
		if (Constant.MagazineSize.ContainsKey(weaponName)) {
			count = Constant.MagazineSize[weaponName];
		}
		if (count < 0) return;
		if (d_LeftBullets.ContainsKey(weaponName)) {
			d_LeftBullets[weaponName] = count;
		}
		else {
			d_LeftBullets.Add(weaponName, count);
		}
	}

	public int GetLeftBulletsByName(WeaponNameType weaponName)
	{
		int count = -1;
		if (d_LeftBullets.ContainsKey(weaponName)) {
			count = d_LeftBullets[weaponName];
		}
		return count;
	}

	public int GetCurLeftBullets()
	{
		return GetLeftBulletsByName(curWeaponName);
	}

	public bool ShootOneBullet()
	{
		if (d_LeftBullets.ContainsKey(curWeaponName)) {
			int count = d_LeftBullets[curWeaponName];
			if (count > 0) {
				d_LeftBullets[curWeaponName] = count - 1;
				return true;
			}
		}
		return false;
	}

	public void AddToBodys(WeaponNameType weaponName, Transform body)
	{
		if (d_Bodys.ContainsKey(weaponName)) {
			d_Bodys[weaponName] = body;
		}
		else {
			d_Bodys.Add(weaponName, body);
		}
	}

	public Transform GetBodyTransformByName(WeaponNameType weaponName)
	{
		if (d_Bodys.ContainsKey(weaponName)) {
			return d_Bodys[weaponName];
		}
		return null;
	}
	protected Transform[] GetBodyTransforms()
	{
		Transform[] transforms = new Transform[d_Bodys.Count];
		d_Bodys.Values.CopyTo(transforms, 0);
		return transforms;
	}

	public bool ChangeCurHealth(float delta)
	{
		curHealth += delta;
		curHealth = curHealth >= 0 ? curHealth : 0;
		curHealth = curHealth <= maxHealth ? curHealth : maxHealth;
		return curHealth <= 0;
	}
}
