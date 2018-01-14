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
	bool isAttackBtnTouched = false;  // AttackButton是否按下
	bool isAttacking = false;  // 是否正在攻击
	float attackBoundary = 1f;  // 远程武器 开枪/转向 的边界距离
	SpriteRenderer bodyRender;
	PlayerManager I_PlayerManager;

	enum AimAttackType
	{
		none, unknown, aming, attacking  // 不瞄准不攻击（没有按住A键）,刚按下A键还没有确定是aim还是attack, 瞄准但不攻击, 攻击
	}
	AimAttackType attackType;
	float aimAttackBoundaryTime = 0.1f; // 当在aimAttackBoundaryTime内A的滑动距离超过attackBoundary则本次AimAttackType为attacking，否则为aming。

	new void Awake()
	{
		base.Awake();
		I_PlayerManager = transform.GetComponent<PlayerManager>();
		bodyRender = body.GetComponent<SpriteRenderer>();
		isAttackBtnTouched = false;
		isAttacking = false;
		attackType = AimAttackType.none;
	}

	new void Start ()
	{
		base.Start();
		canControl = true;
	}

	protected new void OnEnable()
	{
		base.OnEnable();
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
#if UNITY_EDITOR
			// 人物转向
			Vector3 mouseV = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			mouseV.y = 0;
			//body.rotation = Quaternion.Euler(new Vector3(90, 0, GetAngle(Vector3.right, mouseV)));
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, mouseV), 0);
			// 攻击状态
			ShowAttackAnim(Input.GetButton("Fire1"));
			isAttacking = true;
			if (moveDir.sqrMagnitude < 0.01) {
				trueMoveDir = moveDirPC;
			}
//#elif UNITY_ANDROID
			Vector3 faceDirection3D = new Vector3(faceDirection.x, 0, faceDirection.y);
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection3D), 0);
			WeaponType weaponType = I_Manager.GetWeaponType();
			if (weaponType == WeaponType.melee) {

			}
			else if (weaponType == WeaponType.autoDistant) {
				//if

				if (isAttackBtnTouched && faceDirection.sqrMagnitude > attackBoundary * attackBoundary) {
					// 攻击
					ShowAttackAnim(true);
					isAttacking = true;
				}
			}
#endif
			// Move the player
			rb.velocity = Vector3.Lerp(rb.velocity, trueMoveDir * speed, Time.fixedDeltaTime * moveSmooth);
			// 设置状态机
			ShowWalkAnim(rb.velocity.magnitude / speed);
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
		faceDirection = direction;
	}
	/*--------------------- PlayerFaceEvent ---------------------*/

	/*--------------------- AttackDownEvent ---------------------*/
	void AttackDownEventFunc(Vector2 position)
	{
		isAttackBtnTouched = true;
		attackType = AimAttackType.unknown;
	}
	/*--------------------- AttackDownEvent ---------------------*/

	/*--------------------- AttackUpEvent ---------------------*/
	void AttackUpEventFunc(float deltaTime)
	{
		isAttackBtnTouched = false;
		attackType = AimAttackType.none;
		ShowAttackAnim(false);
		WeaponType weaponType = I_Manager.GetWeaponType();
		if (weaponType == WeaponType.singleLoader ||
			(weaponType == WeaponType.autoDistant && !isAttacking && deltaTime < 0.1)) {
			// 抬手后单次射击
			AttackOnce();
		}
		isAttacking = false;
		// 防止下次按下攻击后直接射击
		faceDirection = faceDirection.normalized * (attackBoundary / 2);
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
	protected override void AttackSpeedChangeEventFunc(float rate)
	{
		base.AttackSpeedChangeEventFunc(rate);
		bodyAnim.SetFloat("AttackSpeed", rate);
	}
	/*-------------------- AttackSpeedChangeEvent ---------------------*/
}
