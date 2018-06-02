using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoboControllerSd : MonoBehaviour
{
	LeftStick StickL;

	public delegate void PlayerMoveEventHandler(Vector2 dir);
	public static event PlayerMoveEventHandler PlayerMoveEvent;

	public delegate void PlayerAttackEventHandler(AttackSdType attackType);
	public static event PlayerAttackEventHandler PlayerAttackEvent;

	void Awake()
	{
		StickL = GetComponentInChildren<LeftStick>();
	}

	void OnEnable()
	{
		AttackButtonSd.UIAttackSdTypeEvent += new AttackButtonSd.UIAttackSdTypeEventHandler(UIAttackSdTypeEventFunc);
	}

	void OnDisable()
	{
		AttackButtonSd.UIAttackSdTypeEvent -= UIAttackSdTypeEventFunc;
	}

	/*------------------- UIAttackSdTypeEvent -----------------------*/
	void UIAttackSdTypeEventFunc(AttackSdType attackSdType)
	{
		if (PlayerAttackEvent != null) {
			PlayerAttackEvent(attackSdType);
		}
	}

	/*------------------- UIAttackSdTypeEvent -----------------------*/

	Vector2 last_moveDirection = Vector2.zero;
	void Update()
	{
		// 移动方向
		Vector2 moveDirection = StickL.direction;
		if (moveDirection != last_moveDirection) {
			last_moveDirection = moveDirection;
			// 改变移动方向
			if (PlayerMoveEvent != null) {
				PlayerMoveEvent(moveDirection);
			}
		}
	}

	public void Restart()
	{
		GameStageMachine.Instance.ChangeStage(GameStage.LOGIN);
	}
}
