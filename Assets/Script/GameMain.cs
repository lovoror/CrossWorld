using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

	void Awake()
	{
		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		GameStageMachine.Instance.Init ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void FixedUpdate()
	{
		GameStageMachine.Instance.FixedUpdate ();
	}

	void LateUpdate()
	{
		GameStageMachine.Instance.LateUpdate ();
	}
}
