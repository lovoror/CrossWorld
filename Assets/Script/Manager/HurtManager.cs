using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy受伤处理
public class HurtManager : MonoBehaviour {
	private PlayerDataManager dataManager;

	// Use this for initialization
	void Start () {
		dataManager = transform.GetComponent<PlayerDataManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable()
	{
		//WeaponManager.WeaponHurtEvent += new WeaponManager.WeaponHurtEventHandler(WeaponHurtEventFunc);
	}

	void OnDisable()
	{
		//WeaponManager.WeaponHurtEvent -= WeaponHurtEventFunc;
	}

	// 基础受伤处理函数
	void WeaponHurtEventFunc(Transform suffer, float damage)
	{
		if (suffer == transform) {
			if (dataManager) {
				dataManager.ChangeHealth(-damage);
			}
		}
	}
}
