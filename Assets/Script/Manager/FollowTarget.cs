using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	public float followSpeed = 8.0f;
	public float aimSpeed = 4.0f;
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

	// Use this for initialization
	void Start () {
		follow = GameObject.FindWithTag("Player").transform;
		mainCamera = GetComponent<Camera>();
		orgSize = mainCamera.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {

	}

	Vector3 curOffset = Vector3.zero;
	void FixedUpdate()
	{
		// 根据offset来同步Camera的位置，可实现跟随和瞄准/恢复不冲突。
		Vector3 offset3D = new Vector3(offset.x, 10, offset.y);
		curOffset = Vector3.Lerp(curOffset, offset3D, Time.fixedDeltaTime * aimSpeed);  // 当前的offset点
		// 跟随curOffset点
		transform.position = Vector3.Lerp(transform.position, follow.position + curOffset, Time.fixedDeltaTime * followSpeed);

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
		if (aimPos == new Vector2(-1000, -1000)) {
			float degree = Utils.GetAnglePY(new Vector3(direction.x, 0, direction.y), Vector3.right);
			float rate = Mathf.Clamp01(direction.magnitude);
			WeaponNameType curWeaponName = PlayerData.Instance.curWeaponName;
			float weaponAimRate = curWeaponName == WeaponNameType.Sniper ? 2.2f : 1;
			offset = rate * weaponAimRate * GetOffsetByAngle(degree);
			aimDirection = 2 * offset;
			float dX = Mathf.Abs(aimDirection.x);
			float dY = Mathf.Abs(aimDirection.y);
			sizeScale = Mathf.Max(dX / maxX, dY / maxY);
			sizeScale = sizeScale < 1 ? 1 : sizeScale;
		}
	}

	public void Reset()
	{
		aimPos = new Vector2(-1000, -1000);
		sizeScale = 1;
		offset = Vector2.zero;
	}

	// 以Follow为中心做一个椭圆，Camera被限制在这个椭圆内移动，返回椭圆与Follow面朝方向的焦点:
	// deno = √(b^2(Cosθ)^2+a^2(Sinθ)^2) 焦点：(abCosθ/deno, abSinθ/deno)
	// 1080分辨率对应为40m，1920对应为71.11m
	Vector2 GetOffsetByAngle(float degree)
	{
		float a = 17.78f, b = 15;
		float r = degree * Mathf.Deg2Rad;
		float Cr = Mathf.Cos(r);
		float Sr = Mathf.Sin(r);
		float deno = Mathf.Sqrt(b * b * Cr * Cr + a * a * Sr * Sr);
		return new Vector2(a * b * Cr / deno, a * b * Sr / deno);
	}
}
