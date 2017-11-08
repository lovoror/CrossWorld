using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDisplay : MonoBehaviour {

	protected SpriteRenderer healthBar;
	protected SpriteRenderer healthBarOutline;
	protected Manager I_Manager;
	protected float health;
	protected float maxHealth;
	protected Vector3 healthScale;
	protected float lastHealth; // 上次显示的体力

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
	}

	protected void Start ()
	{
		Transform bar = Utils.FindChildRecursively(transform, "HealthBar");
		Transform barOutline = Utils.FindChildRecursively(transform, "HealthOutline");
		if (bar) {
			healthBar = bar.GetComponent<SpriteRenderer>();
			healthScale = healthBar.transform.localScale;
		}
		if (barOutline) {
			healthBarOutline = barOutline.GetComponent<SpriteRenderer>();
		}
		maxHealth = I_Manager.I_DataManager.GetMaxHealth();
	}
	
	protected void Update ()
	{
		health = I_Manager.I_DataManager.GetHealth();
		if (health != lastHealth) {
			lastHealth = health;
			ShowHealthBar();
		}
	}



	void ShowHealthBar()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health / maxHealth);
		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health / maxHealth, healthScale.y, healthScale.z);
		healthBarOutline.enabled = health > 0;
	}
}
