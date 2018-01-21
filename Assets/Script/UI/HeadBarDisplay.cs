using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBarDisplay : MonoBehaviour {

	public Transform headBar;
	public Vector3 offset = new Vector3(0, 0, 6);

	[HideInInspector]
	public static HeadBarDisplay instance;

	public class Bar
	{
		public Transform bar;   // bar对象
		public bool isVisible = true;
		SpriteRenderer healthBar;
		Vector3 healthScale;
		SpriteRenderer healthBarOutline;
		SpriteRenderer weaponEnergyBar;   // 武器能量槽
		Vector3 energyScale;

		public Bar(Transform bar)
		{
			this.bar = bar;
			healthBar = bar.Find("HealthBar").GetComponent<SpriteRenderer>();
			healthScale = healthBar.transform.localScale;
			healthBarOutline = bar.Find("HealthOutline").GetComponent<SpriteRenderer>();
			weaponEnergyBar = bar.Find("WeaponEngrgyBar").GetComponent<SpriteRenderer>();
			energyScale = weaponEnergyBar.transform.localScale;
		}

		public void ShowHealthBar(float health, float maxHealth)
		{
			if (health < 0 || maxHealth < 0) {
				Debug.LogError("health or maxHealth less than 0");
				return;
			}
			// Set the health bar's colour to proportion of the way between green and red based on the player's health.
			healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health / maxHealth);
			// Set the scale of the health bar to be proportional to the player's health.
			healthBar.transform.localScale = new Vector3(healthScale.x * health / maxHealth, healthScale.y, healthScale.z);
		}

		public void ShowWeaponEnergyBar(int weaponLevel, float weaponEnergy, float maxWeaponEnergy)
		{
			weaponEnergyBar.material.color = Constant.WEAPON_COLORS[weaponLevel];
			if (maxWeaponEnergy > 0) {
				weaponEnergyBar.transform.localScale = new Vector3(energyScale.x * weaponEnergy / maxWeaponEnergy, energyScale.y, energyScale.z);
			}
		}

		public void SetBarVisible(bool show)
		{
			isVisible = show;
			healthBar.enabled = show;
			weaponEnergyBar.enabled = show;
			healthBarOutline.enabled = show;
		}

		public void SetPosition(Vector3? pos)
		{
			if (pos != null) {
				bar.position = pos.Value;
			}
		}
	}
	public static Dictionary<Transform, Bar> barPool = new Dictionary<Transform, Bar>();

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
	}

	void Update()
	{
		foreach (var bar in barPool) {
			if (bar.Value.isVisible == false) continue;
			string gamerName = bar.Key.name;
			if (GameData.IsGamerDead(gamerName)) {
				bar.Value.SetBarVisible(false);
				continue;
			}
			// Bar跟踪Gamer
			bar.Value.SetPosition(bar.Key.position + offset);
			// HealthBar Display
			bar.Value.ShowHealthBar(GameData.GetHealth(gamerName), GameData.GetMaxHealth(gamerName));
			// EnergyBar Display
			bar.Value.ShowWeaponEnergyBar(GameData.GetWeaponLevel(gamerName), GameData.GetWeaponEnergy(gamerName), GameData.GetMaxWeaponEnergy(gamerName));
		}
	}

	public void InitHeadBars()
	{
		foreach (var info in GameData.GamerInfos) {
			Vector3 basePos = info.Value.gamer.position;
			//Transform I_Bar = Instantiate(headBar, basePos + offset, Quaternion.EulerAngles(90, 0, 0));  // zpf modify
			Transform I_Bar = Instantiate(headBar, basePos + offset, Quaternion.Euler(90, 0, 0));
			I_Bar.parent = transform;
			Bar barInfo = new Bar(I_Bar);
			barPool.Add(info.Value.gamer, barInfo);
		}
	}

	public static void StageEnd()
	{
		barPool.Clear();
		instance = null;
	}
}
