using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public delegate void RollEventHandler(Vector2 direction);
	public static event RollEventHandler RollEvent;

	public Image Image_Bg;
	public float dragThresholdRate = 0.1f;
	float dragThreshold;

	Color c;
	Vector3 bgPos;
	Vector2 bgSize;
	Vector2 btnSize;
	float radious;  // 准星移动半径
	void Awake()
	{
		
	}

	void Start ()
	{
		c = Image_Bg.color;
		bgPos = Image_Bg.transform.position;
		Image_Bg.color = new Color(c.r, c.g, c.b, 0);
		bgSize = Image_Bg.rectTransform.sizeDelta;
		btnSize = (transform as RectTransform).sizeDelta;
		dragThreshold = dragThresholdRate * bgSize.x;
		radious = (bgSize.x - btnSize.x) / 2;
	}
	
	void Update ()
	{
		
	}
	public void OnPointerDown(PointerEventData data)
	{
		Image_Bg.color = new Color(c.r, c.g, c.b, 1);
		Image_Bg.transform.position = data.pressPosition;
		transform.position = data.pressPosition;
	}
	public void OnPointerUp(PointerEventData data)
	{
		// 功能实现
		Vector2 localPointerPosition = data.position - data.pressPosition;
		Vector2 dir = localPointerPosition.sqrMagnitude < dragThreshold * dragThreshold ? Vector2.zero : localPointerPosition;
		if (RollEvent != null) {
			RollEvent(dir);
		}
		Image_Bg.color = new Color(c.r, c.g, c.b, 0);
		Image_Bg.transform.position = bgPos;
		transform.position = bgPos;
	}

	public void OnDrag(PointerEventData data)
	{
		// 准星滑动
		Vector2 localPointerPosition = data.position - data.pressPosition;
		if (localPointerPosition.magnitude <= radious) {
			transform.position = data.position;
		}
		else {
			transform.position = data.pressPosition + localPointerPosition.normalized * radious;
		}
	}
}
