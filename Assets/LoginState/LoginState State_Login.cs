public class LoginStateState_Login : LoginStateStateBase
{
    public LoginStateState_Login(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        //ログイン成功でステート遷移
        if (AM.GetLoginState() == AuthManager.LoginState.Login) 
            stateMachine.ChangeState(stateMachine.state_Exit);

        //ログイン情報が無ければ
        //else if (AM.GetLoginState() == AuthManager.LoginState.Logout)
    }

    public override void Exit()
    {

    }
}
