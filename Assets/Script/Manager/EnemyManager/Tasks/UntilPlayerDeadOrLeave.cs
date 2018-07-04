using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class UntilPlayerDeadOrLeave : Action
	{
		public SharedFloat attackDistance;
		GameObject player;
		Manager Player_Manager;
		float sqAtkDist;
		public override void OnStart()
		{
			base.OnStart();
			
			sqAtkDist = attackDistance.Value * attackDistance.Value;
			player = (GameObject)Owner.GetVariable("Player").GetValue();
			Player_Manager = player.GetComponent<Manager>();
		}

		public override TaskStatus OnUpdate()
		{
			float sqDist = (transform.position - player.transform.position).sqrMagnitude;
			if (Player_Manager.IsDead() || sqDist > sqAtkDist) {
				return TaskStatus.Success;
			}
			else {
				transform.LookAt(player.transform);
				return TaskStatus.Running;
			}
		}
	}
}