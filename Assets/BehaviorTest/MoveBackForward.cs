using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackForward : MonoBehaviour
{
	Rigidbody rb;
	public float speed = 5;
	void Start()
	{
		rb = transform.GetComponent<Rigidbody>();
		rb.velocity = transform.forward * speed;
	}

	void Update()
	{
		if (transform.position.x <= -9) {
			rb.velocity = Vector3.right * speed;
		}
		else if(transform.position.x >= 9){
			rb.velocity = Vector3.left * speed;
		}
	}
}
