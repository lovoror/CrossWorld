using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MoveType
{
	stop, walk, run, rush
}

public enum DirectionType
{
	up, upRight, right, downRight, down, downLeft, left, upLeft
}

public enum ControlType
{
	modern, ancient
}

public class LStick
{
	public int moveType;
	public Vector2 direction;
}

public class MoboController : MonoBehaviour
{
	[HideInInspector]
	int controlType;

	AttackButton ButtonA;
	
	public delegate void PlayerMoveEventHandler(LStick L);
	public static event PlayerMoveEventHandler PlayerMoveEvent;

	public delegate void PlayerFaceEventHandler(Vector2 direction);
	public static event PlayerFaceEventHandler PlayerFaceEvent;

	public delegate void AttackDownEventHandler(Vector2 position);
	public static event AttackDownEventHandler AttackDownEvent;

	public delegate void AttackUpEventHandler(float deltaTime);
	public static event AttackUpEventHandler AttackUpEvent;

	LStick L = new LStick();

	void Awake()
	{
		ButtonA = GetComponentInChildren<AttackButton>();
		controlType = (int)ControlType.modern;
	}

	void OnEnable()
	{
		// UILStickMoveEvent
		LPanel.UILStickMoveEvent += new LPanel.UILStickMoveEventHandler(UILStickMoveFunc);
		// UIAttackUpEvent
		AttackButton.UIAttackUpEvent += new AttackButton.UIAttackUpEventHandler(UIAttackUpEventFunc);
		// UIAttackDownEvent
		AttackButton.UIAttackDownEvent += new AttackButton.UIAttackDownEventHandler(UIAttackDownEventFunc);
	}

	void OnDisable()
	{
		LPanel.UILStickMoveEvent -= UILStickMoveFunc;
		AttackButton.UIAttackUpEvent -= UIAttackUpEventFunc;
	}

	Vector2 last_bodyDirection = Vector2.zero;
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
	}

	/*==================== UILStickMoveEvent ====================*/
	void UILStickMoveFunc(Vector2 direction)
	{
		L.direction = direction;
		float magnitude = direction.magnitude;
		if (magnitude < 0.15) {
			L.moveType = (int)MoveType.stop;
		}
		else if (magnitude < 0.9) {
			L.moveType = (int)MoveType.walk;
		}
		else {
			L.moveType = (int)MoveType.run;
		}
		// zpf test 需要添加控制中间层
		if (PlayerMoveEvent != null) {
			PlayerMoveEvent(L);
		}
	}
	/*==================== UILStickMoveEvent ====================*/


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
