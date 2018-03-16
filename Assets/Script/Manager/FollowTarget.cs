using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	public float followSpeed = 4.0f;
	public float scaleSpeed = 2f;
	public Vector2 offset = Vector2.zero;

	private Transform follow;
	private Camera mainCamera;
	private float sizeScale = 1.0f;  // 视野范围放缩系数
	private float orgSize;

	// Use this for initialization
	void Start () {
		follow = GameObject.FindWithTag("Player").transform;
		mainCamera = GetComponent<Camera>();
		orgSize = mainCamera.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate()
	{
		Vector3 offset3D = new Vector3(offset.x, 10, offset.y);
		Vector3 tarPosition = follow.position + offset3D;
		transform.position = Vector3.Lerp(transform.position, tarPosition, Time.fixedDeltaTime * followSpeed);
		if (mainCamera.orthographicSize != sizeScale * orgSize) {
			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, sizeScale * orgSize, Time.fixedDeltaTime * scaleSpeed);
		}
	}

	public void SetOffset(Vector2 newOffset)
	{
		offset = newOffset;
	}

	public void SetSizeScale(float scale)
	{
		sizeScale = scale;
	}

	public void Reset()
	{
		offset = Vector2.zero;
		sizeScale = 1.0f;
	}
}
