public abstract class GameStateStateBase
{
    protected GameStateStateMachine stateMachine;
    protected GameManager GM;
    protected InputManager IM;
    protected TerrainManager TM;
    protected AudioManager AM;
    protected PlayerController playerCon;
    protected PlayerStateMachine playerStateMachine;

    public GameStateStateBase(GameStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        GM = GameManager.Instance;
        IM = InputManager.Instance;
        TM = TerrainManager.Instance;
        AM = AudioManager.Instance;
        playerCon = GM.playerCon;
        playerStateMachine = playerCon.stateMachine;
    }

    public abstract void Enter();

    public abstract void Update(float deltaTime);

    public abstract void Exit();
}
