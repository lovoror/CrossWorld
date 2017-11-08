
/*-------------------------------------------
 * Enemy的核心管理类。
 * 分管EnemyMessenger,EnemyDataManager等等。
 *------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyManager : Manager {

	public EnemyMessenger I_EnemyMessenger;
	public EnemyDataManager I_EnemyDataManager;

	new void Awake()
	{
		base.Awake();
		if (owner) {
			I_EnemyMessenger = owner.GetComponent<EnemyMessenger>();
			I_EnemyDataManager = owner.GetComponent<EnemyDataManager>();
		}
	}

	new void Start () 
	{
		base.Start();
	}
	
	void Update () {
		
	}
}
