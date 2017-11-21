
/*-------------------------------------------
 * Player的核心管理类。
 * 分管PlayerMessenger,PlayerDataManager等等。
 *------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : Manager {

	[HideInInspector]
	public PlayerMessenger I_PlayerMessenger;
	[HideInInspector]
	public PlayerDataManager I_PlayerDataManager;

	new void Awake()
	{
		base.Awake();
		if (self) {
			I_PlayerMessenger = self.GetComponent<PlayerMessenger>();
			I_PlayerDataManager = self.GetComponent<PlayerDataManager>();
		}
	}

	new void Start () 
	{
		base.Start();
	}
	
	void Update () {
		
	}

	/*--------------------- HurtEvent ---------------------*/
		/*------------ Observer -> Manager ------------*/
}
