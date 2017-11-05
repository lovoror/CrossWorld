using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimEvents : MonoBehaviour {

	// 伤害有效期事件：BodyAnimEvents -> MeleeWeaponManager
	public delegate void MeleeDamageEventHandler(bool canDamage);  // 通知MeleeWeaponManager是否是可造成伤害状态
	public event MeleeDamageEventHandler MeleeHurtEvent;
	// 发射子弹事件
	public delegate void BulletShootEventHandler(Transform shooter, string weaponName);
	public event BulletShootEventHandler BulletShootEvent;

	private Transform owner;

	void Awake()
	{
		owner = Utils.GetOwner(transform, Constant.TAGS.Attacker);
	}

	void Start()
	{

	}
	
	void Update () {
		
	}

	// 近战武器左右攻击动画切换
	void ChangeAtkDir()
	{
		float yScale = transform.localScale.y;
		transform.localScale = new Vector3(1, -yScale, 1);
	}

	/*--------------- 帧动画响应函数 ---------------*/
	// 检测近战武器是否攻击到目标
	void OnMeleeAtkBegan()
	{
		if (MeleeHurtEvent != null) {
			MeleeHurtEvent(true);
		}
	}
	void OnMeleeAtkEnd()
	{
		if (MeleeHurtEvent != null) {
			MeleeHurtEvent(false);
		}
	}

	// 远程武器发射子弹
	void OnBulletCreate(string weaponName)
	{
		if (owner) {
			BulletShootEvent(owner, weaponName);
		}
	}

}
