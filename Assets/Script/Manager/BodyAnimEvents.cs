﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimEvents : MonoBehaviour {
	AnimEventsManager I_AnimEventsManager;
	void Awake()
	{
		I_AnimEventsManager = GetComponentInParent<AnimEventsManager>();
	}

	void Start()
	{

	}
	
	void Update () {
		
	}

	// 近战武器左右攻击动画切换
	void ChangeAtkDir()
	{
		I_AnimEventsManager.ChangeAtkDir();
	}

	/*--------------- 帧动画响应函数 ---------------*/
	// 武器攻击 doneAttackSndTime后开始播放AttackEndSnd
	void OnAttack(float doneAttackSndTime)
	{
		I_AnimEventsManager.OnAttack(doneAttackSndTime);
	}

	// 辅助射击
	void AdjustAttackDirection()
	{
		I_AnimEventsManager.AdjustAttackDirection();
	}

	// 攻击结束
	void OnAttackEnd()
	{
		I_AnimEventsManager.OnAttackEnd();
	}

	// 攻击声音的后半段
	void PlayAttackDoneSound()
	{
		//I_AnimEventsManager.PlayAttackDoneSound();
	}

	void OnRollStart()
	{
		I_AnimEventsManager.OnRollStart();
	}

	void OnRollEnd()
	{
		I_AnimEventsManager.OnRollEnd();
	}

	void OnReloadEnd()
	{
		I_AnimEventsManager.OnReloadEnd();
	}

	void PlayReloadSound()
	{
		I_AnimEventsManager.PlayReloadSound();
	}

	void PlayReloadDoneSound()
	{
		I_AnimEventsManager.PlayReloadDoneSound();
	}

	void ResetOnceAttack()
	{
		I_AnimEventsManager.ResetOnceAttack();
	}
}
