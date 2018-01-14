using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateStage : Singleton<CreateStage>, IGameStageBase
{
    private AsyncOperation _sceneLoadOperation;
    private bool _secenIsLoadDone;

	private readonly string loadSceneName = "001";

    public void Begin()
    {
        Debug.Log("CreateStage---Begin");
        loadScene();
    }

    public void Update()
    {
        if (!_secenIsLoadDone && _sceneLoadOperation != null && _sceneLoadOperation.isDone)
        {
            _secenIsLoadDone = true;
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
        Debug.Log("加载场景 >>>> CreateStage ");
		_sceneLoadOperation = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Single);
    }
}