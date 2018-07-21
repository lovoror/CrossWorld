using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiScopeControl : MonoBehaviour
{
	public Color c_normal;
	public Color c_targeted;
	public Rect boundary;
	public float minYL, minYR, minX, maxX;
	public float offset;
	public float ratio = 4;  // 放大倍率
	public float traceRate = 1.8f; // 作用于viewAngle, 发现目标后临时增大viewAnele,使目标不易脱离追踪。

	Vector2 preDirection = new Vector2(-1000, -1000);
	float[] boundaryAngles = new float[4];
	Vector2? lockPosition;  // 若指向操作区域，则camera需要固定在一个特定位置。
	Transform scope;
	Transform player;
	Camera scopeCamera;
	public float baseViewAngle = 25; // 1倍镜的可视角
	float viewAngle;  // 可视角
	float m_traceRate = 1f;  // 作用于viewAngle, 发现目标后临时增大viewAnele,使目标不易脱离追踪。
	float dst2View;  // 可视半径 / 距离
	float maxDistance;  // 狙击枪的最远瞄准距离
	float scopeRadius;  // 瞄目镜半径 单位像素
	float targetViewRadius;
	Vector3? targetPoint = null;

	void Awake()
	{
		
	}

	void Start ()
	{
		player = PlayerData.Instance.target;
		maxDistance = Constant.AimMaxDist[WeaponNameType.Sniper];
		scopeRadius = (transform as RectTransform).sizeDelta.x / 2;
		float angle1 = Mathf.Atan(boundary.height / boundary.width) * Mathf.Rad2Deg;
		boundaryAngles[0] = angle1;
		boundaryAngles[1] = 180f - angle1;
		boundaryAngles[2] = 180f + angle1;
		boundaryAngles[3] = 360f - angle1;
		InitScopeProperty();
	}

	void InitScopeProperty() {
		scope = GameObject.Find("ScopeCamera").transform;
		scopeCamera = scope.GetComponent<Camera>();
		viewAngle = baseViewAngle / ratio;
		float baseRadius = (transform as RectTransform).sizeDelta.x / 2; // 单位像素
		float baseViewRadius = baseRadius * (3.8f / 100);  // 单位米
		targetViewRadius = baseViewRadius * ratio;
		dst2View = Mathf.Tan(baseViewAngle / (2 * ratio) * Mathf.Deg2Rad);
	}

	void OnEnable()
	{
		PlayerController.ScopeCameraDirectionEvent += new PlayerController.ScopeCameraDirectionEventHandler(ScopeCameraDirectionEventFunc);
		lockPosition = null;
		m_traceRate = 1f;
	}

	void OnDisable()
	{
		PlayerController.ScopeCameraDirectionEvent -= ScopeCameraDirectionEventFunc;
		lockPosition = null;
	}


	void Update()
	{
		if (targetPoint != null) {
			scope.position = targetPoint.Value;
			scopeCamera.orthographicSize = targetViewRadius;
		}
	}

	void ScopeCameraDirectionEventFunc(Vector2 direction, Vector3? hitPoint, Transform target)
	{
		SetUiPosition(direction);
		SetScopeCamera(direction, hitPoint, target);
	}

	void SetUiPosition(Vector2 direction)
	{
		if (direction != preDirection) {
			preDirection = direction;
			float x, y;
			float angle = Utils.GetAnglePY2D(direction, new Vector2(1, 0));
			float rad = angle * Mathf.Deg2Rad;
			if (angle >= boundaryAngles[0] && angle < boundaryAngles[1]) {
				// 上边
				float S = Mathf.Sin(rad);
				float T = Mathf.Tan(rad);
				y = boundary.y + boundary.height;
				x = (y - boundary.center.y) / T + boundary.center.x;
				lockPosition = null;
				// 偏移
				x += offset / S;
				if (x > boundary.width + boundary.x) {
					x = boundary.width + boundary.x;
					// 计算y需要下降的距离
					float W = boundary.width;
					float H = boundary.height;
					float R = offset;
					float dy = ((0.5f * W + R) * T - 0.5f * H - R) * ((R + R / S) / (0.5f * W + R - (0.5f * H + R) / T) - 1) - R;
					y -= dy;
				}
			}
			else if (angle >= boundaryAngles[1] && angle < boundaryAngles[2]) {
				// 左边
				float T = Mathf.Tan(rad);
				float C = Mathf.Abs(Mathf.Cos(rad));
				float S = Mathf.Cos(rad);
				x = boundary.x;
				y = (x - boundary.center.x) * T + boundary.center.y;
				// 偏移
				y += offset / C;
				// 超出上边的情况
				if (y > boundary.y + boundary.height) {
					y = boundary.y + boundary.height;
					float Pi = Mathf.PI;
					float Tp = Mathf.Tan(rad - 0.5f * Pi);
					float Sp = Mathf.Sin(rad - 0.5f * Pi);
					float R = offset;
					float W = boundary.width;
					float H = boundary.height;
					float dx = ((0.5f * H + R) * Tp - 0.5f * W - R) * ((R + R / Sp) / (0.5f * H + R - (0.5f * W + R) / Tp) - 1) - R;
					x = boundary.x + dx;
				}
				// 需要lock的情况
				if (y < minYL) {
					if (lockPosition == null) {
						lockPosition = new Vector2(x, minYL);
					}
				}
				else {
					lockPosition = null;
				}
			}
			else if (angle >= boundaryAngles[2] && angle < boundaryAngles[3]) {
				// 下边
				float Pi = Mathf.PI;
				y = boundary.y;
				x = (y - boundary.center.y) / Mathf.Tan(rad) + boundary.center.x;
				// 偏移
				x += offset / Mathf.Sin(2 * Pi - rad);
				if (x < minX) {
					x = minX;
					if (lockPosition == null) {
						lockPosition = new Vector2(x, y);
					}
				}
				else if (x > maxX) {
					x = maxX;
					if (lockPosition == null) {
						lockPosition = new Vector2(x, y);
					}
				}
				else {
					lockPosition = null;
				}
			}
			else {
				// 右边
				float T = Mathf.Tan(rad);
				float C = Mathf.Cos(rad);
				float R = offset;
				float W = boundary.width;
				float H = boundary.height;
				x = boundary.x + boundary.width;
				y = (x - boundary.center.x) * T + boundary.center.y;
				// 是否解除锁定
				if (lockPosition != null) {
					if (y + offset / C > minYR) {
						lockPosition = null;
					}
				}
				// 偏移
				y -= offset / C;
				// 向下压线，则需要处理
				if (y - offset < minYR) {
					if (y + 2 * offset / C < boundary.y + boundary.height) {
						// 向上偏移不会过界则scope放上面
						y += 2 * offset / C;
						// 考虑lock的情况
						if (y < minYR) {
							// 若偏移后还小于，则锁定
							if (lockPosition == null) {
								lockPosition = new Vector2(x, minYR);
							}
						}
					}
					else {
						// 上下都容纳不下，就放最上面，并向左移动
						y = boundary.y + boundary.height;
						float dx = (R / C - H / 2) / T + W / 2;
						x -= dx;
					}
				}
				else {
					lockPosition = null;
				}
			}
			if (lockPosition == null) {
				(transform as RectTransform).position = new Vector2(x, y);
			}
			else {
				(transform as RectTransform).position = lockPosition.Value;
			}
		}
	}

	void SetScopeCamera(Vector2 direction, Vector3? hitPoint, Transform target)
	{
		if (target != null) {
			Vector3 P = target.position;
			targetPoint = new Vector3(P.x, 10, P.z);
			targetViewRadius = (P - player.position).magnitude * dst2View;
			m_traceRate = traceRate;
		}
		else {
			Transform enemy = null;
			float distance = maxDistance;
			float enemyDistance = maxDistance;
			float hitPointDistance = maxDistance;
			// 获得到墙体的距离
			if (hitPoint != null) {
				hitPointDistance = (hitPoint.Value - player.position).magnitude;
				//targetPoint = new Vector3(hitPoint.Value.x, 10, hitPoint.Value.z);
				//targetViewRadius = (hitPoint.Value - player.position).magnitude * dst2View;
			}
			// 获得到敌人的距离
			HasEnemyInRange(direction, ref enemy);
			if (enemy != null) {
				enemyDistance = (enemy.position - player.position).magnitude;
			}
			// 最近距离为实际距离
			distance = Mathf.Min(enemyDistance, hitPointDistance);
			// 设置相机位置
			Vector3 facePosition = player.forward;
			targetPoint = facePosition.normalized * distance + player.position;
			targetPoint = new Vector3(targetPoint.Value.x, 10, targetPoint.Value.z);
			targetViewRadius = distance * dst2View;
			// 是否跟踪
			if (enemyDistance < hitPointDistance) {
				m_traceRate = traceRate;
			}
			else {
				m_traceRate = 1;
			}
		}
	}

	bool HasEnemyInRange(Vector2 direction, ref Transform target)
	{
		float maxDist = 30;
		WeaponNameType curWeaponName = PlayerData.Instance.curWeaponName;
		if (Constant.AimMaxDist.ContainsKey(curWeaponName)) {
			maxDist = Constant.AimMaxDist[curWeaponName];
		}
		Transform player = PlayerData.Instance.target;
		LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");
		Collider[] hitColliders = null;
		hitColliders = Physics.OverlapSphere(player.position, maxDist, enemyLayerMask);
		// 检测是否被墙格挡 是否在瞄准范围内
		if (hitColliders != null) {
			// 从近到远排序
			if (hitColliders.Length > 1) {
				for (int i = 0; i < hitColliders.Length; i++) {
					for (int j = i + 1; j < hitColliders.Length; j++) {
						if ((hitColliders[i].transform.position - player.position).sqrMagnitude >
							(hitColliders[j].transform.position - player.position).sqrMagnitude) {
							Collider temp = hitColliders[i];
							hitColliders[i] = hitColliders[j];
							hitColliders[j] = temp;
						}
					}
				}
			}
			foreach (Collider collider in hitColliders) {
				Vector3 p2e = collider.transform.position - player.position;
				p2e.y = 0;
				Vector3 sLD3D = new Vector3(direction.x, 0, direction.y);
				float angle = Vector3.Angle(p2e, sLD3D);
				if (angle <= viewAngle / 2 * m_traceRate) {
					target = collider.transform;
					return true;
				}
			}
			// 脱离追踪区域则重置traceRate
			m_traceRate = 1;
		}
		return false;
	}
}
