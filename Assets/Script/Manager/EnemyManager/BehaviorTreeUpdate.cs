using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BehaviorTreeUpdate : MonoBehaviour {
	public BehaviorTree behaviorTree;

	Pathfinding.AIPathAgent pathAgent;
	Manager I_Manager;
	SharedFloat baseSpeed;
	SharedFloat curSpeed;
	Transform body;
	Transform leg;
	Animator legAnim;
	Animator bodyAnim;
	AnimatorStateInfo bodyAnimInfo;

	void Awake()
	{
		pathAgent = transform.GetComponent<Pathfinding.AIPathAgent>();
		body = transform.Find("Body");
		leg = transform.Find("Leg");
		legAnim = leg.GetComponent<Animator>();
		bodyAnim = body.GetComponent<Animator>();
		I_Manager = transform.GetComponent<Manager>();
	}

	void Start ()
	{
		baseSpeed = (float)behaviorTree.GetVariable("RunSpeed").GetValue();
	}

	float curTime = 0;
	const float deltaTime = 0.3f;
	Vector3 prePos;
	void Update ()
	{
		curTime += Time.deltaTime;
		if (curTime >= deltaTime) {
			curTime -= deltaTime;
			// 状态机速度变更
			if (prePos == null) {
				prePos = transform.position;
				curSpeed = pathAgent.Velocity().magnitude;
			}
			else {
				Vector3 V = transform.position - prePos;
				curSpeed = V.magnitude / deltaTime;
				prePos = transform.position;
			}
			
			bodyAnimInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
			if (bodyAnimInfo.IsName("Attack")) {
				bodyAnim.speed = I_Manager.I_DataManager.attackSpeedRate;
			}
			else if (bodyAnimInfo.IsName("Walk")) {
				bodyAnim.speed = curSpeed.Value / baseSpeed.Value;
			}
			else {
				bodyAnim.speed = 1;
			}
			legAnim.speed = curSpeed.Value / baseSpeed.Value;
		}
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}
}
