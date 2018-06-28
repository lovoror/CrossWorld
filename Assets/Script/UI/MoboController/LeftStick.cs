using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MoveType
{
	stop, walk, run, rush
}

public class LeftStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
	[HideInInspector]
	public MoveType moveType;
	[HideInInspector]
	public Vector2 direction;

	[SerializeField]
	RectTransform LStickBg;
	[SerializeField]
	RectTransform LStick;

	Vector2 bgSize;
	Image bgImage;
	Vector2 stickSize;
	Vector2 orgPosition;
	float radious;

	void Awake()
	{
		bgImage = LStickBg.GetComponent<Image>();
	}

	void Start()
	{
		bgSize = LStickBg.sizeDelta;
		stickSize = LStick.sizeDelta;
		orgPosition = LStickBg.position;
		radious = (bgSize.x - stickSize.x) / 2;
	}

	public void OnPointerDown(PointerEventData data)
	{
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 1);
		LStickBg.position = data.pressPosition;
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 0.5f);
		LStickBg.position = orgPosition;
		LStick.position = orgPosition;
		ChangeDirection(Vector2.zero);
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

		Vector2 lStickPosition2D = new Vector2(LStick.position.x, LStick.position.y);
		Vector2 dir = (lStickPosition2D - data.pressPosition) / radious;
		ChangeDirection(dir);
	}

	void ChangeDirection(Vector2 dir)
	{
		direction = dir;
		float sqrMagnitude = direction.sqrMagnitude;
		if (sqrMagnitude < 0.15 * 0.15) {
			moveType = MoveType.stop;
		}
		else if (sqrMagnitude < 0.9 * 0.9) {
			moveType = MoveType.walk;
		}
		else {
			moveType = MoveType.run;
		}
	}
}
