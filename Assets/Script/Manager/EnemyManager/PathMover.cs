using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathMover : AIPath {

	public Manager I_Manager;
	public Transform self;
	new void Awake()
	{
		base.Awake();
	}

	new void Start ()
	{
		self = transform;
		I_Manager = self.GetComponent<Manager>();
		//base.Start();
		//repathRate = 10;
		//GoToTarpos(new Vector3(0.2f, 0, 0));
	}
	
	new void Update ()
	{
		base.Update();
		
	}

	new void OnEnable()
	{
		base.OnEnable();
		
	}

	new void OnDisable()
	{
		base.OnDisable();
		
	}

	/*--------------- Utils --------------------*/
	public Vector3 GetVelocity()
	{
		return velocity;
	}
	/*--------------- Utils --------------------*/

	public delegate void CallbackHandler();
	CallbackHandler TargetReachedCallback;
	public void MoveTo(Vector3 pos, CallbackHandler CallbackFunc)
	{
		targetPos = pos;
		TargetReachedCallback = CallbackFunc;
		SearchPath();
	}
	public override void OnTargetReached()
	{
		TargetReachedCallback();
	}

	protected override void MovementUpdate(float deltaTime)
	{
		bool isDead = I_Manager.IsPlayerDead();
		if (!TargetReached && !isDead) {
			base.MovementUpdate(deltaTime);
		}
	}
}
