using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoboControllerSd : MonoBehaviour
{
	LeftStick StickL;
	PlayerData I_PlayerData;
	
	public delegate void PlayerMoveEventHandler(Vector2 dir);
	public static event PlayerMoveEventHandler PlayerMoveEvent;

	public delegate void AttackDownEventHandler(Vector2 position);
	public static event AttackDownEventHandler AttackDownEvent;

	public delegate void AttackUpEventHandler(float deltaTime);
	public static event AttackUpEventHandler AttackUpEvent;

	void Awake()
	{
		StickL = GetComponentInChildren<LeftStick>();
		I_PlayerData = PlayerData.Instance;
	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}

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

	/*--------------------- UIAttackUpEvent ---------------------*/
	void UIAttackUpEventFunc(float deltaTime)
	{
		if (AttackUpEvent != null) {
			AttackUpEvent(deltaTime);
		}
	}
	/*--------------------- UIAttackUpEvent ---------------------*/

	/*--------------------- UIAttackUpEvent ---------------------*/
	void UIAttackDownEventFunc(Vector2 position)
	{
		if (AttackDownEvent != null) {
			AttackDownEvent(position);
		}
	}
	/*--------------------- UIAttackUpEvent ---------------------*/

	public void Restart()
	{
		GameStageMachine.Instance.ChangeStage(GameStage.LOGIN);
	}
}
