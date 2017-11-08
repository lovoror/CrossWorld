using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

	protected Manager I_Manager;

	public float health = 100;
	public float maxHealth = 100;
	public string weaponName;

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
	}

	protected void Start () {
		weaponName = I_Manager.I_WeaponManager.weaponName;
	}
	
	void Update () {
		
	}

	public void ChangeHealth(float delta)
	{
		health += delta;
		health = health < 0 ? 0 : health;
		health = health > maxHealth ? maxHealth : health;
		//if (health == 0 && DeadEvent != null) {
		//	DeadEvent(transform);
		//}
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
}
