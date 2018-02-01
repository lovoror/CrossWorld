using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : BaseData
{
	protected static PlayerData m_instance = null;
	public static PlayerData Instance
	{
		get
		{
			if (m_instance == null)
				m_instance = new PlayerData();
			return m_instance;
		}
	}

	public void Init(Transform player, WeaponNameType curWeaponName, float playerMaxHealth, Dictionary<WeaponNameType, Transform> d_PlayerWeapons, Dictionary<WeaponNameType, Transform> d_PlayerBodys){
		base.Init();
		if (player) {
			this.target = player;
			isPlayer = false;
			Transform leg = null;
			leg = player.Find("Leg");
			legTransform = leg;
		}

		foreach (var body in d_PlayerWeapons) {
			AddToWeapons(body.Key, body.Value);
		}

		foreach (var body in d_PlayerBodys) {
			AddToBodys(body.Key, body.Value);
		}
		maxHealth = playerMaxHealth;
		curHealth = playerMaxHealth;
		this.curWeaponName = curWeaponName;
	}

	public void StageEnd()
	{
		m_instance = null;
	}
}
