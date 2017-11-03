
/*---------------------------
 * Player与Observer间的通信类。
 * Player->Observer
 *--------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMessenger : Messenger {

	protected PlayerManager I_PlayerManager;


	new void Awake()
	{
		base.Awake();
		if (owner) {
			I_PlayerManager = owner.GetComponent<PlayerManager>();
		}
	}

	new void Start()
	{
		base.Start();

	}
	
	void Update () {
		
	}
}
