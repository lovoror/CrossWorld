using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

public class MeleeWeaponManager : WeaponManager {

	public float AtkRange = 0;
	public float AtkAngle = 0;

	LayerMask playerLayerMask;
	LayerMask enemyLayerMask;
	LayerMask ignoreLayerMask;

	protected new void Awake()
	{
		base.Awake();
		playerLayerMask = LayerMask.GetMask("Player");
		enemyLayerMask = LayerMask.GetMask("Enemy");
		ignoreLayerMask = LayerMask.GetMask("Wall");
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void OnEnable()
	{
		base.OnEnable();
	}
	protected new void OnDisable()
	{
		base.OnDisable();
	}

	/*--------------------- AttackEvent ---------------------*/
		/*------------ Self -> Manager ------------*/
	protected override void AttackEventFunc(float doneAttackSndTime = 0)
	{
		if (I_Manager.IsDead()) return;
		base.AttackEventFunc();
		var targetsFound = GetEnemyInRange();
		BasicHurt(self, targetsFound);
	}
	/*--------------------- AttackEvent ---------------------*/

	/*----------------- Tool --------------------*/
	// 


	// 获得攻击范围内的敌人
	protected List<Transform> GetEnemyInRange()
	{
		List<Transform> targetsFound = new List<Transform>();
		Collider[] hitColliders = null;
		if (self.tag == "Player") {
			hitColliders = Physics.OverlapSphere(self.position, AtkRange, enemyLayerMask);
		}
		else if (self.tag == "Enemy") {
			hitColliders = Physics.OverlapSphere(self.position, AtkRange, playerLayerMask);
		}
		// 检测是否被墙格挡
		if (hitColliders != null) {
			foreach (Collider collider in hitColliders) {
				Vector3 s2h = collider.transform.position - self.position;
				s2h.y = 0;
				float angle = Vector3.Angle(s2h, self.transform.forward);
				if (angle - 5 <= AtkAngle / 2) {
					//RaycastHit hit;
					bool isDead = collider.transform.GetComponent<Manager>().IsDead();
					if (!isDead && !Physics.Linecast(self.position, collider.transform.position, ignoreLayerMask)) {
						if(!isDead) targetsFound.Add(collider.transform);
					}
				}
			}
		}
		return targetsFound;
	}
	// 攻击范围内存在敌人
	protected bool HasEnemyInRange()
	{
		Collider[] hitColliders = null;
		if (self.tag == "Player") {
			hitColliders = Physics.OverlapSphere(self.position, AtkRange, enemyLayerMask);
		}
		else if(self.tag == "Enemy"){
			hitColliders = Physics.OverlapSphere(self.position, AtkRange, playerLayerMask);
		}
		// 检测是否被墙格挡 是否在攻击范围内
		if (hitColliders != null) {
			foreach (Collider collider in hitColliders) {
				Vector3 s2h = collider.transform.position - self.position;
				s2h.y = 0;
				float angle = Vector3.Angle(s2h, self.transform.forward);
				if (angle - 5 <= AtkAngle / 2) {
					//RaycastHit hit;
					bool isDead = collider.transform.GetComponent<Manager>().IsDead();
					if (!isDead && !Physics.Linecast(self.position, collider.transform.position, ignoreLayerMask)) {
						return true;
					}
				}
			}
		}
		return false;
	}
}
