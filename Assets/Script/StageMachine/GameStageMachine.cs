public enum GameStage
{
    LOGIN, //登录
    CREATE, //创角
    BATTLE //战斗
}

public class GameStageMachine:Singleton<GameStageMachine>
{
    protected IGameStageBase currentStage;

    public IGameStageBase CurrStage
    {
        get { return currentStage; }
    }

    public virtual void Init()
    {
		ChangeStage(GameStage.LOGIN);
    }

    public IGameStageBase GetStage(GameStage type)
    {
        currentStage = null;
        switch (type)
        {
            case GameStage.LOGIN:
                currentStage = LoginStage.Instance;
                break;
			case GameStage.CREATE:
				currentStage = CreateStage.Instance;
                break;
			case GameStage.BATTLE:
				currentStage = BattleStage.Instance;
                break;
        }
        return currentStage;
    }

    public virtual void ChangeStage(GameStage type)
    {
        if (currentStage != null)
            currentStage.End();

        currentStage = GetStage(type);
        currentStage.Begin();
    }

    public void Update()
    {
        if (currentStage != null)
            currentStage.Update();
    }

    public void FixedUpdate()
    {
        if (currentStage != null)
            currentStage.FixedUpdate();
    }

	public void LateUpdate()
	{
		if (currentStage != null)
			currentStage.LateUpdate ();
	}

    public virtual void Destroy()
    {
    }
}