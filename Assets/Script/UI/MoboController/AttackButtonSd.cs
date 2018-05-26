using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AttackSdType
{
	none,						// 没有按下AttackBtn
	A, AU, AD, AL, AR,			// 短按，刚按下即赋值为短按
	RA, RAU, RAD, RAL, RAR,		// 短按释放
	HA, HAU, HAD, HAL, HAR,		// 长按
	RHA, RHAU, RHAD, RHAL, RHAR	// 长按释放
}

public class AttackButtonSd : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	[HideInInspector]
	public AttackSdType attackSdType = AttackSdType.none;
	AttackSdType preAttackSdType = AttackSdType.none;
	[HideInInspector]
	public Vector2 direction = new Vector2(0, 0);

	DirectionType4 direction4 = DirectionType4.none;
	int touchId = -1;

	[SerializeField]
	float dragThreshold = 0.2f;
	[SerializeField]
	float minHoldTime = 0.1f;

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

	public delegate void UIAttackSdTypeEventHandler(AttackSdType attackSdType);
	public static event UIAttackSdTypeEventHandler UIAttackSdTypeEvent;

	//public delegate void UIAttackDownEventHandler(AttackSdType attackSdType, Vector2 position);
	//public static event UIAttackDownEventHandler UIAttackDownEvent;

	//public delegate void UIAttackUpEventHandler(AttackSdType attackSdType, AttackSdType preAttackSdType, float deltaTime);
	//public static event UIAttackUpEventHandler UIAttackUpEvent;

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
		attackSdType = preAttackSdType = AttackSdType.none;
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
		}

		// 设置 AttackSdType
		if (isHolding && isDraging) {

		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		// 显示背景
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 1);
		AStickBg.position = data.pressPosition + new Vector2(bgSize.x / 2, 0) - new Vector2(0, bgSize.y / 2);
		stickImage.color = new Color(0.31f, 0.89f, 0.94f, 1);

		// 确定attackType
		if (touchId < 0) {
			lastPressDownTime = Time.time;
			touchId = data.pointerId;
			attackSdType = AttackSdType.A;
			if (UIAttackSdTypeEvent != null) {
				UIAttackSdTypeEvent(attackSdType);
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

		// 确定释放类型：AR/HAR
		if (isHolding) {
			attackSdType = AttackSdType.HAR;
		}
		else {
			attackSdType = AttackSdType.AR;
		}
		// 发出释放事件
		if (UIAttackSdTypeEvent != null) {
			UIAttackSdTypeEvent(attackSdType);
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
		else {
			// 取消滑动
			isDraging = false;
			direction = Vector2.zero;
		}
	}

	DirectionType4 GetDirectionType4()
	{
		if (direction.sqrMagnitude < dragThreshold * dragThreshold) return DirectionType4.none;

		return Utils.GetDirection4(direction);
	}

	//public void SetDirection(Vector2 dir)
	//{
	//	direction = dir;
	//}
	public Vector2 GetDirection()
	{
		return direction;
	}
}
