public class PlayerState_Run : PlayerStateBase
{
    public PlayerState_Run(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {

    }
    public override void Update()
    {
        if (InputManager.Instance.jumpButtonPush) 
            stateMachine.ChangeState(stateMachine.state_Jump);
        else if (InputManager.Instance.squatButtonPush)
            stateMachine.ChangeState(stateMachine.state_Squat);
        else if (InputManager.Instance.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Attack);
    }
    public override void Exit()
    {

    }
}
