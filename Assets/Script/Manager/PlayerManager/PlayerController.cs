//#define KEYBOARD_CONTROL

using UnityEngine;
using System.Collections;

public class PlayerController : Controller
{
	public delegate void ScopeCameraDirectionEventHandler(Vector2 direction, Vector3? hitPoint, Transform target = null);
	public static event ScopeCameraDirectionEventHandler ScopeCameraDirectionEvent;

	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;
	public Vector2 showScopeBoundary;  // 瞄点超出该范围则显示ScopeCamera

	protected bool canControl;
	bool isBtnADown = false;
	bool inRollState = false;  // 是否正在翻滚
	bool showScopeCamera = false; // 是否显示ScopeCamera
	float btnATouchedTime = 0;  // ButtonA按下的时间。
	Vector3 moveDir; // 当前移动的方向
	//Vector3 moveDirPC; // WASD控制的移动方向
	Vector2 faceDirection; // 当前面朝的方向 
	GameObject uiScope;
	float aimSpeedRateAccelerate
	{
		get
		{
			return I_FollowTarget.aimSpeedRateAccelerate;
		}
	}
	MeleeWeaponManager I_KnifeManager;
	Transform focusTarget = null;  // 当前锁定的目标
	Transform curAimTarget = null;
	Vector3? aimHitPoint;
	Vector2 stickLDirection { get; set; }
	float attackBoundary = 0.8f;  // 远程武器 开枪/转向 的边界距离
	float atkBoundaryRate = 1.7f;  // aming 状态下的attackBoundary增加系数
	bool knifeAttacking = false;  // Knife是否正在攻击
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
	//PlayerManager I_PlayerManager;
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
	float aimAttackBoundaryTime = 0.25f; // 当在aimAttackBoundaryTime内A的滑动距离超过attackBoundary则本次AimAttackType为attacking，否则为aming。

	new void Awake()
	{
		base.Awake();
		//I_PlayerManager = transform.GetComponent<PlayerManager>();
		I_AimController = transform.GetComponentInChildren<AimController>();
		I_FollowTarget = GameObject.FindWithTag("MainCamera").transform.GetComponent<FollowTarget>();
		I_KnifeManager = transform.Find("Weapons").Find("Knife").GetComponent<MeleeWeaponManager>();
		attackType = AimAttackType.none;
		rb = self.GetComponent<Rigidbody>();
		uiScope = GameObject.Find("Scope");
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
		// AttackEndEvent
		I_Manager.I_AnimEventsManager.AttackEndEvent += new AnimEventsManager.AttackEndEventHandler(AttackEndEventFunc);
		// RollEvent
		SkillButton.RollEvent += new SkillButton.RollEventHandler(RollEventFunc);
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
		I_Manager.I_AnimEventsManager.OnReloadEndEvent -= OnReloadEndEventFunc;
		I_Manager.I_AnimEventsManager.AttackEndEvent -= AttackEndEventFunc;
		SkillButton.RollEvent -= RollEventFunc;
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
		// 增加耐力
		if (!inRollState && !knifeAttacking) {
			float delta = Constant.strengthRestoreSpeed * Time.deltaTime;
			I_BaseData.ChangeCurStrength(delta);
		}
		if (!canControl) return;
		// 确定武器的攻击状态
		if (attackType != AimAttackType.none) {
			btnATouchedTime += Time.deltaTime;
			if (curWeaponType == WeaponType.singleLoader) {
				if (faceDirection.sqrMagnitude >= 0.64 * attackBoundary * attackBoundary) {
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
						if (faceDirection.sqrMagnitude >= 1.21f * attackBoundary * attackBoundary && leftBullets > 0) {
							attackType = AimAttackType.attacking;
							ShowAttackAnim(true);
						}
					}
				}
			}
			else if (curWeaponType == WeaponType.melee) {
				// 开启Knife攻击状态
				if (I_BaseData.curStrength > Constant.minKnifeAttackStrength * GlobalData.diffRate) {
					if (attackType == AimAttackType.aming) {
						bool hasEnemy = I_KnifeManager.GetEnemyInRange().Count > 0;
						if (hasEnemy || faceDirection.sqrMagnitude >= 1.21f * attackBoundary * attackBoundary) {
							attackType = AimAttackType.attacking;
							ShowAttackAnim(true);
							knifeAttacking = true;
						}
					}
				}
			}
		}
#if UNITY_EDITOR
		//float x = Input.GetAxisRaw("Horizontal");
		//float y = Input.GetAxisRaw("Vertical");
		//moveDirPC = new Vector3(x, 0, y).normalized;
#endif
	}

	void LateUpdate()
	{
		if (canControl) {
			Vector3 trueMoveDir = moveDir;
			if (trueMoveDir != Vector3.zero && inDelayInit) {
				// 在延迟归位过程中，移动Player，则退出延迟归位。
				DelayInitAimTarget();
			}
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
					if (curAimTarget != null) {
						// 如果当前有瞄准对象，则降低激发条件
						trueAttackBoundary *= 0.6f;
					}
					else {
						if (attackType == AimAttackType.aming ) {
							trueAttackBoundary *= atkBoundaryRate;
						}
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
			SetScopeCamera();
#endif
			// Move the player
			rb.velocity = Vector3.Lerp(rb.velocity, trueMoveDir * speed, Time.fixedDeltaTime * moveSmooth);
			// 始终以最快速度移动
			//rb.velocity = Vector3.Lerp(rb.velocity, trueMoveDir.normalized * speed, Time.fixedDeltaTime * moveSmooth);
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
		if (direction == Vector2.zero) {
			stickLDirection = stickLDirection.normalized * 0.01f;
		}
		else {
			stickLDirection = direction;
		}
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
			attackType = AimAttackType.aming;
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
	bool inDelayInit = false;  // 是否出在单发枪延迟归位过程中
	void AttackUpEventFunc(float deltaTime)
	{
		ShowAttackAnim(false);
		// RollState状态下不要重置方向
		if (!inRollState) {
			stickLDirection = faceDirection;  // 抬手后需要朝当前faceDirection方向射击。
		}

		// 单次攻击
		if (curWeaponType == WeaponType.melee) {
			if (attackType != AimAttackType.attacking && I_BaseData.curStrength >= Constant.minKnifeAttackStrength * GlobalData.diffRate) {
				AttackOnce();
			}
			knifeAttacking = false;
		}
		else if (curWeaponType == WeaponType.autoDistant) {
			if (leftBullets > 0) {
				if (focusTarget != null) {
					AttackOnce();
				}
				else {
					if (btnATouchedTime <= aimAttackBoundaryTime && attackType == AimAttackType.unknown) {
						// 瞬发需要辅助射击
						AttackOnceAssist();
						AttackOnce();
					}
				}
			}
		}
		else if (curWeaponType == WeaponType.singleLoader) {
			if (leftBullets > 0) {
				if (focusTarget != null) {
					AttackOnce();
				}
				else {
					if (btnATouchedTime <= aimAttackBoundaryTime) {
						// 瞬发需要辅助射击
						AttackOnceAssist();
						AttackOnce();
					}
					else if (attackType == AimAttackType.aming) {
						AttackOnce();
					}
				}
			}
		}

		// 单发枪朝Enemy开枪后，镜头延迟归位
		if (curWeaponType == WeaponType.singleLoader && curAimTarget != null) {
			Invoke("DelayInitAimTarget", 0.5f);
			inDelayInit = true;
		}
		else {
			//aimHitPoint = new Vector3(-1000, -1000, -1000);
			curAimTarget = null;
		}
		attackType = AimAttackType.none;
		btnATouchedTime = 0;
		isBtnADown = false;
		// 防止下次按下攻击后直接射击
		if (!inRollState) {
			faceDirection = faceDirection.normalized * 0.01f;
			stickLDirection = stickLDirection.normalized * 0.01f;
		}
	}
	void DelayInitAimTarget()
	{
		curAimTarget = null;
		inDelayInit = false;
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
			bodyRender.sortingOrder = 32766;
			I_AimController.SetVisible(false);
			RangeAimController I_RangeAimController = transform.GetComponentInChildren<RangeAimController>();
			if (I_RangeAimController != null) {
				I_RangeAimController.SetVisible(false);
			}
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
	WeaponNameType nextWeaponName = WeaponNameType.unknown;
	void PlayerChangeWeaponEventFunc(Transform player, WeaponNameType weaponName)
	{
		AnimatorStateInfo stateInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		if (!stateInfo.IsName("Base.Roll")) {
			PlayerData.Instance.curWeaponName = weaponName;
			ChangeWeapon();
		}
		else {
			// 记录下所要切换的武器，在RollEnd时进行切换
			nextWeaponName = weaponName;
		}
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

	/*--------------- AttackEndEvent -----------------*/
	void AttackEndEventFunc()
	{
		if (curWeaponName == WeaponNameType.Knife) {
			if (attackType == AimAttackType.attacking) {
				if (I_BaseData.curStrength > Constant.minKnifeAttackStrength * GlobalData.diffRate) {
					knifeAttacking = true;
				}
				else {
					// 关闭Knife攻击状态
					if (isBtnADown) {
						attackType = AimAttackType.aming;
					}
					else {
						attackType = AimAttackType.none;
					}
					knifeAttacking = false;
					ShowAttackAnim(false);
				}
			}
		}
	}
	/*--------------- AttackEndEvent -----------------*/

	/*--------------- RollEvent -----------------*/
	Vector2 rollDir = Vector2.zero;
	//Vector2 preFaceDir = Vector2.zero;

	void RollEventFunc(Vector2 dir)
	{
		if (I_BaseData.curStrength < Constant.minRollStrength * GlobalData.diffRate) {
			Utils.PlayInActiveSnd();
			return;
		}
		rollDir = dir;
		bodyAnim.SetTrigger("Roll");
		legAnim.SetTrigger("Roll");
	}

	public void OnRollStart()
	{
		canControl = false;
		curAimTarget = null;
		inRollState = true;
		// 取消播放Reload音效
		Transform weapon = I_BaseData.curWeaponTransform;
		Transform child = weapon.Find("SndReload");
		if(child != null){
			AudioSource audio = child.GetComponent<AudioSource>();
			if (audio.clip.name == "sndReloadClip" && audio.isPlaying) {
				audio.Stop();
			}
		}
		// 耐力扣除
		I_BaseData.ChangeCurStrength(-Constant.rollStrength * GlobalData.diffRate);
		// 隐藏瞄准三角
		I_AimController.SetVisible(false);
		// 取消enemyCollider与playerCollider的碰撞
		int enemyCollider = LayerMask.NameToLayer("Enemy");
		int playerCollider = LayerMask.NameToLayer("Player");
		Physics.IgnoreLayerCollision(playerCollider, enemyCollider);
		// 翻滚
		Vector2 faceDir;
		if (rollDir == Vector2.zero) {
			faceDir = moveDir == Vector3.zero ? new Vector2(transform.forward.x, transform.forward.z) :
				new Vector2(moveDir.x, moveDir.z);
		}
		else {
			faceDir = new Vector2(rollDir.x, rollDir.y);
		}
		transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, new Vector3(faceDir.x, 0, faceDir.y)), 0);
		rb.velocity = Vector3.zero;
		iTween.MoveBy(transform.gameObject, iTween.Hash(
				"z", 22,
				"time", 0.44, "EaseType", "linear"
			));
	}

	public void OnRollEnd() {
		canControl = true;
		rollDir = Vector2.zero;
		inRollState = false;
		// 改变Player朝向
		faceDirection = stickLDirection;
		// 重启enemyCollider与playerCollider的碰撞
		if (!I_Manager.IsDead()) {
			int enemyCollider = LayerMask.NameToLayer("Enemy");
			int playerCollider = LayerMask.NameToLayer("Player");
			Physics.IgnoreLayerCollision(playerCollider, enemyCollider, false);
		}
		// 是否需要切换武器
		if (nextWeaponName != WeaponNameType.unknown) {
			PlayerData.Instance.curWeaponName = nextWeaponName;
			nextWeaponName = WeaponNameType.unknown;
			ChangeWeapon();
		}
		// 是否需要Reload
		if (curWeaponType == WeaponType.autoDistant || curWeaponType == WeaponType.singleLoader) {
			int bullets = PlayerData.Instance.GetCurLeftBullets();
			if (bullets <= 0) {
				((DistantWeaponManager)I_Manager.I_WeaponManager).Reload();
			}
		}
	}
	/*--------------- RollEvent -----------------*/

	public void MeleeCostStrength()
	{
		I_BaseData.ChangeCurStrength(-Constant.knifeAttackStrength * GlobalData.diffRate);
	}

	public void ResetOnceAttack()
	{
		bodyAnim.ResetTrigger("OnceAttack");
	}

	bool HasEnemyInRange(ref Transform target, ref Vector3? hitPosition)
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
				Vector3 sLD3D = new Vector3(stickLDirection.x, 0, stickLDirection.y);
				float aimDist = 0.01f;
				if (Constant.AimAssistDist.ContainsKey(curWeaponName)) {
					aimDist = Constant.AimAssistDist[curWeaponName] / GlobalData.diffRate;
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
			aimHitPoint = null;
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
				else if (aimHitPoint != null) {
					float distance = (transform.position - aimHitPoint.Value).magnitude;
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
						I_FollowTarget.SetAimDirection(faceDirection / 1f, aimSpeedRate);
					}
				}
			}
		}
	}

	void SetScopeCamera()
	{
		if (curWeaponName == WeaponNameType.Sniper && attackType == AimAttackType.aming) {
			Vector3 po = transform.position;
			Vector2? direction = null;
			if (curAimTarget != null) {
				Vector3 pt = curAimTarget.position;
				direction = new Vector2(pt.x, pt.z) - new Vector2(po.x, po.z);
			}
			else if (aimHitPoint != null) {
				direction = new Vector2(aimHitPoint.Value.x, aimHitPoint.Value.z) - new Vector2(po.x, po.z);
			}

			showScopeCamera = false;
			if (direction == null) {
				// 瞄准最远方向
				showScopeCamera = true;
			}
			else if (Mathf.Abs(direction.Value.x) > showScopeBoundary.x ||
				Mathf.Abs(direction.Value.y) > showScopeBoundary.y) {
					showScopeCamera = true;
			}

			uiScope.SetActive(showScopeCamera);
			if (showScopeCamera && ScopeCameraDirectionEvent != null) {
				ScopeCameraDirectionEvent(faceDirection, aimHitPoint, curAimTarget);
			}
		}
		else {
			showScopeCamera = false;
			uiScope.SetActive(showScopeCamera);
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

	// 辅助瞄准，计算出相应结果，供之后调用的AdjustAttackDirection使用。
	Transform attackOnceTarget = null;
	void AttackOnceAssist()
	{
		attackOnceTarget = null;
		float maxDist = 30;
		if (Constant.OnceAttackMaxDist.ContainsKey(curWeaponName)) {
			maxDist = Constant.OnceAttackMaxDist[curWeaponName];
		}
		LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");
		LayerMask wallLayerMask = LayerMask.GetMask("Wall");
		Collider[] hitColliders = null;
		hitColliders = Physics.OverlapSphere(transform.position, maxDist, enemyLayerMask);
		// 检测是否被墙格挡 是否在瞄准范围内
		if (hitColliders != null) {
			foreach (Collider collider in hitColliders) {
				Vector3 p2e = collider.transform.position - transform.position;
				p2e.y = 0;
				Vector3 forward = transform.forward;
				float aimDist = 0.01f;
				if (Constant.OnceAttackAssistDist.ContainsKey(curWeaponName)) {
					aimDist = Constant.OnceAttackAssistDist[curWeaponName] / GlobalData.diffRate;
				}
				float a = p2e.magnitude, b = aimDist;
				float aimDegree = b / Mathf.Sqrt(a * a + b * b) * Mathf.Rad2Deg;
				float angle = Vector3.Angle(p2e, forward);
				if (angle <= aimDegree) {
					//RaycastHit hit;
					bool isDead = collider.transform.GetComponent<Manager>().IsDead();
					if (!isDead && !Physics.Linecast(transform.position, collider.transform.position, wallLayerMask)) {
						attackOnceTarget = collider.transform;
					}
				}
			}
		}
	}

	public void AdjustAttackDirection()
	{
		if (attackOnceTarget != null) {
			Vector3 p2e = attackOnceTarget.position - transform.position;
			Vector2 p2e2D = new Vector2(p2e.x, p2e.z);
			faceDirection = stickLDirection = p2e2D.normalized * 0.01f;
			Vector3 faceDirection3D = new Vector3(faceDirection.x, 0, faceDirection.y);
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection3D), 0);
			attackOnceTarget = null;
		}
	}
}
