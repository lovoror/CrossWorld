using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemysData
{
	protected static EnemysData m_instance = null;
	public static EnemysData Instance
	{
		get
		{
			if (m_instance == null)
				m_instance = new EnemysData();
			return m_instance;
		}
	}

	public Dictionary<Transform, EnemyData> d_EnemysData = new Dictionary<Transform, EnemyData>();
	public int count
	{
		get{
			return d_EnemysData.Count;
		}
	}
	public Transform[] enemyTransforms
	{
		get
		{
			Transform[] enemyTransforms = new Transform[d_EnemysData.Count];
			d_EnemysData.Keys.CopyTo(enemyTransforms, 0);
			return enemyTransforms;
		}
	}

	public void OnInitEnd()
	{

	}

	public void AddEnemyData(Transform enemy, WeaponNameType curWeaponName, float enemyMaxHealth, Dictionary<WeaponNameType, Transform> d_EnemyWeapons, Dictionary<WeaponNameType, Transform> d_EnemyBodys)
	{
		if (d_EnemysData.ContainsKey(enemy)) {
			d_EnemysData.Remove(enemy);
		}
		EnemyData enemyData = new EnemyData();
		enemyData.Init(enemy, curWeaponName, enemyMaxHealth, d_EnemyWeapons, d_EnemyBodys);
		d_EnemysData.Add(enemy, enemyData);
	}

	public EnemyData GetEnemyDataByTransform(Transform enemy)
	{
		if (d_EnemysData.ContainsKey(enemy)) {
			return d_EnemysData[enemy];
		}
		return null;
	}
	public void StageEnd()
	{
		m_instance = null;
	}
}
