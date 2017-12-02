using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class TaskA : Action {

	BehaviorTree behaviorTree;

	public override void OnAwake()
	{
		base.OnAwake();
		behaviorTree = transform.GetComponent<BehaviorTree>();
	}


	public override TaskStatus OnUpdate()
	{
		base.OnUpdate();
		MonoBehaviour.print((Vector3)behaviorTree.GetVariable("NoisePos").GetValue());
		return TaskStatus.Success;
	}
}
