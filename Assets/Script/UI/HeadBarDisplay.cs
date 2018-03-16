using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBarDisplay : MonoBehaviour {

	public Transform headBar;
	public Vector3 offset = new Vector3(0, 0, 6);
	[HideInInspector]
	protected static HeadBarDisplay m_instance = null;

	public class Bar
	{
		public Transform bar { get; set; }   // bar对象
		public Transform target { get; set; }  // 跟随的对象
		public bool isVisible { get; set; }
		Vector3 offset;
		SpriteRenderer healthBar { get; set; }
		Vector3 healthScale { get; set; }
		SpriteRenderer healthBarOutline { get; set; }
		SpriteRenderer weaponEnergyBar { get; set; }   // 武器能量槽
		Vector3 energyScale { get; set; }
		BaseData tarData { get; set; }
		float health
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
		}

		public void Update()
		{
			if (!isVisible) return;
			if (health == 0) {
				SetBarVisible(false);
				return;
			}
			UpdatePosition();
			UpdateHealthBar();
			UpdateWeaponEnergyBar();
		}

		void UpdatePosition()
		{
			bar.position = target.position + offset;
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

		public void SetBarVisible(bool show)
		{
			isVisible = show;
			healthBar.enabled = show;
			weaponEnergyBar.enabled = show;
			healthBarOutline.enabled = show;
		}
	}
	
	public static Dictionary<Transform, Bar> d_BarPool = new Dictionary<Transform, Bar>();

	void Awake()
	{

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
		AddToBarPool(player, playerBar);

		foreach (Transform enemy in EnemysData.Instance.enemyTransforms) {
			Bar enemyBar = new Bar(enemy, headBar, offset);
			AddToBarPool(enemy, enemyBar);
		}
	}

	void AddToBarPool(Transform key, Bar bar)
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
		foreach (Bar bar in d_BarPool.Values) {
			bar.Update();
		}
	}

	public static void StageEnd()
	{
		d_BarPool.Clear();
		m_instance = null;
	}
}
