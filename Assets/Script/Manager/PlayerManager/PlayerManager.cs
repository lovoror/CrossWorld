
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
	public PlayerController I_PlayerController;
	[HideInInspector]
	public PlayerDataManager I_PlayerDataManager;
	[HideInInspector]
	public PlayerData I_PlayerData;

	new void Awake()
	{
		base.Awake();
		isPlayer = true;
		if (self) {
			I_PlayerMessenger = self.GetComponent<PlayerMessenger>();
			I_PlayerDataManager = self.GetComponent<PlayerDataManager>();
			I_PlayerController = self.GetComponent<PlayerController>();
		}
		I_PlayerData = PlayerData.Instance;
	}

	new void Start () 
	{
		base.Start();
	}
	
	void Update () {
		
	}
}
