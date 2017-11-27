using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
public class TaskB : Action
{
	[LinkedTask]
	public TaskA reffTaskA;
	public float SomeFloat;
}