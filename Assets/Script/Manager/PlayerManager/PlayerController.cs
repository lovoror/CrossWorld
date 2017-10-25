using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;

	private Rigidbody2D rb2d;
	private Transform player;
	private Transform body;
	private Transform leg;
	private Animator legAnim;
	private Animator bodyAnim;
	private Camera camera;

	void Start ()
	{
		player = transform;
		rb2d = player.GetComponent<Rigidbody2D>();
		body = player.Find("Body");
		leg = player.Find("Leg");
		legAnim = leg.GetComponent<Animator>();
		bodyAnim = body.GetComponent<Animator>();
		camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	void Update()
	{

	}

	void FixedUpdate()
	{
		// Store Input and normalize vector for consistant speed on diagonals
		Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
		// Move the player
		rb2d.velocity = Vector2.Lerp(rb2d.velocity, moveDir * speed, Time.fixedDeltaTime * moveSmooth);
		// 设置状态机
		legAnim.SetBool("Walk", moveDir.magnitude > 0);
		bodyAnim.SetBool("Walk", moveDir.magnitude > 0);
		bodyAnim.SetBool("Attack", Input.GetButton("Fire1"));
		// 改变Leg的朝向
		leg.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngle(Vector3.right, moveDir)));
		// 人物转向
		Vector3 mouseV = camera.ScreenToWorldPoint(Input.mousePosition) - player.position;
		mouseV.z = 0;
		body.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngle(Vector3.right, mouseV)));
	}

	// 获得一个0-360度的夹角
	float GetAngle(Vector3 from, Vector3 to)
	{
		float angle = Vector3.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		if (normal.z < 0) {
			angle = 360 - angle;
		}
		return angle;
	}
}
