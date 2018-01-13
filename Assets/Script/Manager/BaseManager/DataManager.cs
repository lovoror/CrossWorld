using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

	protected Manager I_Manager;
	protected Transform self;

	public bool isDead = false;
	public int killedWeaponName = -1;
	public float health = 100;
	public float maxHealth = 100;
	public int curWeaponName;
	public float attackSpeedRate = 1;

	public int weaponLevel = 1;
	public float weaponEnergy = 0;

	public delegate void AttackSpeedChangeEventHandler(float speedRate);
	public event AttackSpeedChangeEventHandler AttackSpeedChangeEvent;

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
	}

	protected void Start () {
		curWeaponName = I_Manager.I_WeaponManager.weaponName;
		self = transform;
	}
	
	void Update () {
		
	}

	protected void OnEnable()
	{
		I_Manager.I_Messenger.WeaponEnergyChangeNotifyEvent += new Messenger.WeaponEnergyChangeNotifyEventHandler(WeaponEnergyChangeNotifyEventFunc);
	}

	protected void OnDisable()
	{
		I_Manager.I_Messenger.WeaponEnergyChangeNotifyEvent -= WeaponEnergyChangeNotifyEventFunc;

	}


	public void ChangeHealth(float delta)
	{
		health += delta;
		health = health < 0 ? 0 : health;
		health = health > maxHealth ? maxHealth : health;
	}

	public void SetHealth(float curHealth)
	{
		health = curHealth;
	}

	public float GetHealth()
	{
		return health;
	}

	public float GetMaxHealth()
	{
		return maxHealth;
	}

	/*------------- WeaponEnergyChangeEvent ---------------*/
	void WeaponEnergyChangeNotifyEventFunc(Transform target, int level, float energy)
	{
		weaponEnergy = energy;
		if (weaponLevel != level) {
			weaponLevel = level;
			if (Constant.WEAPON_SPEED_RATE.ContainsKey(curWeaponName)) {
				attackSpeedRate = Constant.WEAPON_SPEED_RATE[curWeaponName][weaponLevel - 1];
				if (AttackSpeedChangeEvent != null) {
					AttackSpeedChangeEvent(attackSpeedRate);
				}
			}
		}
	}
	public float GetMaxWeaponEnergy()
	{
		if (Constant.MAX_WEAPON_ENERGY.ContainsKey(curWeaponName) &&
			Constant.MAX_WEAPON_ENERGY[curWeaponName].Count > weaponLevel) {
				List<float> maxList = Constant.MAX_WEAPON_ENERGY[curWeaponName];
				return maxList[weaponLevel] - maxList[weaponLevel - 1];
		}
		return -1;
	}
	public int GetWeaponLevel()
	{
		return weaponLevel;
	}
	public float GetWeaponEnergy()
	{
		return weaponEnergy;
	}
	/*------------- WeaponEnergyChangeEvent ---------------*/
}
