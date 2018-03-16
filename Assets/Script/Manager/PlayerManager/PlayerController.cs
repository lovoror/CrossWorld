//#define KEYBOARD_CONTROL

using UnityEngine;
using System.Collections;

public class PlayerController : Controller {

	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	protected bool canControl;
	Vector3 moveDir; // 当前移动的方向
	Vector3 moveDirPC; // WASD控制的移动方向
	Vector2 faceDirection; // 当前面朝的方向
	Vector2 stickLDirection { get; set; }
	float attackBoundary = 0.8f;  // 远程武器 开枪/转向 的边界距离
	float atkBoundaryRate = 1.7f;  // aming 状态下的attackBoundary增加系数
	Rigidbody rb;
	SpriteRenderer bodyRender { get; set; }
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
		bodyRender = body.GetComponent<SpriteRenderer>();
		attackType = AimAttackType.none;
		rb = self.GetComponent<Rigidbody>();
	}

	new void Start ()
	{
		base.Start();
		canControl = true;

		// test
		// 设置瞄准三角是否可见
		//I_AimController.SetVisible(true);
		// test
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

	float btnATouchedTime = 0;  // ButtonA按下的时间。
	new void Update()
	{
		base.Update();
		if (attackType == AimAttackType.unknown) {
			btnATouchedTime += Time.deltaTime;
			if (btnATouchedTime >= aimAttackBoundaryTime) {
				attackType = AimAttackType.aming;
			}
			else if (faceDirection.sqrMagnitude >= attackBoundary * attackBoundary) {
				attackType = AimAttackType.attacking;
				ShowAttackAnim(true);
			}
		}
		else {
			btnATouchedTime = 0;
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
			WeaponType weaponType = I_Manager.GetWeaponType();

			float trueAttackBoundary = attackBoundary;
			if (weaponType == WeaponType.melee) {

			}
			else if (weaponType == WeaponType.autoDistant && attackType == AimAttackType.aming) {
				trueAttackBoundary *= atkBoundaryRate;
			}
			
			if ((attackType == AimAttackType.unknown || attackType == AimAttackType.aming) && 
				faceDirection.sqrMagnitude > trueAttackBoundary * trueAttackBoundary) {
				// 攻击
				attackType = AimAttackType.attacking;
				ShowAttackAnim(true);
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
	}
	/*--------------------- AttackDownEvent ---------------------*/

	/*--------------------- AttackUpEvent ---------------------*/
	void AttackUpEventFunc(float deltaTime)
	{
		ShowAttackAnim(false);
		WeaponType weaponType = I_Manager.GetWeaponType();
		if (weaponType == WeaponType.melee || weaponType == WeaponType.singleLoader ||
			(weaponType == WeaponType.autoDistant && attackType == AimAttackType.unknown)) {
			// 抬手后单次射击
			AttackOnce();
		}
		attackType = AimAttackType.none;
		// 防止下次按下攻击后直接射击
		faceDirection = faceDirection.normalized * 0.1f;
		stickLDirection = stickLDirection.normalized * 0.1f;
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
		float aimDegree = 20;
		float maxDist = 45;
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
				float angle = Vector3.Angle(p2e, sLD3D);
				if (angle <= aimDegree / 2) {
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

	// 设置瞄准三角
	void SetAimTriangleAndCamera()
	{
		WeaponType weaponType = I_Manager.GetWeaponType();
		// 设置瞄准三角是否可见
		if ((attackType == AimAttackType.attacking || attackType == AimAttackType.aming) &&
			(weaponType == WeaponType.autoDistant || weaponType == WeaponType.singleLoader)) {
			I_AimController.SetVisible(true);
			// 设置瞄准三角大小
			Transform target = null;
			HasEnemyInRange(ref target);
			if (target != null) {
				float distance = (transform.position - target.position).magnitude;
				Vector3 p2e = target.position - transform.position;
				Vector2 p2e2D = new Vector2(p2e.x, p2e.z);
				Vector3 direction3D = new Vector3(stickLDirection.x, 0, stickLDirection.y);
				float degree = Utils.GetAnglePY(p2e, direction3D);
				degree = degree > 180 ? degree - 360 : degree;
				I_AimController.UpdateAim(distance / 10, degree);
				faceDirection = p2e2D.normalized * stickLDirection.magnitude;
				// 设置Camera的Offset
				I_FollowTarget.SetOffset(p2e2D / 2);
				// 设置Camera的sizeScale
				float dZ = Mathf.Abs(target.position.z - transform.position.z);
				if (dZ >= 30) {
					I_FollowTarget.SetSizeScale(dZ / 30);
				}
			}
			else {
				faceDirection = stickLDirection;
				I_AimController.UpdateAim(4, 0);
				// 设置Camera的Offset
				float degree = Utils.GetAnglePY(new Vector3(faceDirection.x, 0, faceDirection.y), Vector3.right);
				float rate = faceDirection.magnitude / 1.5f;
				rate = rate < 1 ? rate : 1;
				I_FollowTarget.SetOffset(GetOffsetByAngle(degree)*rate);
			}
		}
		else {
			faceDirection = stickLDirection;
			I_AimController.SetVisible(false);
			I_FollowTarget.Reset();
		}
	}

	// 以Player为中心做一个椭圆，返回椭圆与Player面朝方向的焦点:
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
