using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackForward1 : MonoBehaviour
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
		if (transform.position.z <= -9) {
			rb.velocity = Vector3.forward * speed;
		}
		else if(transform.position.z >= 9){
			rb.velocity = Vector3.back * speed;
		}
	}

}
