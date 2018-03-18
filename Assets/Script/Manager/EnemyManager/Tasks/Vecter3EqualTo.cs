namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("SharedVecter3 equals to targetValue Returns Success.")]
	public class Vecter3EqualTo : Action
	{
		[Tooltip("The value to set the SharedVector3 to")]
		public SharedVector3 targetValue;
		[RequiredField]
		[Tooltip("The SharedVector3 to check")]
		public SharedVector3 targetVariable;

		public override TaskStatus OnUpdate()
		{
			if (targetVariable.Value == targetValue.Value) {
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			targetValue = new SharedVector3();
			targetVariable = new SharedVector3();
		}
	}
}