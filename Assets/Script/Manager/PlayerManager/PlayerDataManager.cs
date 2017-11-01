using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour {
	public float health = 100;
	public float maxHealth = 100;

	// 玩家死亡事件
	public delegate void DeadEventHandler(Transform target);
	public static event DeadEventHandler DeadEvent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeHealth(float delta)
	{
		health += delta;
		health = health < 0 ? 0 : health;
		health = health > maxHealth ? maxHealth : health;
		if (health == 0 && DeadEvent != null) {
			DeadEvent(transform);
		}
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
