using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class IsPlayerDead : Conditional
	{
		GameObject player;
		Manager Player_Manager;
		public override void OnStart()
		{
			base.OnStart();
			player = (GameObject)Owner.GetVariable("Player").GetValue();
			Player_Manager = player.GetComponent<Manager>();
		}

		public override TaskStatus OnUpdate()
		{
			if (Player_Manager.IsDead()) {
				return TaskStatus.Success;
			}
			else {
				return TaskStatus.Failure;
			}
		}

	}
}