using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
	int controlType;
	
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
		controlType = (int)ControlType.modern;
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
			if (controlType == (int)ControlType.modern) {
				if (PlayerMoveEvent != null) {
					PlayerMoveEvent(moveDirection);
				}
			}
			else if (controlType == (int)ControlType.ancient) {

			}
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

	public void Restart()
	{
		print("Restart");
		// SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
