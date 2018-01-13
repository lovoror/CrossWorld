using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class UntilPlayerDead : Action
	{
		GameObject player;
		Manager I_Manager;
		public override void OnStart()
		{
			base.OnStart();
			player = (GameObject)Owner.GetVariable("Player").GetValue();
			I_Manager = player.GetComponent<Manager>();
		}

		public override TaskStatus OnUpdate()
		{
			if (I_Manager.IsDead()) {
				return TaskStatus.Success;
			}
			else {
				transform.LookAt(player.transform);
				return TaskStatus.Running;
			}
		}

	}
}