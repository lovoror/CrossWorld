using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	public float followSpeed = 4.0f;
	public float scaleSpeed = 2f;
	public Vector2 offset = Vector2.zero;

	Transform follow;
	Vector2 followPos
	{
		get
		{
			Vector3 pos = follow.position;
			return new Vector2(pos.x, pos.z);
		}
	}
	Camera mainCamera;
	float sizeScale = 1.0f;  // 视野范围放缩系数
	float orgSize;
	Vector2 aimPos = new Vector2(-1000, -1000);  // 瞄准位置
	Vector2 aimDirection = Vector2.zero;         // 瞄准方向
	Vector2 aimDirectionPos = Vector2.zero;      // 只有瞄准方向没有瞄准位置时的虚拟瞄准点

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

	float maxX = 53.3f, maxY = 30;
	public void SetAimPos(Vector2 aimPos)
	{
		this.aimPos = aimPos;
		if (aimPos == new Vector2(-1000, -1000)) {
			sizeScale = 1;
			offset = Vector2.zero;
			return;
		}
		Vector2 direction = aimPos - followPos;
		offset = direction / 2;
		float dX = Mathf.Abs(direction.x);
		float dY = Mathf.Abs(direction.y);
		sizeScale = Mathf.Max(dX / maxX, dY / maxY);
		sizeScale = sizeScale < 1 ? 1 : sizeScale;
	}

	public void SetAimDirection(Vector2 direction)
	{
		aimDirection = direction;
		if (aimPos == new Vector2(-1000, -1000)) {
			float degree = Utils.GetAnglePY(new Vector3(direction.x, 0, direction.y), Vector3.right);
			float rate = direction.magnitude;
			rate = rate > 1 ? 1 : rate;
			offset = GetOffsetByAngle(degree) * rate;
			//Vector2 f2o = offset - followPos;
			//float dX = Mathf.Abs(f2o.x);
			//float dY = Mathf.Abs(f2o.y);
			//sizeScale = Mathf.Max(dX / maxX, dY / maxY);
			//sizeScale = sizeScale < 1 ? 1 : sizeScale;
		}
	}

	public void Reset()
	{
		aimPos = new Vector2(-1000, -1000);
		sizeScale = 1;
		offset = Vector2.zero;
	}

	// 以Follow为中心做一个椭圆，返回椭圆与Follow面朝方向的焦点:
	// deno = √(b^2(Cosθ)^2+a^2(Sinθ)^2) 焦点：(abCosθ/deno, abSinθ/deno)
	Vector2 GetOffsetByAngle(float degree)
	{
		float a = 17.78f, b = 10;
		float r = degree * Mathf.Deg2Rad;
		float Cr = Mathf.Cos(r);
		float Sr = Mathf.Sin(r);
		float deno = Mathf.Sqrt(b * b * Cr * Cr + a * a * Sr * Sr);
		return new Vector2(a * b * Cr / deno, a * b * Sr / deno);
	}
}
