using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimEvents : MonoBehaviour {

	// 伤害有效期事件：BodyAnimEvents -> MeleeWeaponManager
	public delegate void MeleeDamageEventHandler(bool canDamage);  // 通知MeleeWeaponManager是否是可造成伤害状态
	public event MeleeDamageEventHandler MeleeHurtEvent;

	// Use this for initialization
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

}
