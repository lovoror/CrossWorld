using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AttackType
{
	untouched, unknown, click, hold, clickPointer, holdPointer
}

public enum DirectionType
{
	up, upRight, right, downRight, down, downLeft, left, upLeft
}

public enum ControlType
{
	modern, ancient
}

public class MoboController : MonoBehaviour
{

	AttackButton ButtonA;
	LeftStick StickL;
	FuncRButton[] FuncRButtons = new FuncRButton[2];
	//ControlType controlType;
	
	public delegate void PlayerMoveEventHandler(Vector2 dir);
	public static event PlayerMoveEventHandler PlayerMoveEvent;

	public delegate void PlayerFaceEventHandler(Vector2 direction);
	public static event PlayerFaceEventHandler PlayerFaceEvent;

	public delegate void AttackDownEventHandler(Vector2 position);
	public static event AttackDownEventHandler AttackDownEvent;

	public delegate void AttackUpEventHandler(float deltaTime);
	public static event AttackUpEventHandler AttackUpEvent;

	void Awake()
	{
		ButtonA = GetComponentInChildren<AttackButton>();
		StickL = GetComponentInChildren<LeftStick>();
		//controlType = ControlType.modern;
		FuncRButtons[0] = new FuncRButton(WeaponNameType.Machinegun);
		FuncRButtons[1] = new FuncRButton(WeaponNameType.Knife);
	}

	void OnEnable()
	{
		// UIAttackUpEvent
		AttackButton.UIAttackUpEvent += new AttackButton.UIAttackUpEventHandler(UIAttackUpEventFunc);
		// UIAttackDownEvent
		AttackButton.UIAttackDownEvent += new AttackButton.UIAttackDownEventHandler(UIAttackDownEventFunc);
	}

	void OnDisable()
	{
		AttackButton.UIAttackUpEvent -= UIAttackUpEventFunc;
		AttackButton.UIAttackDownEvent -= UIAttackDownEventFunc;
	}

	Vector2 last_bodyDirection = Vector2.zero;
	Vector2 last_moveDirection = Vector2.zero;
	void Update()
	{
		// 人物朝向
		Vector2 bodyDirection = ButtonA.direction;
		if (bodyDirection != Vector2.zero && bodyDirection != last_bodyDirection) {
			last_bodyDirection = bodyDirection;
			// 改变人物朝向
			if (PlayerFaceEvent != null) {
				PlayerFaceEvent(bodyDirection);
			}
		}

		// 移动方向
		Vector2 moveDirection = StickL.direction;
		if (moveDirection != last_moveDirection) {
			last_moveDirection = moveDirection;
			// 改变移动方向
			//if (controlType == ControlType.modern) {
				if (PlayerMoveEvent != null) {
					PlayerMoveEvent(moveDirection);
				}
			//}
			//else if (controlType == ControlType.ancient) {

			//}
		}
	}

	/*==================== UIAttackUpEvent ====================*/
	void UIAttackUpEventFunc(float deltaTime)
	{
		if (AttackUpEvent != null) {
			AttackUpEvent(deltaTime);
		}
	}
	/*==================== UIAttackUpEvent ====================*/

	/*==================== UIAttackUpEvent ====================*/
	void UIAttackDownEventFunc(Vector2 position)
	{
		if (AttackDownEvent != null) {
			AttackDownEvent(position);
		}
	}
	/*==================== UIAttackUpEvent ====================*/

	public void OnClick_Func_R1()
	{
		if (Observer.IsPlayerDead()) return;
		FuncRButtons[0].OnClick();
	}

	public void OnClick_Func_R2()
	{
		if (Observer.IsPlayerDead()) return;
		FuncRButtons[1].OnClick();
	}

	public void Restart()
	{
		GameStageMachine.Instance.ChangeStage(GameStage.LOGIN);
	}
}
