using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AnimEvents 同意处理类
public class AnimEventsManager : MonoBehaviour
{
	// 攻击事件：BodyAnimEvents -> WeaponManager
	public delegate void AttackEventHandler();  // 通知WeaponManager武器攻击
	public event AttackEventHandler AttackEvent;
	public delegate void AttackEndEventHandler();  // 通知WeaponManager武器攻击结束
	public event AttackEndEventHandler AttackEndEvent;
	public delegate void OnReloadEndEventHandler();
	public event OnReloadEndEventHandler OnReloadEndEvent;
	public delegate void PlayReloadSoundEventHandler();
	public event PlayReloadSoundEventHandler PlayReloadSoundEvent;

	PlayerManager I_PlayerManager;
	Manager I_Manager;

	void Awake()
	{
		I_PlayerManager = GetComponentInParent<PlayerManager>();
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

	// 辅助射击
	public void AdjustAttackDirection()
	{
		I_PlayerManager.I_PlayerController.AdjustAttackDirection();
	}

	// 攻击结束
	public void OnAttackEnd()
	{
		if (AttackEndEvent != null) {
			AttackEndEvent();
		}
	}

	public void OnRollStart()
	{
		I_PlayerManager.I_PlayerController.OnRollStart();
	}

	public void OnRollEnd()
	{
		I_PlayerManager.I_PlayerController.OnRollEnd();
	}

	// Reload动画结束
	public void OnReloadEnd()
	{
		if (OnReloadEndEvent != null) {
			OnReloadEndEvent();
		}
	}

	public void PlayReloadSound()
	{
		if (PlayReloadSoundEvent != null) {
			PlayReloadSoundEvent();
		}
	}

	public void ResetOnceAttack()
	{
		I_PlayerManager.I_PlayerController.ResetOnceAttack();
	}
}
