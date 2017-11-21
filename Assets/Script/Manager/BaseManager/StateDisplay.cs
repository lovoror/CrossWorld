using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDisplay : MonoBehaviour {

	protected SpriteRenderer healthBar;
	protected SpriteRenderer healthBarOutline;
	protected SpriteRenderer weaponEnergyBar;   // 武器能量槽
	protected Manager I_Manager;
	protected float health;
	protected float maxHealth;
	protected float weaponEnergy;
	protected float maxWeaponEnergy;
	protected int weaponLevel;
	protected Vector3 healthScale;
	protected Vector3 energyScale;
	protected float lastHealth = -1; // 上次显示的体力
	protected float lastWeaponEnergy = -1; // 上次显示的武器能量
	protected float lastWeaponLevel = -1;

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
	}

	protected void Start ()
	{
		Transform bar = Utils.FindChildRecursively(transform, "HealthBar");
		Transform barOutline = Utils.FindChildRecursively(transform, "HealthOutline");
		Transform energyBar = Utils.FindChildRecursively(transform, "WeaponEngrgyBar");
		if (bar) {
			healthBar = bar.GetComponent<SpriteRenderer>();
			healthScale = healthBar.transform.localScale;
		
		}
		if (energyBar) {
			weaponEnergyBar = energyBar.GetComponent<SpriteRenderer>();
			energyScale = weaponEnergyBar.transform.localScale;
			weaponEnergyBar.transform.localScale = new Vector3(0, energyScale.y, energyScale.z);
		}
		if (barOutline) {
			healthBarOutline = barOutline.GetComponent<SpriteRenderer>();
		}
		maxHealth = I_Manager.I_DataManager.GetMaxHealth();
		maxWeaponEnergy = I_Manager.I_DataManager.GetMaxWeaponEnergy();
		weaponLevel = I_Manager.I_DataManager.GetWeaponLevel();
	}
	
	protected void Update ()
	{
		health = I_Manager.I_DataManager.GetHealth();
		if (health != lastHealth) {
			lastHealth = health;
			ShowHealthBar();
		}

		// WeaponEnergy
		weaponLevel = I_Manager.I_DataManager.GetWeaponLevel();
		weaponEnergy = I_Manager.I_DataManager.GetWeaponEnergy();
		if (weaponLevel != lastWeaponLevel || weaponEnergy != lastWeaponEnergy) {
			lastWeaponLevel = weaponLevel;
			lastWeaponEnergy = weaponEnergy;
			ShowWeaponEnergyBar();
		}
	}

	void ShowWeaponEnergyBar()
	{
		weaponEnergyBar.material.color = Constant.WEAPON_COLORS[weaponLevel];
		maxWeaponEnergy = I_Manager.I_DataManager.GetMaxWeaponEnergy();
		if (maxWeaponEnergy > 0) {
			weaponEnergyBar.transform.localScale = new Vector3(energyScale.x * weaponEnergy / maxWeaponEnergy, energyScale.y, energyScale.z);
		}
		weaponEnergyBar.enabled = health > 0;
	}

	void ShowHealthBar()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health / maxHealth);
		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health / maxHealth, healthScale.y, healthScale.z);
		// 死亡后隐藏
		healthBarOutline.enabled = health > 0;
		healthBar.enabled = health > 0;
	}
}
