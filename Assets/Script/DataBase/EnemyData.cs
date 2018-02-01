using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : BaseData
{
	public void Init(Transform enemy, WeaponNameType curWeaponName, float enemyMaxHealth, Dictionary<WeaponNameType, Transform> d_EnemyWeapons, Dictionary<WeaponNameType, Transform> d_EnemyBodys){
		base.Init();
		if (enemy) {
			this.target = enemy;
			isPlayer = false;
			Transform leg = null;
			leg = enemy.Find("Leg");
			legTransform = leg;
		}

		foreach (var weapon in d_EnemyWeapons) {
			AddToWeapons(weapon.Key, weapon.Value);
		}

		foreach (var body in d_EnemyBodys) {
			AddToBodys(body.Key, body.Value);
		}
		maxHealth = enemyMaxHealth;
		curHealth = enemyMaxHealth;
		this.curWeaponName = curWeaponName;
	}
}
