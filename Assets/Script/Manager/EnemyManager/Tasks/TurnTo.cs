using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class TurnTo : Action
	{
		[RequiredField]
		public SharedGameObject player;
		public SharedFloat rotationSpeed = 360;

		Quaternion raw_rotation;
		Quaternion lookat_rotation;
		float rotate_angle;  // 旋转角度
		float total_tm;   // 旋转总时间
		float lerp_tm;

		public override void OnStart()
		{
			base.OnStart();
			raw_rotation = transform.rotation;
			transform.LookAt(((GameObject) player.GetValue()).transform);
			lookat_rotation = transform.rotation;
			transform.rotation = raw_rotation;
			rotate_angle = Quaternion.Angle(raw_rotation, lookat_rotation);
			total_tm = rotate_angle / (float)rotationSpeed.GetValue();
			lerp_tm = 0;
		}

		public override TaskStatus OnUpdate()
		{
			lerp_tm += Time.deltaTime / total_tm;
			transform.rotation = Quaternion.Lerp(raw_rotation, lookat_rotation, lerp_tm);
			if (lerp_tm >= 1) {
				transform.rotation = lookat_rotation;
				return TaskStatus.Success;
			}
			else {
				return TaskStatus.Running;
			}
		}
	}
}