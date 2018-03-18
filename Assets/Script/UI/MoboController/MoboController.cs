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
	//FuncRButton[] FuncRButtons = new FuncRButton[3];
	Dictionary<WeaponNameType, FuncRButton> FuncRButtons = new Dictionary<WeaponNameType, FuncRButton>();
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
		//FuncRButtons[0] = new FuncRButton(WeaponNameType.Machinegun);
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

	public void OnClick_Func_R(string str_WeaponName)
	{
		if (Observer.IsPlayerDead()) return;
		WeaponNameType weaponName = (WeaponNameType)System.Enum.Parse(typeof(WeaponNameType), str_WeaponName, true);
		FuncRButton func;
		if (FuncRButtons.ContainsKey(weaponName)) {
			func = FuncRButtons[weaponName];
		}
		else {
			func = new FuncRButton(weaponName);
			FuncRButtons[weaponName] = func;
		}
		func.OnClick();
	}

	public void Restart()
	{
		GameStageMachine.Instance.ChangeStage(GameStage.LOGIN);
	}
}
