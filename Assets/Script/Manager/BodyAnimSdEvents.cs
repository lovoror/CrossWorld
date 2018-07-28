using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class BodyAnimSdEvents : MonoBehaviour {
	//AnimEventsManager I_AnimEventsManager;
	PlayerControllerSd I_Controller;
	void Awake()
	{
		//I_AnimEventsManager = GetComponent<AnimEventsManager>();
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

	void SetMoveSpeedRate(float moveSpeedRate)
	{
		I_Controller.SetMoveSpeedRate(moveSpeedRate);
	}

	void SetTurnSpeedRate(float turnSpeedRate)
	{
		I_Controller.SetTurnSpeedRate(turnSpeedRate);
	}

	// 以左摇杆为指向翻滚，若左摇杆没触发，则向前翻滚。
	void RollByLStick(string args)
	{
		JsonData data = JsonMapper.ToObject(args);
		float time = (float)data["time"];
		float distance = (float)data["distance"];
		I_Controller.RollByLStick(time, distance);
	}

	void MoveForward(string args)
	{
		JsonData data = JsonMapper.ToObject(args);
		float time = (float)data["time"];
		float distance = (float)data["distance"];
		I_Controller.MoveForward(time, distance);
	}

	void PlaySound(string name)
	{
		if (name == "Run1" || name == "Run2") {
			I_Controller.PlayWalkSound(name);
		}
		else {
			I_Controller.PlaySound(name);
		}
	}

	void FaceByL()
	{
		I_Controller.FaceByL();
	}
}
