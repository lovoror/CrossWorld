using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimEvents : MonoBehaviour {

	// 攻击事件：BodyAnimEvents -> WeaponManager
	public delegate void AttackEventHandler();  // 通知WeaponManager武器攻击
	public event AttackEventHandler AttackEvent;

	private Transform self;

	void Awake()
	{
		self = Utils.GetOwner(transform, Constant.TAGS.Attacker);
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
	// 近战武器攻击
	void OnAttack()
	{
		if (AttackEvent != null) {
			AttackEvent();
		}
	}
}
