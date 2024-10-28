public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected PlayerController playerCon;
    protected GameManager GM;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        GM = stateMachine.GM;
        playerCon = GameManager.Instance.playerCon;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
