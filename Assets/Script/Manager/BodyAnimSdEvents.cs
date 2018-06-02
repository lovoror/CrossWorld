using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimSdEvents : MonoBehaviour {
	AnimEventsManager I_AnimEventsManager;
	PlayerControllerSd I_Controller;
	void Awake()
	{
		I_AnimEventsManager = GetComponent<AnimEventsManager>();
		I_Controller = GetComponentInParent<PlayerControllerSd>();
	}

	void Start()
	{

	}
	
	void Update () {
		
	}

	void ResetTriggers()
	{
		I_Controller.ResetTriggers();
	}
}
