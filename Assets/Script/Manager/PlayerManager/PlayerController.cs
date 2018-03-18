//#define KEYBOARD_CONTROL

using UnityEngine;
using System.Collections;

public class PlayerController : Controller {

	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	protected bool canControl;
	float btnATouchedTime = 0;  // ButtonA按下的时间。
	Vector3 moveDir; // 当前移动的方向
	Vector3 moveDirPC; // WASD控制的移动方向
	Vector2 faceDirection; // 当前面朝的方向
	Transform curAimTarget = null;
	Vector2 stickLDirection { get; set; }
	float attackBoundary = 0.8f;  // 远程武器 开枪/转向 的边界距离
	float atkBoundaryRate = 1.7f;  // aming 状态下的attackBoundary增加系数
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
	WeaponNameType curWeaponName
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

	new void Start ()
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
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		MoboController.PlayerMoveEvent -= PlayerMoveEventFunc;
		MoboController.PlayerFaceEvent -= PlayerFaceEventFunc;
		MoboController.AttackDownEvent -= AttackDownEventFunc;
		MoboController.AttackUpEvent   -= AttackUpEventFunc;
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
			else if (curWeaponType == WeaponType.autoDistant || curWeaponType == WeaponType.melee) {
				if (attackType == AimAttackType.unknown) {
					if (btnATouchedTime >= aimAttackBoundaryTime) {
						attackType = AimAttackType.aming;
					}
					else {
						if (faceDirection.sqrMagnitude >= attackBoundary * attackBoundary) {
							attackType = AimAttackType.attacking;
							ShowAttackAnim(true);
						}
					}
				}
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
			Vector3 faceDirection3D = new Vector3(faceDirection.x, 0, faceDirection.y);
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection3D), 0);

			if (curWeaponType == WeaponType.singleLoader) {

			}
			else if (curWeaponType == WeaponType.melee || curWeaponType == WeaponType.autoDistant) {
				float trueAttackBoundary = attackBoundary;
				if (attackType == AimAttackType.aming) {
					trueAttackBoundary *= atkBoundaryRate;
				}
				if (faceDirection.sqrMagnitude > trueAttackBoundary * trueAttackBoundary) {
					// 攻击
					attackType = AimAttackType.attacking;
					ShowAttackAnim(true);
				}
			}
			SetAimTriangleAndCamera();
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
		attackType = AimAttackType.unknown;
		btnATouchedTime = 0;
	}
	/*--------------------- AttackDownEvent ---------------------*/

	/*--------------------- AttackUpEvent ---------------------*/
	void AttackUpEventFunc(float deltaTime)
	{
		ShowAttackAnim(false);
		stickLDirection = faceDirection;  // 抬手后需要朝当前faceDirection方向射击。
		if (curWeaponType == WeaponType.melee ||
			(curWeaponType == WeaponType.autoDistant && attackType == AimAttackType.unknown) ||
			(curWeaponType == WeaponType.singleLoader && (attackType == AimAttackType.aming || btnATouchedTime <= aimAttackBoundaryTime))) {
			// 抬手后单次射击
			AttackOnce();
		}
		// 狙击枪朝Enemy开枪后，镜头延迟归位
		if (curWeaponType == WeaponType.singleLoader && curAimTarget != null) {
			Invoke("DelayInitAimTarget", 0.5f);
		}
		else {
			curAimTarget = null;
		}
		attackType = AimAttackType.none;
		btnATouchedTime = 0;
		// 防止下次按下攻击后直接射击
		faceDirection = faceDirection.normalized * 0.1f;
		stickLDirection = stickLDirection.normalized * 0.1f;
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
	}
	/*------------ PlayerChangeWeaponEvent --------------*/


	bool HasEnemyInRange(ref Transform target)
	{
		float maxDist = 30;
		if (Constant.AimMaxDist.ContainsKey(curWeaponName)) {
			maxDist = Constant.AimMaxDist[curWeaponName];
		}
		Transform player = transform;
		LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");
		LayerMask ignoreLayerMask = LayerMask.GetMask("Wall");
		Collider[] hitColliders = null;
		hitColliders = Physics.OverlapSphere(player.position, maxDist, enemyLayerMask);
		// 检测是否被墙格挡 是否在瞄准范围内
		if (hitColliders != null) {
			foreach (Collider collider in hitColliders) {
				Vector3 p2e = collider.transform.position - player.position;
				p2e.y = 0;
				Vector3 sLD3D = new Vector3(stickLDirection.x, 0, stickLDirection.y);
				float aimDist = 0.01f;
				if(Constant.AimAssistDist.ContainsKey(curWeaponName)){
					aimDist = Constant.AimAssistDist[curWeaponName];
				}
				float a = p2e.magnitude, b = aimDist;
				float aimDegree = b / Mathf.Sqrt(a * a + b * b) * Mathf.Rad2Deg;
				float angle = Vector3.Angle(p2e, sLD3D);
				if (angle <= aimDegree) {
					//RaycastHit hit;
					bool isDead = collider.transform.GetComponent<Manager>().IsDead();
					if (!isDead && !Physics.Linecast(player.position, collider.transform.position, ignoreLayerMask)) {
						target = collider.transform;
						return true;
					}
				}
			}
		}
		target = null;
		return false;
	}

	void SetAimTriangleAndCamera()
	{
		SetAimTriangle();
		SetCamera();
	}

	// 设置瞄准三角
	void SetAimTriangle()
	{
		WeaponType curWeaponType = I_Manager.GetWeaponType();
		// 设置瞄准三角是否可见
		if ((attackType == AimAttackType.attacking || attackType == AimAttackType.aming) &&
			(curWeaponType == WeaponType.autoDistant || curWeaponType == WeaponType.singleLoader)) {
			I_AimController.SetVisible(true);
			// 设置瞄准三角大小
			HasEnemyInRange(ref curAimTarget);
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

	// 需要在SetAimTriangle之后
	void SetCamera()
	{
		if (curAimTarget != null) {
			// 设置Camera的aimPos
			Vector2 aimPos = new Vector2(curAimTarget.position.x, curAimTarget.position.z);
			I_FollowTarget.SetAimPos(aimPos);
		}
		else {
			I_FollowTarget.Reset();
			if (curWeaponType == WeaponType.autoDistant) {
				if (attackType == AimAttackType.aming || attackType == AimAttackType.attacking) {
					// autoDistant武器不允许根据BtnA的滑动距离改变Camera的offset
					// 以免划出attackBoundary触发攻击，造成误操作
					I_FollowTarget.SetAimDirection(faceDirection.normalized / 1.5f);
				}
			}
			else if (curWeaponType == WeaponType.singleLoader) {
				if (attackType != AimAttackType.none) {
					// singleLoader可以通过BtnA的滑动距离改变Camera的offset
					I_FollowTarget.SetAimDirection(faceDirection / 1.5f);
				}
			}
		}
	}
}
