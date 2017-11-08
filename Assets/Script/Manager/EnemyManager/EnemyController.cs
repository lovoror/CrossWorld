using UnityEngine;
using System.Collections;

public class EnemyController : Controller {
	public float speed = 3;
	[Header("移动平滑度，越大越不平滑")]
	public float moveSmooth = 10;
	//private Vector2 moveDir; // 当前移动的方向

	void Update()
	{

	}

	void LateUpdate()
	{

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


	void OnCollisionEnter2D(Collision2D other)
	{
		rb.velocity = Vector3.zero;
	}
	void OnCollisionStay2D(Collision2D other)
	{
		rb.velocity = Vector3.zero;
	}
}
