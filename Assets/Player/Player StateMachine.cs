public class PlayerStateMachine
{
    public PlayerStateBase currentState {  get; private set; }
    public PlayerController playerController { get; private set; }

    public PlayerStateBase state_Model_Kinematic { get; private set; }
    public PlayerStateBase state_Model_LookAround { get; private set; }
    public PlayerStateBase state_Model_Move { get; private set; }
    public PlayerStateBase state_Model_Roll { get; private set; }
    public PlayerStateBase state_Model_ResetRotation { get; private set; }
    public PlayerStateBase state_Model_Jump { get; private set; }
    public PlayerStateBase state_Model_Delay { get; private set; }
    public PlayerStateBase state_Model_Squat { get; private set; }
    public PlayerStateBase state_Model_Dragged { get; private set; }
    public PlayerStateBase state_Model_MenuToPlay { get; private set; }
    public PlayerStateBase state_Model_ResultToMenu { get; private set; }
    public PlayerStateBase state_Model_ResultToPlay { get; private set; }
    public PlayerStateBase state_Model_PauseToMenu { get; private set; }
    public PlayerStateBase state_Model_TrainingClear { get; private set; }
    public PlayerStateBase state_Model_MenuToSkin { get; private set; }
    public PlayerStateBase state_Model_Skin { get; private set; }
    public PlayerStateBase state_Model_SkinToMenu { get; private set; }
    public PlayerStateBase state_Play_Run { get; private set; }
    public PlayerStateBase state_Play_Jump { get; private set; }
    public PlayerStateBase state_Play_Squat { get; private set; }
    public PlayerStateBase state_Play_Attack { get; private set; }
    public PlayerStateBase state_Play_SmallJump { get; private set; }
    public PlayerStateBase state_GameOver { get; private set; }

    public PlayerStateMachine(PlayerController playerController)
    {
        this.playerController = playerController;
        state_Model_Kinematic = new PlayerState_Model_Kinematic(this);
        state_Model_LookAround = new PlayerState_Model_LookAround(this);
        state_Model_Move = new PlayerState_Model_Move(this);
        state_Model_Roll = new PlayerState_Model_Roll(this);
        state_Model_ResetRotation = new PlayerState_Model_ResetRotation(this);
        state_Model_Jump = new PlayerState_Model_Jump(this);
        state_Model_Delay = new PlayerState_Model_Delay(this);
        state_Model_Squat = new PlayerState_Model_Squat(this);
        state_Model_Dragged = new PlayerState_Model_Dragged(this);
        state_Model_MenuToPlay = new PlayerState_Model_MenuToPlay(this);
        state_Model_ResultToMenu = new PlayerState_Model_ResultToMenu(this);
        state_Model_ResultToPlay = new PlayerState_Model_ResultToPlay(this);
        state_Model_PauseToMenu = new PlayerState_Model_PauseToMenu(this);
        state_Model_TrainingClear = new PlayerState_Model_TrainingClear(this);
        state_Model_MenuToSkin = new PlayerState_Model_MenuToSkin(this);
        state_Model_Skin = new PlayerState_Model_Skin(this);
        state_Model_SkinToMenu = new PlayerState_Model_SkinToMenu(this);
        state_Play_Run = new PlayerState_Play_Run(this);
        state_Play_Jump = new PlayerState_Play_Jump(this);
        state_Play_Squat = new PlayerState_Play_Squat(this);
        state_Play_Attack = new PlayerState_Play_Attack(this);
        state_Play_SmallJump = new PlayerState_Play_SmallJump(this);
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

    public void ChangeStateDelay(PlayerStateBase newState, float delayTime)
    {
        ((PlayerState_Model_Delay)state_Model_Delay).nextState = newState;
        ((PlayerState_Model_Delay)state_Model_Delay).delayTime = delayTime;
        currentState.Exit();
        currentState = state_Model_Delay;
        state_Model_Delay.Enter();
    }

    public void Update()
    {
        if (currentState != null) currentState.Update();
    }
}
