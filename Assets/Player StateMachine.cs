public class PlayerStateMachine
{
    public PlayerStateBase currentState {  get; private set; }
    public PlayerController playerController { get; private set; }

    public PlayerStateBase state_Run { get; private set; }
    public PlayerStateBase state_Jump { get; private set; }
    public PlayerStateBase state_Squat { get; private set; }
    public PlayerStateBase state_Attack { get; private set; }
    public PlayerStateBase state_SmallJump { get; private set; }
    public PlayerStateBase state_GameOver { get; private set; }

    public PlayerStateMachine(PlayerController playerController)
    {
        this.playerController = playerController;
        state_Run = new PlayerState_Run(this);
        state_Jump = new PlayerState_Jump(this);
        state_Squat = new PlayerState_Squat(this);
        state_Attack = new PlayerState_Attack(this);
        state_SmallJump = new PlayerState_SmallJump(this);
        state_GameOver = new PlayerState_GameOver(this);
    }

    public void Initialize(PlayerStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
    }

    public void ChangeState(PlayerStateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }

    public void Update()
    {
        if (currentState != null) currentState.Update();
    }
}
