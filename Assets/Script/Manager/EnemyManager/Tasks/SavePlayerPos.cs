using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("设置指定变量的值为当前Player所在的值. Returns Success.")]
	public class SavePlayerPos : Action
	{
		[RequiredField]
		[Tooltip("The SharedVector3 to set")]
		public SharedVector3 targetVariable;

		public override TaskStatus OnUpdate()
		{
			targetVariable.Value = ((GameObject)(Owner.GetVariable("Player").GetValue())).transform.position;

			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetVariable = Vector3.zero;
		}
	}
}