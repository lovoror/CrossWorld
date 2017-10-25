using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimEvents : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void ChangeAtkDir()
	{
		float yScale = transform.localScale.y;
		transform.localScale = new Vector3(1, -yScale, 1);
	}
}
