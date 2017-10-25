using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	public float smooth = 4.0f;

	private Transform follow;

	// Use this for initialization
	void Start () {
		follow = GameObject.FindWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate()
	{
		Vector3 tarPosition = follow.position + new Vector3(0, 0, -5);
		transform.position = Vector3.Lerp(transform.position, tarPosition, Time.fixedDeltaTime * smooth);
	}
}
