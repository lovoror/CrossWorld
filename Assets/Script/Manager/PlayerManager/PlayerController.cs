//#define KEYBOARD_CONTROL

using UnityEngine;
using System.Collections;

public class PlayerController : Controller
{
	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	protected bool canControl;
	bool isBtnADown = false;
	float btnATouchedTime = 0;  // ButtonA按下的时间。
	Vector3 moveDir; // 当前移动的方向
	Vector3 moveDirPC; // WASD控制的移动方向
	Vector2 faceDirection; // 当前面朝的方向
	float aimSpeedRateAccelerate
	{
		get
		{
			return I_FollowTarget.aimSpeedRateAccelerate;
		}
	}
	Transform focusTarget = null;  // 当前锁定的目标
	Transform curAimTarget = null;
	Vector3 aimHitPoint = new Vector3(-1000, -1000, -1000);
	Vector2 stickLDirection { get; set; }
	float attackBoundary = 0.8f;  // 远程武器 开枪/转向 的边界距离
	float atkBoundaryRate = 1.7f;  // aming 状态下的attackBoundary增加系数
	int leftBullets
	{
		get
		{
			return I_BaseData.GetCurLeftBullets();
		}
	}
	Rigidbody rb;
	SpriteRenderer bodyRender
	{
		get
		{
			return body.GetComponent<SpriteRenderer>();
		}
	}
	PlayerManager I_PlayerManager;
	AimController I_AimController;
	FollowTarget I_FollowTarget;
	new WeaponNameType curWeaponName
	{
		get
		{
			return PlayerData.Instance.curWeaponName;
		}
	}
	WeaponType curWeaponType
	{
		get
		{
			return PlayerData.Instance.curWeaponType;
		}
	}
	protected float attackSpeedRate
	{
		get
		{
			float speed = I_Manager.I_DataManager.attackSpeedRate;
			return speed >= 0 ? speed : 1;
		}
	}

	enum AimAttackType
	{
		none, unknown, aming, attacking  // 不瞄准不攻击（没有按住A键）,刚按下A键还没有确定是aim还是attack, 瞄准但不攻击, 攻击
	}
	AimAttackType attackType;
	float aimAttackBoundaryTime = 0.2f; // 当在aimAttackBoundaryTime内A的滑动距离超过attackBoundary则本次AimAttackType为attacking，否则为aming。

	new void Awake()
	{
		base.Awake();
		I_PlayerManager = transform.GetComponent<PlayerManager>();
		I_AimController = transform.GetComponentInChildren<AimController>();
		I_FollowTarget = GameObject.FindWithTag("MainCamera").transform.GetComponent<FollowTarget>();
		attackType = AimAttackType.none;
		rb = self.GetComponent<Rigidbody>();
	}

	new void Start()
	{
		base.Start();
		canControl = true;
	}

	protected new void OnEnable()
	{
		base.OnEnable();
		Reset();
		// PlayerMoveEvent
		MoboController.PlayerMoveEvent += new MoboController.PlayerMoveEventHandler(PlayerMoveEventFunc);
		// PlayerFaceEvent
		MoboController.PlayerFaceEvent += new MoboController.PlayerFaceEventHandler(PlayerFaceEventFunc);
		// AttackDownEvent
		MoboController.AttackDownEvent += new MoboController.AttackDownEventHandler(AttackDownEventFunc);
		// AttackUpEvent
		MoboController.AttackUpEvent += new MoboController.AttackUpEventHandler(AttackUpEventFunc);
		// PlayerChangeWeaponEvent
		FuncRButton.PlayerChangeWeaponEvent += new FuncRButton.PlayerChangeWeaponEventHandler(PlayerChangeWeaponEventFunc);
		// PlayerReloadWeaponEvent
		FuncRButton.PlayerReloadEvent += new FuncRButton.PlayerReloadEventHandler(PlayerReloadWeaponEventFunc);
		// OnReloadEndEvent
		I_Manager.I_AnimEventsManager.OnReloadEndEvent += new AnimEventsManager.OnReloadEndEventHandler(OnReloadEndEventFunc);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		MoboController.PlayerMoveEvent -= PlayerMoveEventFunc;
		MoboController.PlayerFaceEvent -= PlayerFaceEventFunc;
		MoboController.AttackDownEvent -= AttackDownEventFunc;
		MoboController.AttackUpEvent -= AttackUpEventFunc;
		FuncRButton.PlayerChangeWeaponEvent -= PlayerChangeWeaponEventFunc;
		FuncRButton.PlayerReloadEvent -= PlayerReloadWeaponEventFunc;
	}

	void Reset()
	{
		attackType = AimAttackType.none;
		moveDir = Vector3.zero;
		faceDirection = transform.forward;
	}

	new void Update()
	{
		base.Update();
		// 确定武器的攻击状态
		if (attackType != AimAttackType.none) {
			btnATouchedTime += Time.deltaTime;
			if (curWeaponType == WeaponType.singleLoader) {
				if (faceDirection.sqrMagnitude >= attackBoundary * attackBoundary) {
					attackType = AimAttackType.aming;
				}
				else {
					attackType = AimAttackType.unknown;
				}
			}
			else if (curWeaponType == WeaponType.autoDistant) {
				if (attackType == AimAttackType.unknown) {
					if (btnATouchedTime >= aimAttackBoundaryTime) {
						attackType = AimAttackType.aming;
					}
					else {
						if (faceDirection.sqrMagnitude >= attackBoundary * attackBoundary && leftBullets > 0) {
							attackType = AimAttackType.attacking;
							ShowAttackAnim(true);
						}
					}
				}
			}
			else if (curWeaponType == WeaponType.melee) {

			}
		}
#if UNITY_EDITOR
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		moveDirPC = new Vector3(x, 0, y).normalized;
#endif
	}

	void LateUpdate()
	{
		if (canControl) {
			Vector3 trueMoveDir = moveDir;
#if KEYBOARD_CONTROL
			// 人物转向
			Vector3 mouseV = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			mouseV.y = 0;
			//body.rotation = Quaternion.Euler(new Vector3(90, 0, GetAngle(Vector3.right, mouseV)));
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, mouseV), 0);
			// 攻击状态
			ShowAttackAnim(Input.GetButton("Fire1"));
			if (moveDir.sqrMagnitude < 0.01) {
				trueMoveDir = moveDirPC;
			}
#else
			// Player 面朝方向
			if (focusTarget) {
				Vector3 player2Target = focusTarget.position - transform.position;
				player2Target.y = 0;
				transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, player2Target), 0);
			}
			else {
				Vector3 faceDirection3D = new Vector3(faceDirection.x, 0, faceDirection.y);
				transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection3D), 0);
			}

			// 攻击状态
			if (focusTarget) {
				if (curWeaponType == WeaponType.autoDistant) {
					if (attackType != AimAttackType.none) {
						if (leftBullets > 0) {
							// 攻击
							attackType = AimAttackType.attacking;
							ShowAttackAnim(true);
						}
					}
				}
			}
			else {
				if (curWeaponType == WeaponType.singleLoader) {

				}
				else if (curWeaponType == WeaponType.melee) {

				}
				else if (curWeaponType == WeaponType.autoDistant) {
					float trueAttackBoundary = attackBoundary;
					if (attackType == AimAttackType.aming) {
						trueAttackBoundary *= atkBoundaryRate;
					}
					if (faceDirection.sqrMagnitude > trueAttackBoundary * trueAttackBoundary && leftBullets > 0) {
						// 攻击
						attackType = AimAttackType.attacking;
						ShowAttackAnim(true);
					}
				}
			}
			SetRangeAngle();
			SetAimTriangle();
			SetCamera();
#endif
			// Move the player
			rb.velocity = Vector3.Lerp(rb.velocity, trueMoveDir * speed, Time.fixedDeltaTime * moveSmooth);
			// 设置状态机
			ShowWalkAnim(rb.velocity.magnitude / speed);
			ChangeAttackSpeed();
			// 改变Leg的朝向
			leg.eulerAngles = new Vector3(-90, Utils.GetAnglePY(Vector3.forward, trueMoveDir), -90);
		}
	}

	/*--------------------- PlayerMoveEvent ---------------------*/
	void PlayerMoveEventFunc(Vector2 dir)
	{
		moveDir = new Vector3(dir.x, 0, dir.y);
	}
	/*--------------------- PlayerMoveEvent ---------------------*/

	/*--------------------- PlayerFaceEvent ---------------------*/
	void PlayerFaceEventFunc(Vector2 direction)
	{
		stickLDirection = direction;
	}
	/*--------------------- PlayerFaceEvent ---------------------*/

	/*--------------------- AttackDownEvent ---------------------*/
	void AttackDownEventFunc(Vector2 position)
	{
		isBtnADown = true;
		// Reload
		if (curWeaponType == WeaponType.autoDistant || curWeaponType == WeaponType.singleLoader) {
			if (leftBullets < 1) {
				DistantWeaponManager dstWeaponManager = (DistantWeaponManager)I_Manager.I_WeaponManager;
				MyDelegate.vfv myCallback = new MyDelegate.vfv(ReloadCallback);
				dstWeaponManager.Reload(myCallback);
			}
			else {
				attackType = AimAttackType.unknown;
				btnATouchedTime = 0;
			}
		}
		else if (curWeaponType == WeaponType.melee) {
			attackType = AimAttackType.attacking;
			ShowAttackAnim(true);
			btnATouchedTime = 0;
		}
	}
	void ReloadCallback()
	{
		if (isBtnADown) {
			attackType = AimAttackType.unknown;
			btnATouchedTime = 0;
		}
	}
	/*--------------------- AttackDownEvent ---------------------*/

	/*--------------------- AttackUpEvent ---------------------*/
	void AttackUpEventFunc(float deltaTime)
	{
		ShowAttackAnim(false);
		stickLDirection = faceDirection;  // 抬手后需要朝当前faceDirection方向射击。
		if (curWeaponType == WeaponType.melee ||
			(curWeaponType == WeaponType.autoDistant && attackType == AimAttackType.unknown) ||
			(curWeaponType == WeaponType.singleLoader && (focusTarget != null || (attackType != AimAttackType.none && (attackType == AimAttackType.aming || btnATouchedTime <= aimAttackBoundaryTime))))) {
			// 抬手后单次射击
			if (leftBullets > 0) {
				AttackOnce();
			}
		}
		// 狙击枪朝Enemy或Wall开枪后，镜头延迟归位
		if (curWeaponType == WeaponType.singleLoader && curAimTarget != null) {
			Invoke("DelayInitAimTarget", 0.5f);
		}
		else {
			//aimHitPoint = new Vector3(-1000, -1000, -1000);
			curAimTarget = null;
		}
		attackType = AimAttackType.none;
		btnATouchedTime = 0;
		isBtnADown = false;
		// 防止下次按下攻击后直接射击
		faceDirection = faceDirection.normalized * 0.0001f;
		stickLDirection = stickLDirection.normalized * 0.0001f;
	}
	void DelayInitAimTarget()
	{
		curAimTarget = null;
	}
	/*--------------------- AttackDownEvent ---------------------*/


	/*--------------------- DeadNotifyEvent ---------------------*/
	protected override void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		base.DeadNotifyEventFunc(killer, dead);
		if (killer == transform) {
			//speed = speedTmp;
		}
		if (dead == transform) {
			canControl = false;
			int enemyCollider = LayerMask.NameToLayer("Enemy");
			int playerCollider = LayerMask.NameToLayer("Player");
			Physics.IgnoreLayerCollision(playerCollider, enemyCollider);
			bodyRender.sortingLayerName = "Default";
			I_AimController.SetVisible(false);
			if (rb) {
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		}
	}
	/*--------------------- DeadNotifyEvent ---------------------*/

	/*-------------------- AttackSpeedChangeEvent ---------------------*/
	public void ChangeAttackSpeed()
	{
		bodyAnim.SetFloat("AttackSpeed", attackSpeedRate);
	}
	/*-------------------- AttackSpeedChangeEvent ---------------------*/

	/*------------ PlayerChangeWeaponEvent --------------*/
	void PlayerChangeWeaponEventFunc(Transform player, WeaponNameType weaponName)
	{
		PlayerData.Instance.curWeaponName = weaponName;
		ChangeWeapon();
	}
	public void ChangeWeapon()
	{
		var d_bodys = PlayerData.Instance.d_Bodys;
		var d_weapons = PlayerData.Instance.d_Weapons;
		foreach (var body in d_bodys) {
			body.Value.gameObject.SetActive(body.Key == curWeaponName);
		}
		foreach (var weapon in d_weapons) {
			weapon.Value.gameObject.SetActive(weapon.Key == curWeaponName);
		}
		// 弹夹空了则Reload
		Invoke("DelayReload", 0.5f);
	}
	void DelayReload()
	{
		if (curWeaponType == WeaponType.singleLoader || curWeaponType == WeaponType.autoDistant) {
			int bullets = PlayerData.Instance.GetCurLeftBullets();
			if (bullets <= 0) {
				((DistantWeaponManager)I_Manager.I_WeaponManager).Reload();
			}
		}
	}
	/*------------ PlayerChangeWeaponEvent --------------*/

	/*------------ PlayerReloadWeaponEvent --------------*/
	void PlayerReloadWeaponEventFunc(Transform player)
	{
		if (transform != player) return;
		if (curWeaponType == WeaponType.singleLoader || curWeaponType == WeaponType.autoDistant) {
			int bullets = PlayerData.Instance.GetCurLeftBullets();
			int magazineSize = 0;
			if (Constant.MagazineSize.ContainsKey(curWeaponName)) {
				magazineSize = Constant.MagazineSize[curWeaponName];
			}
			if (bullets < magazineSize) {
				((DistantWeaponManager)I_Manager.I_WeaponManager).Reload();
			}
		}
	}

	/*------------ PlayerReloadWeaponEvent --------------*/

	/*--------------- OnReloadEndEvent -----------------*/
	void OnReloadEndEventFunc()
	{
	}
	/*--------------- OnReloadEndEvent -----------------*/

	public void ResetOnceAttack()
	{
		bodyAnim.ResetTrigger("OnceAttack");
	}

	bool HasEnemyInRange(ref Transform target, ref Vector3 hitPosition)
	{
		float maxDist = 30;
		if (Constant.AimMaxDist.ContainsKey(curWeaponName)) {
			maxDist = Constant.AimMaxDist[curWeaponName];
		}
		Transform player = transform;
		LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");
		LayerMask wallLayerMask = LayerMask.GetMask("Wall");
		Collider[] hitColliders = null;
		hitColliders = Physics.OverlapSphere(player.position, maxDist, enemyLayerMask);
		// 检测是否被墙格挡 是否在瞄准范围内
		if (hitColliders != null) {
			foreach (Collider collider in hitColliders) {
				Vector3 p2e = collider.transform.position - player.position;
				p2e.y = 0;
				Vector3 sLD3D = new Vector3(stickLDirection.x, 0, stickLDirection.y);
				float aimDist = 0.01f;
				if (Constant.AimAssistDist.ContainsKey(curWeaponName)) {
					aimDist = Constant.AimAssistDist[curWeaponName];
				}
				float a = p2e.magnitude, b = aimDist;
				float aimDegree = b / Mathf.Sqrt(a * a + b * b) * Mathf.Rad2Deg;
				float angle = Vector3.Angle(p2e, sLD3D);
				if (angle <= aimDegree) {
					//RaycastHit hit;
					bool isDead = collider.transform.GetComponent<Manager>().IsDead();
					if (!isDead && !Physics.Linecast(player.position, collider.transform.position, wallLayerMask)) {
						target = collider.transform;
						return true;
					}
				}
			}
		}
		// 返回瞄准线与墙的交接点
		Ray ray = new Ray();
		ray.origin = player.position;
		ray.direction = player.forward;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, maxDist, wallLayerMask)) {
			hitPosition = hitInfo.point;
		}
		target = null;
		return false;
	}

	// Shotgun范围三角
	void SetRangeAngle()
	{
		if (curWeaponName == WeaponNameType.Shotgun &&
			(attackType == AimAttackType.attacking || 
			attackType == AimAttackType.aming)) {
			if (ShowRangeTriangleEvent != null) {
				ShowRangeTriangleEvent(WeaponNameType.Shotgun, true);
			}
		}
		else {
			if (ShowRangeTriangleEvent != null) {
				ShowRangeTriangleEvent(WeaponNameType.Shotgun, false);
			}
		}
	}

	// 设置瞄准三角
	void SetAimTriangle()
	{
		WeaponType curWeaponType = I_Manager.GetWeaponType();
		if (focusTarget != null) {
			if (curWeaponType == WeaponType.autoDistant || curWeaponType == WeaponType.singleLoader) {
				float distance = (transform.position - focusTarget.position).magnitude;
				I_AimController.UpdateAim(distance / 10, 0);
				I_AimController.SetVisible(true);
			}
			else {
				I_AimController.SetVisible(false);
			}
		}
		else {
			aimHitPoint = new Vector3(-1000, -1000, -1000);
			// 设置瞄准三角是否可见
			if ((attackType == AimAttackType.attacking || attackType == AimAttackType.aming) &&
				(curWeaponType == WeaponType.autoDistant || curWeaponType == WeaponType.singleLoader)) {
				AnimatorStateInfo stateInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
				I_AimController.SetVisible(!stateInfo.IsName("Base.Reload"));
				// 设置瞄准三角大小
				HasEnemyInRange(ref curAimTarget, ref aimHitPoint);
				if (curAimTarget != null) {
					float distance = (transform.position - curAimTarget.position).magnitude;
					Vector3 p2e = curAimTarget.position - transform.position;
					Vector2 p2e2D = new Vector2(p2e.x, p2e.z);
					Vector3 direction3D = new Vector3(stickLDirection.x, 0, stickLDirection.y);
					float degree = Utils.GetAnglePY(p2e, direction3D);
					degree = degree > 180 ? degree - 360 : degree;
					I_AimController.UpdateAim(distance / 10, degree);
					faceDirection = p2e2D.normalized * stickLDirection.magnitude;
				}
				else if (aimHitPoint != new Vector3(-1000, -1000, -1000)) {
					float distance = (transform.position - aimHitPoint).magnitude;
					I_AimController.UpdateAim(distance / 10, 0);
					faceDirection = stickLDirection;
				}
				else {
					faceDirection = stickLDirection;
					float aimLangth = 3;
					if (Constant.AimMaxDist.ContainsKey(curWeaponName)) {
						aimLangth = Constant.AimMaxDist[curWeaponName] / 10;
					}
					I_AimController.UpdateAim(aimLangth, 0);
				}
			}
			else {
				faceDirection = stickLDirection;
				I_AimController.SetVisible(false);
			}
		}
	}

	// 需要在SetAimTriangle之后
	// 在瞄准离开curAimTarget之后的smoothTime时间内，镜头需要缓动。
	float aimSpeedRate = 0;  // AimSpeed衰减程度
	Transform preAimTarget = null; // 前一次瞄准的对象（不考虑focusTarget的情况）。
	void SetCamera()
	{
		if (focusTarget) {
			Vector2 targetPos2D = new Vector2(focusTarget.position.x, focusTarget.position.z);
			I_FollowTarget.SetAimPos(targetPos2D);
			preAimTarget = null;
			aimSpeedRate = 1;  // 恢复瞄准速度
		}
		else {
			if (curAimTarget != null) {
				if (preAimTarget == null) {
					// 之前没有瞄准对象，则更新preAimTarget为当前对象
					preAimTarget = curAimTarget;
				}
				else {
					// 第一次发现preAimTarget不再是瞄准对象的时候，镜头开始缓动。
					if (preAimTarget != curAimTarget) {
						preAimTarget = curAimTarget;
						aimSpeedRate = 0;
					}
				}
				// 设置Camera的aimPos
				Vector2 aimPos = new Vector2(curAimTarget.position.x, curAimTarget.position.z);
				//aimSpeedRate = Mathf.Lerp(aimSpeedRate, 1, aimSpeedRateAccelerate * Time.deltaTime);
				aimSpeedRate = aimSpeedRate >= 1 ? 1 : aimSpeedRate + aimSpeedRateAccelerate * Time.deltaTime;
				I_FollowTarget.SetAimPos(aimPos, aimSpeedRate);
			}
			//else if (aimHitPoint != new Vector3(-1000, -1000, -1000)) {
			//	// 瞄准墙体
			//	Vector2 aimPos = new Vector2(aimHitPoint.x, aimHitPoint.z);
			//	I_FollowTarget.SetAimPos(aimPos);
			//}
			else {
				if (preAimTarget != null) {
					// 上一次有瞄准对象，当前没有瞄准对象，镜头开始缓动。
					preAimTarget = null;
					aimSpeedRate = 0;
				}
				//aimSpeedRate = Mathf.Lerp(aimSpeedRate, 1, aimSpeedRateSmooth * Time.deltaTime);
				aimSpeedRate = aimSpeedRate >= 1 ? 1 : aimSpeedRate + aimSpeedRateAccelerate * Time.deltaTime;
				I_FollowTarget.Reset();
				if (curWeaponType == WeaponType.autoDistant) {
					if (attackType == AimAttackType.aming || attackType == AimAttackType.attacking) {
						// autoDistant武器不允许根据BtnA的滑动距离改变Camera的offset
						// 以免划出attackBoundary触发攻击，造成误操作
						I_FollowTarget.SetAimDirection(faceDirection.normalized / 1.5f, aimSpeedRate);
					}
				}
				else if (curWeaponType == WeaponType.singleLoader) {
					if (attackType != AimAttackType.none) {
						// singleLoader可以通过BtnA的滑动距离改变Camera的offset
						I_FollowTarget.SetAimDirection(faceDirection / 1.5f, aimSpeedRate);
					}
				}
			}
		}
	}

	// 取消、锁定目标
	public void SetFocusTarget(Transform target)
	{
		// 取消目标锁定时，设置faceDirection
		if (focusTarget != null && target == null) {
			Vector3 player2Target = focusTarget.position - transform.position;
			faceDirection = new Vector2(player2Target.x, player2Target.z).normalized * 0.0001f;
			stickLDirection = faceDirection;
		}
		focusTarget = target;
	}

	public Transform GetFocusTarget()
	{
		return focusTarget;
	}
}
