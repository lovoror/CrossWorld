using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AnimEvents 同意处理类
public class AnimEventsManager : MonoBehaviour
{
	// 攻击事件：BodyAnimEvents -> WeaponManager
	public delegate void AttackEventHandler();  // 通知WeaponManager武器攻击
	public event AttackEventHandler AttackEvent;

	Manager I_Manager;

	void Awake()
	{
		I_Manager = GetComponentInParent<Manager>();
	}

	void Start()
	{

	}

	void Update()
	{

	}

	// 近战武器左右攻击动画切换
	public void ChangeAtkDir()
	{
		Transform curBody = I_Manager.I_DataManager.curBodyTransform;
		float yScale = curBody.localScale.y;
		curBody.localScale = new Vector3(1, -yScale, 1);
	}

	/*--------------- 帧动画响应函数 ---------------*/
	// 武器攻击
	public void OnAttack()
	{
		if (AttackEvent != null) {
			AttackEvent();
		}
	}
}
