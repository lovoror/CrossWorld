
using UnityEngine;

public class BattleStage : Singleton<BattleStage>, IGameStageBase
{
	private AsyncOperation _sceneLoadOperation;
	private bool _secenIsLoadDone;
	private GameObject scene;


	public void Begin()
	{
		Init ();
	}

	private void Init()
	{
		
	}

	public void Update()
	{
		
	}

	public void FixedUpdate()
	{
		
	}

	public void LateUpdate()
	{
		
	}

	public void End()
	{
	}

}