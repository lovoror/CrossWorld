using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	public RectTransform LStickBg;
	public RectTransform LStick;

	public delegate void UILStickMoveEventHandler(Vector2 direction);
	public static event UILStickMoveEventHandler UILStickMoveEvent;

	Vector2 bgSize;
	Image bgImage;
	Vector2 stickSize;
	Vector2 orgPosition;
	float radious;

	void Awake()
	{
		bgImage = LStickBg.GetComponent<Image>();
		bgSize = LStickBg.sizeDelta;
		stickSize = LStick.sizeDelta;
		orgPosition = LStickBg.position;
		radious = (bgSize.x - stickSize.x) / 2;
	}

	public void OnPointerDown(PointerEventData data)
	{
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 1);
		LStickBg.position = data.pressPosition - bgSize / 2;
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 0.5f);
		LStickBg.position = orgPosition;
		LStick.position = orgPosition + LStickBg.sizeDelta / 2;
		// UILStickMoveEvent
		if (UILStickMoveEvent != null) {
			UILStickMoveEvent(new Vector2(0, 0));
		}
	}

	public void OnDrag(PointerEventData data)
	{
		Vector2 localPointerPosition = data.position - data.pressPosition;
		if (localPointerPosition.magnitude <= radious) {
			LStick.position = data.position;
		}
		else {
			LStick.position = data.pressPosition + localPointerPosition.normalized * radious;
		}
		// UILStickMoveEvent
		if (UILStickMoveEvent != null) {
			Vector2 lStickPosition2D = new Vector2(LStick.position.x, LStick.position.y);
			Vector2 direction = (lStickPosition2D - data.pressPosition) / radious;
			UILStickMoveEvent(direction);
		}
	}
}
