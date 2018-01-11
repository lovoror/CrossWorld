using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public class LStick
{
	public int moveType;
	public Vector2 direction;
}

public class MoboController : MonoBehaviour
{
	public delegate void PlayerMoveEventHandler(LStick L);
	public static event PlayerMoveEventHandler PlayerMoveEvent;

	LStick L = new LStick();

	void Awake()
	{
		
	}

	void OnEnable()
	{
		// UILStickMoveEvent
		LPanel.UILStickMoveEvent += new LPanel.UILStickMoveEventHandler(UILStickMoveFunc);
		// UIAttackUpEvent
		AttackButton.UIAttackUpEvent += new AttackButton.UIAttackUpEventHandler(UIAttackUpEventFunc);
	}

	void OnDisable()
	{
		LPanel.UILStickMoveEvent -= UILStickMoveFunc;
		AttackButton.UIAttackUpEvent -= UIAttackUpEventFunc;
	}

	void Update()
	{

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

	}
	/*==================== UIAttackUpEvent ====================*/

}
