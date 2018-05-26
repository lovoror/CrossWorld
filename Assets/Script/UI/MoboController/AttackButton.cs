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

	[SerializeField]
	RectTransform AStickBg;
	[SerializeField]
	RectTransform AStick;
	Vector2 bgSize;
	Image bgImage;
	Image stickImage;
	Vector2 stickSize;
	Vector2 orgPosition;
	float radious;

	void Awake()
	{
		Image image = GetComponent<Image>();
		//image.alphaHitTestMinimumThreshold = 0.08f;
		buttonRadious = image.rectTransform.sizeDelta[0] / 2;
		dragThreshold *= buttonRadious;

		bgImage = AStickBg.GetComponent<Image>();
		stickImage = AStick.GetComponent<Image>();
		bgSize = AStickBg.sizeDelta;
		stickSize = AStick.sizeDelta;
		orgPosition = AStickBg.position;
		radious = (bgSize.x - stickSize.x) / 2;
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
		// 显示背景
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 1);
		AStickBg.position = data.pressPosition + new Vector2(bgSize.x / 2, 0) - new Vector2(0, bgSize.y / 2);
		stickImage.color = new Color(0.31f, 0.89f, 0.94f, 1);

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
		// 隐藏背景
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 0);
		AStickBg.position = orgPosition;
		AStick.position = orgPosition - new Vector2(AStickBg.sizeDelta.x / 2, 0) + new Vector2(0, AStickBg.sizeDelta.y / 2);
		stickImage.color = new Color(1, 1, 1, 0.8f);
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
		// 准星滑动
		Vector2 localPointerPosition = data.position - data.pressPosition;
		if (localPointerPosition.magnitude <= radious) {
			AStick.position = data.position;
		}
		else {
			AStick.position = data.pressPosition + localPointerPosition.normalized * radious;
		}

		// 功能实现
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
}
