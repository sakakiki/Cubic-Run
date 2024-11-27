public abstract class GameStateStateBase
{
    protected GameStateStateMachine stateMachine;
    protected GameManager GM;
    protected TerrainManager TM;
    protected PlayerController playerCon;
    protected PlayerStateMachine playerStateMachine;

    public GameStateStateBase(GameStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        GM = GameManager.Instance;
        TM = TerrainManager.Instance;
        playerCon = GM.playerCon;
        playerStateMachine = playerCon.stateMachine;
    }

    public abstract void Enter();

    public abstract void Update(float deltaTime);

    public abstract void Exit();
}
