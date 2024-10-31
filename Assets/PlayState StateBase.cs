public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected PlayerController playerCon;
    protected TerrainManager TM;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        TM = stateMachine.TM;
        playerCon = GameManager.Instance.playerCon;
    }

    public abstract void Enter();
    public abstract void Update(float deltaTime);
    public abstract void Exit();
}
