using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HeadBarDisplay : MonoBehaviour {

	public Transform headBar;
	public Vector3 offset = new Vector3(0, 0, 6);

	static Transform s_HeadBar;
	static Vector3 s_offset;

	public class Bar
	{
		public Transform bar { get; set; }   // bar对象
		public Transform target { get; set; }  // 跟随的对象
		public bool isVisible { get; set; }
		bool isStrengthBarVisible { get; set; }
		Vector3 offset;
		SpriteRenderer healthBar { get; set; }
		Vector3 healthScale { get; set; }
		SpriteRenderer healthBarOutline { get; set; }
		SpriteRenderer weaponEnergyBar { get; set; }   // 武器能量槽
		Vector3 energyScale { get; set; }
		SpriteRenderer strengthBar { get; set; }   // 武器能量槽
		Vector3 strengthScale { get; set; }
		BaseData tarData { get; set; }
		public float health
		{
			get
			{
				return tarData.curHealth;
			}
		}
		float maxHealth { get; set; }
		int curWeaponLevel
		{
			get
			{
				return tarData.curWeaponLevel;
			}
		}
		float energyPercent
		{
			get
			{
				return tarData.curWeaponLeftEnergyPercent;
			}
		}

		float strength
		{
			get
			{
				return tarData.curStrength;
			}
		}

		public Bar(Transform target, Transform headBar, Vector3 offset)
		{
			this.bar = Instantiate(headBar, -1000 * Vector3.right, Quaternion.Euler(90, 0, 0));
			this.target = target;
			this.offset = offset;
			tarData = Utils.GetBaseData(target);
			maxHealth = tarData.maxHealth;
			isVisible = true;
			healthBar = bar.Find("HealthBar").GetComponent<SpriteRenderer>();
			healthScale = healthBar.transform.localScale;
			healthBarOutline = bar.Find("HealthOutline").GetComponent<SpriteRenderer>();
			weaponEnergyBar = bar.Find("WeaponEngrgyBar").GetComponent<SpriteRenderer>();
			energyScale = weaponEnergyBar.transform.localScale;
			strengthBar = bar.Find("StrengthBar").GetComponent<SpriteRenderer>();
			strengthScale = strengthBar.transform.localScale;
		}

		public void Close()
		{
			Destroy(bar.gameObject);
		}

		float deltaTime = 0.1f;
		float last_update_time = 0;

		public void Update()
		{
			UpdatePosition();
			if (Time.time - last_update_time >= deltaTime) {
				last_update_time = Time.time;
				UpdateHealthBar();
				UpdateWeaponEnergyBar();
				UpdateStrengthBar();
			}
		}

		void UpdatePosition()
		{
			bar.position = target.position + offset;
		}

		void UpdateStrengthBar()
		{
			if (!isStrengthBarVisible) return;
			strengthBar.transform.localScale = new Vector3(strengthScale.x * strength / 100f, strengthScale.y, strengthScale.z);
			if (strength < Constant.minRollStrength * GlobalData.Instance.diffRate) {
				strengthBar.material.color = Color.red;
			}
			else {
				strengthBar.material.color = Color.yellow;
			}
		}

		void UpdateHealthBar()
		{
			if (health < 0 || maxHealth < 0) {
				Debug.LogError("health or maxHealth less than 0");
				return;
			}
			healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health / maxHealth);
			healthBar.transform.localScale = new Vector3(healthScale.x * health / maxHealth, healthScale.y, healthScale.z);
		}

		void UpdateWeaponEnergyBar()
		{
			weaponEnergyBar.material.color = Constant.WEAPON_COLORS[curWeaponLevel];
			weaponEnergyBar.transform.localScale = new Vector3(energyScale.x * energyPercent / 100, energyScale.y, energyScale.z);
		}

		public void SetStrengthBarVisible(bool visible)
		{
			isStrengthBarVisible = visible;
			if (visible) {
				healthScale = new Vector3(1, 0.671f, 1);
				healthBar.transform.localScale = healthScale;
				healthBar.transform.localPosition = new Vector3(-0.471f, 0, 0);
			}
			else {
				healthScale = new Vector3(1, 0.88f, 1);
				healthBar.transform.localScale = healthScale;
				healthBar.transform.localPosition = new Vector3(-0.471f, 0.019f, 0);
			}
			strengthBar.enabled = visible;
		}

		public void SetBarVisible(bool show)
		{
			isVisible = show;
			healthBar.enabled = show;
			strengthBar.enabled = show;
			weaponEnergyBar.enabled = show;
			healthBarOutline.enabled = show;
		}
	}
	
	public static Dictionary<Transform, Bar> d_BarPool = new Dictionary<Transform, Bar>();

	void Awake()
	{
		s_HeadBar = headBar;
		s_offset = offset;
	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}

	void Start()
	{
		Transform player = PlayerData.Instance.target;
		Bar playerBar = new Bar(player, headBar, offset);
		playerBar.SetStrengthBarVisible(true);
		AddToBarPool(player, playerBar);

		foreach (Transform enemy in EnemysData.Instance.enemyTransforms) {
			Bar enemyBar = new Bar(enemy, headBar, offset);
			AddToBarPool(enemy, enemyBar);
		}
	}

	static void AddToBarPool(Transform key, Bar bar)
	{
		if (d_BarPool.ContainsKey(key)) {
			d_BarPool[key] = bar;
		}
		else {
			d_BarPool.Add(key, bar);
		}
	}

	void Update()
	{
		List<Transform> gcBar = new List<Transform>();
		foreach (var kv in d_BarPool) {
			Bar bar = kv.Value;
			Transform key = kv.Key;
			if (bar.health > 0) {
				bar.Update();
			}
			else {
				bar.Close();
				gcBar.Add(key);
			}
		}
		// 回收Bar
		foreach (Transform t in gcBar) {
			d_BarPool.Remove(t);
		}
	}

	public static void AddEnemyBar(Transform enemy)
	{
		Bar enemyBar = new Bar(enemy, s_HeadBar, s_offset);
		AddToBarPool(enemy, enemyBar);
	}

	public static void StageEnd()
	{
		d_BarPool.Clear();
		s_HeadBar = null;
		s_offset = Vector3.zero;
	}
}
