using UnityEngine;
using System.Collections;

public class PlayerController : Controller {
	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	private Vector3 moveDir; // 当前移动的方向
	private float speedTmp;

	[HideInInspector]
	PlayerManager I_PlayerManager;

	new void Awake()
	{
		base.Awake();
		I_PlayerManager = transform.GetComponent<PlayerManager>();
	}

	new void Start ()
	{
		base.Start();
		speedTmp = speed;
		canControl = true;
	}

	new void Update()
	{
		base.Update();
		// Store Input and normalize vector for consistant speed on diagonals
		moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
	}

	void LateUpdate()
	{
		if (canControl) {
			// Move the player
			rb.velocity = Vector3.Lerp(rb.velocity, moveDir * speed, Time.fixedDeltaTime * moveSmooth);
			// 设置状态机
			ShowWalkAnim(moveDir.magnitude > 0);
			ShowAttackAnim(Input.GetButton("Fire1"));
			// 改变Leg的朝向
			leg.eulerAngles = new Vector3(-90, Utils.GetAnglePY(Vector3.forward, moveDir), -90);
			// 人物转向
			Vector3 mouseV = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			mouseV.y = 0;
			//body.rotation = Quaternion.Euler(new Vector3(90, 0, GetAngle(Vector3.right, mouseV)));
			transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, mouseV), 0);
		}
	}

	void FixedUpdate()
	{

	}

	new void OnEnable()
	{
		base.OnEnable();
	}

	new void OnDisable()
	{
		base.OnDisable();
	}

	void OnCollisionEnter(Collision other)
	{
		string tag = other.transform.tag;
		if (Constant.TAGS.Attacker.Contains(tag)) {
			speed /= 1.8f;
		}
	}
	void OnCollisionStay(Collision other)
	{

	}
	void OnCollisionExit(Collision other)
	{
		string tag = other.transform.tag;
		if (Constant.TAGS.Attacker.Contains(tag)) {
			speed = speedTmp;
		}
	}

	/*--------------------- DeadNotifyEvent ---------------------*/
	protected override void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		base.DeadNotifyEventFunc(killer, dead);
		if (killer == transform) {
			speed = speedTmp;
		}
		if (dead == transform) {
			rb.velocity = Vector3.zero;
		}
	}
	/*--------------------- DeadNotifyEvent ---------------------*/

	/*----------------- WeaponNoiseEvent ------------------*/
	protected override void WeaponNoiseNotifyEventFunc(Transform source, float radius)
	{

	}
	/*----------------- WeaponNoiseEvent ------------------*/
}
