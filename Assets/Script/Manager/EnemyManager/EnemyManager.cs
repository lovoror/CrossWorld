
/*-------------------------------------------
 * Enemy的核心管理类。
 * 分管EnemyMessenger,EnemyDataManager等等。
 *------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class EnemyManager : Manager {

	[HideInInspector]
	public EnemyMessenger I_EnemyMessenger;
	[HideInInspector]
	public EnemyDataManager I_EnemyDataManager;

	protected BehaviorTree behaviorTree; 

	new void Awake()
	{
		base.Awake();
		isPlayer = false;
		if (self) {
			I_EnemyMessenger = self.GetComponent<EnemyMessenger>();
			I_EnemyDataManager = self.GetComponent<EnemyDataManager>();
			behaviorTree = self.GetComponent<BehaviorTree>();
		}
	}

	new void Start () 
	{
		base.Start();
	}
	
	void Update () {
		
	}

	public override void HurtNotifyEventDeal(Transform attacker, Transform suffer)
	{
		base.HurtNotifyEventDeal(attacker, suffer);
		if (self == suffer) {
			behaviorTree.SendEvent("GetHurtEvent");
		}
	}
}
