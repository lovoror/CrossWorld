//#define CONSTANT_SPEED  // 瞄准时镜头匀速移动
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	public float followSpeed = 8.0f;
	public float aimSpeed = 4.0f;   // 开启/关闭CONSTANT_SPEED，参考值分别为:20/2
	public float aimSpeedRateAccelerate = 1.5f; // aimSpeedRate的恢复速度
	float aimSpeedRate = 1f;  // aimSpeed的缩减程度。
	public float maxAimTime = 0.8f; // 开启CONSTANT_SPEED时有效，单次镜头移动的最长时间
	public float scaleSpeed = 2f;   // 关闭CONSTANT_SPEED时有效
	public Vector2 offset = Vector2.zero;
	[HideInInspector]
	public bool controlByOther = false; // 是否被外部脚本控制

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
	bool isPosAming = false;
	float lastPosAimingTime = 0;
	public float posAimingExitTime = 0.2f;

	// Use this for initialization
	void Start () {
		follow = GameObject.FindWithTag("Player").transform;
		mainCamera = GetComponent<Camera>();
		orgSize = mainCamera.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {

	}

	Vector3 preFollowPos = new Vector3(-1000, -1000, -1000);
	Vector3 curOffset = Vector3.zero;
	Vector3 tarOffset = Vector3.zero;   // 本次镜头移动的目标tarOffset点
	Vector3 orgOffset = Vector3.zero;   // 本次镜头移动的出事Offset点
	//int curTimes = 0;     // 本次镜头移动的次数
	//float orgSizeScale = 1;   // 初始放缩比例
	//float tarSizeScale = 1;   // 本次镜头变换的目标Scale
	//float curSizeScale = 1;
	//float deltaScale = 0; // 每次Lerp的递增比例
	public float boundary = 16;
	void FixedUpdate()
	{
		if (controlByOther) return;
		if (isPosAming) {
			lastPosAimingTime = Time.realtimeSinceStartup;
		}
		else {
			if (Time.realtimeSinceStartup < lastPosAimingTime + posAimingExitTime) {
				return;
			}
		}
		// 根据offset来同步Camera的位置，可实现跟随和瞄准/恢复不冲突。
		Vector3 offset3D = new Vector3(offset.x, 10, offset.y);
#if CONSTANT_SPEED
		if (tarOffset != offset3D || follow.position != preFollowPos) {
			// 本次镜头移动初始化
			preFollowPos = follow.position;
			orgOffset = new Vector3(transform.position.x - follow.position.x, 10, transform.position.z - follow.position.z);
			tarOffset = offset3D;
			orgSizeScale = mainCamera.orthographicSize / orgSize;
			tarSizeScale = sizeScale;
			curTimes = 1;
			float dist = (tarOffset - orgOffset).magnitude;
			deltaScale = Time.fixedDeltaTime * aimSpeed / dist;
			float maxMoveTimeScale = Time.fixedDeltaTime / maxAimTime;
			deltaScale = deltaScale > maxMoveTimeScale ? deltaScale : maxMoveTimeScale;
			curOffset = Vector3.Lerp(orgOffset, tarOffset, curTimes * deltaScale);
			curSizeScale = Mathf.Lerp(orgSizeScale, tarSizeScale, curTimes * deltaScale);
		}
		else if (tarOffset != curOffset) {
			curTimes += 1;
			if (curTimes * deltaScale >= 1) {
				curOffset = tarOffset;
				curSizeScale = tarSizeScale;
			}
			else {
				curOffset = Vector3.Lerp(orgOffset, tarOffset, curTimes * deltaScale);
				curSizeScale = Mathf.Lerp(orgSizeScale, tarSizeScale, curTimes * deltaScale);
			}
		}
		mainCamera.orthographicSize = curSizeScale * orgSize;
		transform.position = follow.position + curOffset;
#else
		Vector3 v = offset3D - curOffset;
		float speedAdjust = 1;
		if (v.sqrMagnitude < boundary * boundary) {
			speedAdjust = boundary / v.magnitude;
		}
		float trueAimSpeed = aimSpeed * aimSpeedRate;
		curOffset = Vector3.Lerp(curOffset, offset3D, Time.fixedDeltaTime * (trueAimSpeed / 10) * speedAdjust);  // 当前的offset点
		// 跟随curOffset点
		transform.position = Vector3.Lerp(transform.position, follow.position + curOffset, Time.fixedDeltaTime * followSpeed);

		if (mainCamera.orthographicSize != sizeScale * orgSize) {
			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, sizeScale * orgSize, Time.fixedDeltaTime * scaleSpeed * speedAdjust);
		}
#endif
	}

	public float maxX = 53.33f, maxY = 30;
	public void SetAimPos(Vector2 aimPos, float aimSpeedRate = 1)
	{
		this.aimSpeedRate = aimSpeedRate;
		this.aimPos = aimPos;
		isPosAming = true;
		if (aimPos == new Vector2(-1000, -1000)) {
			sizeScale = 1;
			offset = Vector2.zero;
			return;
		}
		Vector2 direction = aimPos - followPos;
		//offset = direction / 2;
		offset = Vector2.Lerp(followPos, aimPos, 12f / 30f);
		offset -= followPos; 
		float dX = Mathf.Abs(direction.x);
		float dY = Mathf.Abs(direction.y);
		sizeScale = Mathf.Max(dX / maxX, dY / maxY);
		sizeScale = sizeScale < 1 ? 1 : sizeScale;
	}

	public void SetAimDirection(Vector2 direction, float aimSpeedRate = 1)
	{
		this.aimSpeedRate = aimSpeedRate;
		if (aimPos == new Vector2(-1000, -1000)) {
			isPosAming = false;
			float degree = Utils.GetAnglePY(new Vector3(direction.x, 0, direction.y), Vector3.right);
			float rate = Mathf.Clamp01(direction.magnitude);
			WeaponNameType curWeaponName = PlayerData.Instance.curWeaponName;
			float weaponAimRate = curWeaponName == WeaponNameType.Sniper ? 1f : 0.8f;
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
		isPosAming = false;
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
