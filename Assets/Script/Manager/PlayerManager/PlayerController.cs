using UnityEngine;
using System.Collections;

public class PlayerController : Controller {
	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	protected bool canControl;

	Vector3 moveDir; // 当前移动的方向
	LStick C_L = new LStick(); // 左摇杆
	SpriteRenderer bodyRender;

	[HideInInspector]
	PlayerManager I_PlayerManager;

	new void Awake()
	{
		base.Awake();
		I_PlayerManager = transform.GetComponent<PlayerManager>();
		bodyRender = body.GetComponent<SpriteRenderer>();
	}

	new void Start ()
	{
		base.Start();
		//speedTmp = speed;
		canControl = true;
	}

	protected new void OnEnable()
	{
		base.OnEnable();
		// PlayerMoveEvent
		MoboController.PlayerMoveEvent += new MoboController.PlayerMoveEventHandler(PlayerMoveEventFunc);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		MoboController.PlayerMoveEvent -= PlayerMoveEventFunc;
	}

	new void Update()
	{
		base.Update();
		bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		if (bodyAnimInfo.IsName("Attack")) {
			bodyAnim.speed = I_Manager.I_DataManager.attackSpeedRate;
		}
		else {
			bodyAnim.speed = 1;
		}
#if UNITY_EDITOR
		if (C_L.moveType == (int)MoveType.stop) {
			moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		}
#endif
	}

	void LateUpdate()
	{
		if (canControl) {
			// Move the player
			rb.velocity = Vector3.Lerp(rb.velocity, moveDir * speed, Time.fixedDeltaTime * moveSmooth);
			// 设置状态机
			ShowWalkAnim(rb.velocity.magnitude / speed);
			ShowAttackAnim(Input.GetButton("Fire1"));
			// 改变Leg的朝向
			leg.eulerAngles = new Vector3(-90, Utils.GetAnglePY(Vector3.forward, moveDir), -90);
			// 人物转向
#if UNITY_EDITOR
			Vector3 mouseV = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			mouseV.y = 0;
			//body.rotation = Quaternion.Euler(new Vector3(90, 0, GetAngle(Vector3.right, mouseV)));
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, mouseV), 0);
#elif UNITY_ANDROID

#endif
		}
	}

	/*--------------------- PlayerMoveEvent ---------------------*/
	void PlayerMoveEventFunc(LStick L)
	{
		C_L = L;
		moveDir = new Vector3(L.direction.x, 0, L.direction.y);
	}
	/*--------------------- PlayerMoveEvent ---------------------*/

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
}
