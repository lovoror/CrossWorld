
/*---------------------------
 * Enemy与Observer间的通信类。
 * Enemy->Observer
 *--------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMessenger : Messenger {

	protected EnemyManager I_EnemyManager;

	new void Awake()
	{
		base.Awake();
		if (self) {
			I_EnemyManager = self.GetComponent<EnemyManager>();
		}
	}

	new void Start()
	{
		base.Start();
	}
	
	void Update () {
		
	}
}
