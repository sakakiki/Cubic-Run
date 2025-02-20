public class LoginStateState_Login : LoginStateStateBase
{
    public LoginStateState_Login(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(){ }

    public override void Update()
    {
        //���O�C�������ŃQ�[���J�n������
        if (AM.GetLoginState() == AuthManager.LoginState.Login) 
            stateMachine.ChangeState(stateMachine.state_Exit);

        //���O�C����񂪖�����Γ����F�؂ł̃A�J�E���g�쐬��
        else if (AM.GetLoginState() == AuthManager.LoginState.NoAccount)
            stateMachine.ChangeState(stateMachine.state_Register);
    }

    public override void Exit(){ }
}
