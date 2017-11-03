using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateDisplay : MonoBehaviour {

	private SpriteRenderer healthBar;
	private EnemyDataManager dataManager;
	private float health;
	private float maxHealth;
	private Vector3 healthScale;
	private float lastHealth; // 上次显示的体力

	// Use this for initialization
	void Start()
	{
		Transform bar = FindChildRecursively(transform, "HealthBar");
		if (bar) {
			healthBar = bar.GetComponent<SpriteRenderer>();
		}
		dataManager = transform.GetComponent<EnemyDataManager>();
		healthScale = healthBar.transform.localScale;
		maxHealth = dataManager.GetMaxHealth();
	}
	
	// Update is called once per frame
	void Update () {
		health = dataManager.GetHealth();
		if (health != lastHealth) {
			lastHealth = health;
			ShowHealthBar();
		}
	}

	Transform FindChildRecursively(Transform top, string name)
	{
		Transform tar;
		tar = top.Find(name);
		if (!tar) {
			foreach (Transform child in top) {
				tar = FindChildRecursively(child, name);
				if (tar) break;
			}
		}
		return tar;
	}

	public void ShowHealthBar()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health / maxHealth);
		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health / maxHealth, healthScale.y, healthScale.z);
	}
}
