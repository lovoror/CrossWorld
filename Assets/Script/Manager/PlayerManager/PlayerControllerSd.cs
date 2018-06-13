//#define KEYBOARD_CONTROL

using UnityEngine;
using System.Collections;

public class PlayerControllerSd : MonoBehaviour
{

	public GameObject obj;

	public float speed = 30;
	public Animator bodyAnim;
	public Animator legAnim;
	public float turnSpeed = 10;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	Vector2 moveDir; // 当前移动的方向
	Rigidbody rb;
	Transform leg;
	Transform body;
	float moveSpeedRate = 1;
	float turnSpeedRate = 1;

	void Awake()
	{
		rb = transform.GetComponent<Rigidbody>();
		leg = transform.Find("Leg");
		body = transform.Find("Body");
	}

	void Start()
	{
		
	}

	void OnEnable()
	{
		Reset();
		MoboControllerSd.PlayerAttackEvent += new MoboControllerSd.PlayerAttackEventHandler(PlayerAttackEventFunc);
		MoboControllerSd.PlayerMoveEvent += new MoboControllerSd.PlayerMoveEventHandler(PlayerMoveEventFunc);
	}

	void OnDisable()
	{
		MoboControllerSd.PlayerAttackEvent -= PlayerAttackEventFunc;
		MoboControllerSd.PlayerMoveEvent -= PlayerMoveEventFunc;
	}

	void Reset()
	{
		moveDir = Vector2.zero;
	}

	Vector3 faceDirection = Vector3.zero;
	void Update()
	{
		if (moveDir != Vector2.zero) {
			faceDirection = new Vector3(moveDir.x, 0, moveDir.y);
		}
		// 改变Leg的朝向
		Vector3 moveDir3D = new Vector3(moveDir.x, 0, moveDir.y);
		leg.eulerAngles = new Vector3(-90, Utils.GetAnglePY(Vector3.forward, moveDir3D), -90);
		// Body旋转
		if (faceDirection != Vector3.zero && !Utils.IsParallel(transform.forward, faceDirection)) {
			float deltaAngle = turnSpeed * turnSpeedRate * Time.deltaTime;
			float totalAngle = Utils.GetAnglePY(transform.forward, faceDirection);
			if (totalAngle < deltaAngle || totalAngle + deltaAngle >= 360) {
				transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection), 0);
			}
			else {
				if (totalAngle < 180) {
					// 逆时针旋转
					transform.Rotate(new Vector3(0, 1, 0), deltaAngle);
				}
				else {
					transform.Rotate(new Vector3(0, 1, 0), -deltaAngle);
				}
			}
		}
		// 移动同步
		// Move the player
		rb.velocity = Vector3.Lerp(rb.velocity, moveDir3D * speed * moveSpeedRate, Time.fixedDeltaTime * moveSmooth);
		ShowWalkAnim(rb.velocity.magnitude / speed);
	}

	void PlayerMoveEventFunc(Vector2 direction)
	{
		moveDir = direction;
		bodyAnim.SetBool("LButton", direction != Vector2.zero);
	}

	void PlayerAttackEventFunc(AttackSdType attackType)
	{
		switch (attackType) {
			/*-------- 点击 --------*/
			case AttackSdType.A:  // 轻攻击
				ResetTriggers();
				bodyAnim.SetTrigger("A");
				break;
			case AttackSdType.AU:
				ResetTriggers();
				bodyAnim.SetTrigger("AU");
				break;
			case AttackSdType.AD:
				ResetTriggers();
				bodyAnim.SetTrigger("AD");
				break;
			case AttackSdType.AL:
				ResetTriggers();
				bodyAnim.SetTrigger("AL");
				break;
			case AttackSdType.AR:
				ResetTriggers();
				bodyAnim.SetTrigger("AR");
				break;
			/*-------- 点击释放 --------*/
			case AttackSdType.ARN:
				ResetTriggers();
				bodyAnim.SetTrigger("ARN");
				break;
			case AttackSdType.ARU:
				ResetTriggers();
				bodyAnim.SetTrigger("ARU");
				break;
			case AttackSdType.ARD:  // 闪避
				ResetTriggers();
				bodyAnim.SetTrigger("ARD");
				break;
			case AttackSdType.ARL:
				ResetTriggers();
				bodyAnim.SetTrigger("ARL");
				break;
			case AttackSdType.ARR:  // 重攻击
				ResetTriggers();
				bodyAnim.SetTrigger("ARR");
				break;
			/*-------- 长按 --------*/
			case AttackSdType.HA:   // 蓄力
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				break;
			case AttackSdType.HAU:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAU", true);
				break;
			case AttackSdType.HAD:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAD", true);
				break;
			case AttackSdType.HAL:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAL", true);
				break;
			case AttackSdType.HAR:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAR", true);
				break;
			/*-------- 长按释放 --------*/
			case AttackSdType.HARN:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARN");
				break;
			case AttackSdType.HARU:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARU");
				break;
			case AttackSdType.HARD:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARD");
				break;
			case AttackSdType.HARL:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARL");
				break;
			case AttackSdType.HARR:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARR");
				break;
		}
	}

	public void ResetTriggers()
	{
		bodyAnim.ResetTrigger("A");
		bodyAnim.ResetTrigger("AU");
		bodyAnim.ResetTrigger("AD");
		bodyAnim.ResetTrigger("AL");
		bodyAnim.ResetTrigger("AR");
		bodyAnim.ResetTrigger("ARN");
		bodyAnim.ResetTrigger("ARU");
		bodyAnim.ResetTrigger("ARD");
		bodyAnim.ResetTrigger("ARL");
		bodyAnim.ResetTrigger("ARR");
		bodyAnim.ResetTrigger("HARN");
		bodyAnim.ResetTrigger("HARU");
		bodyAnim.ResetTrigger("HARD");
		bodyAnim.ResetTrigger("HARL");
		bodyAnim.ResetTrigger("HARR");
	}

	public void SetMoveSpeedRate(float moveSpeedRate)
	{
		this.moveSpeedRate = moveSpeedRate;
	}

	public void SetTurnSpeedRate(float turnSpeedRate)
	{
		this.turnSpeedRate = turnSpeedRate;
	}

	// 以左摇杆为指向翻滚，若左摇杆没触发，则向前翻滚。
	public void RollByLStick(float time, float distance)
	{
		// 停止播放脚部动作
		ShowWalkAnim(0);
		// 向左摇杆方向翻滚
		Vector3 faceDirection = transform.forward;
		if (moveDir != Vector2.zero) {
			faceDirection = new Vector3(moveDir.x, 0, moveDir.y);
		}
		transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection), 0);
		Vector3 tarPos = faceDirection.normalized * distance;
		iTween.MoveBy(transform.gameObject, iTween.Hash(
				"z", distance,
				"time", time, "EaseType", "linear"
			));
	}

	public void MoveForward(float time, float distance)
	{
		Vector3 faceDirection = transform.forward;
		transform.eulerAngles = new Vector3(0, Utils.GetAnglePY(Vector3.forward, faceDirection), 0);
		Vector3 tarPos = faceDirection.normalized * distance;
		iTween.MoveBy(transform.gameObject, iTween.Hash(
				"z", distance,
				"time", time, "EaseType", "linear"
			));
	}

	void ResetHAS()
	{
		bodyAnim.SetBool("HA", false);
		ResetHASWithoutHA();
	}

	void ResetHASWithoutHA()
	{
		bodyAnim.SetBool("HAU", false);
		bodyAnim.SetBool("HAD", false);
		bodyAnim.SetBool("HAL", false);
		bodyAnim.SetBool("HAR", false);
	}

	/*------------------ 状态机 ------------------*/
	void ShowWalkAnim(float speedRate)
	{
		legAnim.SetFloat("Speed", speedRate);
	}

	void StopWorking()
	{
		legAnim.SetFloat("Speed", 0);
	}
	/*------------------ 状态机 ------------------*/
}
