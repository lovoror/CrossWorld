using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	[SerializeField]
	RectTransform AStickBg;
	[SerializeField]
	RectTransform AStick;

	Vector2 bgSize;
	Image bgImage;
	Vector2 stickSize;
	Vector2 orgPosition;
	float radious;

	void Awake()
	{
		bgImage = AStickBg.GetComponent<Image>();
		bgSize = AStickBg.sizeDelta;
		stickSize = AStick.sizeDelta;
		orgPosition = AStickBg.position;
		radious = (bgSize.x - stickSize.x) / 2;
	}

	public void OnPointerDown(PointerEventData data)
	{
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 1);
		AStickBg.position = data.pressPosition - bgSize / 2;
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 0);
		AStickBg.position = orgPosition;
		AStick.position = orgPosition + AStickBg.sizeDelta / 2;
	}

	public void OnDrag(PointerEventData data)
	{
		Vector2 localPointerPosition = data.position - data.pressPosition;
		if (localPointerPosition.magnitude <= radious) {
			AStick.position = data.position;
		}
		else {
			AStick.position = data.pressPosition + localPointerPosition.normalized * radious;
		}
	}
}
