using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public float moveSpeed = 2;
	public Camera mainCamera;
	FollowTarget I_FollowTarget;
	Vector3 startPos;
	void Awake()
	{
		I_FollowTarget = mainCamera.GetComponent<FollowTarget>();
	}

	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

	public void OnPointerDown(PointerEventData data)
	{
		I_FollowTarget.controlByOther = true;
		startPos = mainCamera.transform.position;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		I_FollowTarget.controlByOther = false;

	}

	public void OnDrag(PointerEventData data)
	{
		Vector2 mv = data.position - data.pressPosition;
		mainCamera.transform.position = new Vector3(startPos.x + mv.x * moveSpeed, startPos.y, startPos.z + mv.y * moveSpeed);
	}
}
