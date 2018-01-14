
using UnityEngine;

public class LoginStage : Singleton<LoginStage>, IGameStageBase
{
    private AsyncOperation _sceneLoadOperation;
    private bool _secenIsLoadDone;
    private GameObject scene;

    public void Begin()
    {
        loadScene();
    }

    public void Update()
    {
        if (!_secenIsLoadDone && _sceneLoadOperation != null && _sceneLoadOperation.isDone)
        {
            _secenIsLoadDone = true;
            //UIManager.Instance.OpenUI(UIPanelName.LoginPanel);
        }
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

    private void loadScene()
    {
        Debug.Log("加载场景 >>>>  ");
    }
}