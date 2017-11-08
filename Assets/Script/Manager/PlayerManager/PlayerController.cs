using UnityEngine;
using System.Collections;

public class PlayerController : Controller {
	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	private Vector3 moveDir; // 当前移动的方向
	private float speedTmp;

	new void Start ()
	{
		base.Start();
		speedTmp = speed;
		canControl = true;
	}

	void Update()
	{
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
			leg.eulerAngles = new Vector3(90, 0, GetAngle(Vector3.right, moveDir));
			// 人物转向
			Vector3 mouseV = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			mouseV.y = 0;
			//body.rotation = Quaternion.Euler(new Vector3(90, 0, GetAngle(Vector3.right, mouseV)));
			body.eulerAngles = new Vector3(90, 0, GetAngle(Vector3.right, mouseV));
		}
	}

	void FixedUpdate()
	{

	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}

	// 获得一个0-360度的夹角
	float GetAngle(Vector3 from, Vector3 to)
	{
		float angle = Vector3.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		if (normal.y > 0) {
			angle = 360 - angle;
		}
		return angle;
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
}
