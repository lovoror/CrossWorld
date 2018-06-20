using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

	protected Manager I_Manager { get; set; }
	protected BaseData selfData { get; set; }
	public bool isDead
	{
		get
		{
			return selfData.isDead;
		}
	}
	public WeaponNameType killedWeaponName = WeaponNameType.unknown;
	public float health
	{
		get
		{
			return selfData.curHealth;
		}
	}
	public float maxHealth { get; set; }
	public Transform curBodyTransform
	{
		get
		{
			return selfData.curBodyTransform;
		}
	}
	public Transform curWeaponTransform
	{
		get
		{
			return selfData.curWeaponTransform;
		}
	}
	public WeaponNameType curWeaponName
	{
		get
		{
			return selfData.curWeaponName;
		}
	}
	public float attackSpeedRate
	{
		get
		{
			return selfData.curWeaponSpeed;
		}
	}

	public int weaponLevel
	{
		get
		{
			return selfData.curWeaponLevel;
		}
	}
	public float weaponEnergy
	{
		get
		{
			return selfData.curWeaponEnergy;
		}
	}
	protected void Awake()
	{

	}

	protected void Start () {
		selfData = Utils.GetBaseData(transform);
		maxHealth = selfData.maxHealth;
		I_Manager = transform.GetComponent<Manager>();
	}
	
	void Update () {
		
	}

	protected void OnEnable()
	{
	}

	protected void OnDisable()
	{
	}

	public float GetHealth()
	{
		return health;
	}

	public float GetMaxHealth()
	{
		return maxHealth;
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
}
