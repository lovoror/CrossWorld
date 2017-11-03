using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {

	public float health = 100;

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
	}

	public float GetHealth()
	{
		return health;
	}
}
