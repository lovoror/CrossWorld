using UnityEngine.SceneManagement;
using UnityEngine;

public class LoginStage : Singleton<LoginStage>, IGameStageBase
{
    private AsyncOperation _sceneLoadOperation;
    private bool _secenIsLoadDone;
	private readonly string loadSceneName = "AnimationTest";
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
		Observer.StageEnd();
    }

    private void loadScene()
    {
        Debug.Log("加载场景 >>>>  ");
		_sceneLoadOperation = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Single);
	}
}