public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected PlayerController playerCon;
    protected TerrainManager TM;
    public static float playTime;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        TM = TerrainManager.Instance;
        playerCon = GameManager.Instance.playerCon;
    }

    public abstract void Enter();
    public abstract void Update(float deltaTime);
    public abstract void Exit();
}
