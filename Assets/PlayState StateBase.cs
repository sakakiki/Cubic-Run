public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected PlayerController playerCon;
    protected GameManager GM;
    protected TerrainManager TM;
    public static float playTime;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        GM = GameManager.Instance;
        TM = TerrainManager.Instance;
        playerCon = GM.playerCon;
    }

    public abstract void Enter();
    public abstract void Update(float deltaTime);
    public abstract void Exit();
}
