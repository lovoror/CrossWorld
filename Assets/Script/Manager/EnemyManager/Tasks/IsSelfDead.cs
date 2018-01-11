using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class IsSelfDead : Conditional
	{
		Manager I_Manager;
		public override void OnStart()
		{
			base.OnStart();
			I_Manager = transform.GetComponent<Manager>();
		}

		public override TaskStatus OnUpdate()
		{
			if (I_Manager.IsDead()) {
				return TaskStatus.Success;
			}
			else {
				return TaskStatus.Failure;
			}
		}

	}
}