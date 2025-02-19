public class LoginStateState_Select : LoginStateStateBase
{
    public LoginStateState_Select(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        if (AM.GetIsLogin()) stateMachine.ChangeState(stateMachine.state_Exit);
    }

    public override void Exit()
    {

    }
}
