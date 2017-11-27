using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
public class TaskA : Action
{
	[LinkedTask]
	public TaskA[] referrncedTaskA;
	public void OnAwake()
	{
		
	}
}