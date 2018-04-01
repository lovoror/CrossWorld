using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusTarget
{
	public Transform focusTarget;
	public bool isFocus;

	public FocusTarget(Transform target, bool isFocus = false)
	{
		focusTarget = target;
		this.isFocus = isFocus;
	}
}
public class FocusTargets
{
	public List<FocusTarget> focusTargets = new List<FocusTarget>();
	public int focusIndex
	{
		get
		{
			int index = -1;
			for (int i = 0; i < focusTargets.Count; ++i) {
				if (focusTargets[i].isFocus) {
					index = i;
					break;
				}
			}
			return index;
		}
	}

	public void SetFocusTarget(Transform target)
	{
		if (target == null) return;
		for (int i = 0; i < focusTargets.Count; ++i) {
			if (focusTargets[i].focusTarget == target) {
				focusTargets[i].isFocus = true;
			}
			else {
				focusTargets[i].isFocus = false;
			}
		}
	}
	public void SetTargets(List<Transform> targets)
	{
		// 移除focusTargets中的多余的条目
		for (int i = focusTargets.Count - 1; i >= 0; --i) {
			if (!targets.Contains(focusTargets[i].focusTarget)) {
				focusTargets.Remove(focusTargets[i]);
			}
		}
		// 添加focusTargets中的缺少的条目
		foreach (Transform target in targets) {
			bool contains = false;
			foreach (FocusTarget focusTarget in focusTargets) {
				if (focusTarget.focusTarget == target) {
					contains = true;
					break;
				}
			}
			if (!contains) {
				FocusTarget focusTarget = new FocusTarget(target);
				focusTargets.Add(focusTarget);
			}
		}
	}
}

public class PlayerData : BaseData
{
	protected static PlayerData m_instance = null;
	[HideInInspector]
	public FocusTargets I_FocusTargets = new FocusTargets();
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
