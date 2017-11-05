using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float speed = 30;

	private Vector3 direction;
	private Rigidbody rd;

	void Start ()
	{
		direction = transform.right;
		rd = transform.GetComponent<Rigidbody>();
		rd.velocity = direction * speed;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Wall") {
			Destroy(gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Wall") {
		}
	}
	
	void Update ()
	{
		
	}
}
