using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	[HideInInspector]
	public AttackType attackType = AttackType.untouched;
	[HideInInspector]
	public Vector2 direction = new Vector2(0, 0);
	[HideInInspector]
	public int touchId = -1;


	[SerializeField]
	float dragThreshold = 0.2f;
	[SerializeField]
	float minHoldTime = 0.1f;

	public delegate void UIAttackDownEventHandler(Vector2 position);
	public static event UIAttackDownEventHandler UIAttackDownEvent;

	public delegate void UIAttackUpEventHandler(float deltaTime);
	public static event UIAttackUpEventHandler UIAttackUpEvent;

	//public delegate void UIAttackDragEventHandler(Vector2 direction);
	//public static event UIAttackDragEventHandler UIAttackDragEvent;

	//public delegate void UIAttackCancelDragEventHandler();
	//public static event UIAttackCancelDragEventHandler UIAttackCancelDragEvent;

	float lastPressDownTime = 0;
	bool isDraging = false;
	bool isHolding = false;

	float buttonRadious;

	void Awake()
	{
		Image image = GetComponent<Image>();
		image.alphaHitTestMinimumThreshold = 0.1f;
		buttonRadious = image.rectTransform.sizeDelta[0] / 2;
		dragThreshold *= buttonRadious;
	}

	public void Reset()
	{
		attackType = AttackType.untouched;
		direction = Vector2.zero;
		touchId = -1;
		lastPressDownTime = 0;
		isDraging = false;
		isHolding = false;
	}

	void Update()
	{
		if (touchId < 0) return;

		// 检测是否是Hold
		if (!isHolding && Time.time - lastPressDownTime > minHoldTime) {
			isHolding = true;
			// Hold Type
			attackType = isDraging ? AttackType.holdPointer : AttackType.hold;
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (touchId < 0) {
			lastPressDownTime = Time.time;
			touchId = data.pointerId;
			attackType = AttackType.unknown;
			if (UIAttackDownEvent != null) {
				UIAttackDownEvent(data.pressPosition);
			}
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		// 检测是否是click
		if (!isHolding) {
			// Click Type
			attackType = isDraging ? AttackType.clickPointer : AttackType.click;
		}

		if (UIAttackUpEvent != null) {
			UIAttackUpEvent(Time.time - lastPressDownTime);
		}

		Reset();
	}

	public void OnDrag(PointerEventData data)
	{
		Vector2 pointer = data.position - data.pressPosition;

		if (pointer.sqrMagnitude > dragThreshold * dragThreshold) {
			isDraging = true;
			direction = pointer / buttonRadious;
		}
		else if (isDraging) {
			// 取消滑动
			isDraging = false;
			direction = Vector2.zero;
		}

		// 确定AttackType
		// 若 isHolding == false 证明AttackType还是unknown状态，故无需修改。
		if (isHolding) {
			attackType = isDraging ? AttackType.holdPointer : AttackType.hold;
		}
	}

	public void SetDirection(Vector2 dir)
	{
		direction = dir;
	}
	public Vector2 GetDirection()
	{
		return direction;
	}
	public DirectionType GetDirection4()
	{
		float angle = Utils.GetAnglePZ2D(new Vector2(1, 0), direction);
		if (angle >= 45 && angle < 135) {
			return DirectionType.right;
		}
		else if (angle >= 135 && angle < 225) {
			return DirectionType.down;
		}
		else if (angle >= 225 && angle < 275) {
			return DirectionType.left;
		}
		else {
			return DirectionType.up;
		}
	}

	public DirectionType GetDirection8()
	{
		float angle = Utils.GetAnglePZ2D(new Vector2(1, 0), direction);
		if (angle >= 22.5 && angle < 67.5) {
			return DirectionType.upRight;
		}
		else if (angle >= 67.5 && angle < 112.5) {
			return DirectionType.right;
		}
		else if (angle >= 112.5 && angle < 157.5) {
			return DirectionType.downRight;
		}
		else if (angle >= 112.5 && angle < 157.5) {
			return DirectionType.downRight;
		}
		else if (angle >= 157.5 && angle < 202.5) {
			return DirectionType.down;
		}
		else if (angle >= 202.5 && angle < 247.5) {
			return DirectionType.downLeft;
		}
		else if (angle >= 247.5 && angle < 292.5) {
			return DirectionType.left;
		}
		else if (angle >= 292.5 && angle < 337.5) {
			return DirectionType.upLeft;
		}
		else {
			return DirectionType.up;
		}
	}
}
