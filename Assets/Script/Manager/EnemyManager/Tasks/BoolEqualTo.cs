namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("SharedBool equals to targetValue Returns Success.")]
	public class BoolEqualTo : Action
	{
		[Tooltip("The value to set the SharedBool to")]
		public SharedBool targetValue;
		[RequiredField]
		[Tooltip("The SharedBool to check")]
		public SharedBool targetVariable;

		public override TaskStatus OnUpdate()
		{
			if (targetVariable.Value == targetValue.Value) {
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			targetValue = false;
			targetVariable = false;
		}
	}
}