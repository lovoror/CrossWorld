
/*-------------------------------------------
 * Player的核心管理类。
 * 分管PlayerMessenger,PlayerDataManager等等。
 *------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : Manager {

	public PlayerMessenger I_PlayerMessenger;
	public PlayerDataManager I_PlayerDataManager;

	new void Awake()
	{
		base.Awake();
		if (owner) {
			I_PlayerMessenger = owner.GetComponent<PlayerMessenger>();
			I_PlayerDataManager = owner.GetComponent<PlayerDataManager>();
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
