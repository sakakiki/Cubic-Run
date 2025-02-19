public class LoginStateState_Login : LoginStateStateBase
{
    public LoginStateState_Login(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        //���O�C�������ŃX�e�[�g�J��
        if (AM.GetLoginState() == AuthManager.LoginState.Login) 
            stateMachine.ChangeState(stateMachine.state_Exit);

        //���O�C����񂪖������
        //else if (AM.GetLoginState() == AuthManager.LoginState.Logout)
    }

    public override void Exit()
    {

    }
}
