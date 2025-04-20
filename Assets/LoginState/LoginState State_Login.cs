public class LoginStateState_Login : LoginStateStateBase
{
    public LoginStateState_Login(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(){ }

    public override void Update()
    {
        //ログイン成功でゲーム開始処理へ
        if (AM.GetLoginState() == AuthManager.LoginState.Login) 
            stateMachine.ChangeState(stateMachine.state_Exit);

        //ログイン情報が無ければ匿名認証でのアカウント作成へ
        else if (AM.GetLoginState() == AuthManager.LoginState.NoAccount)
            stateMachine.ChangeState(stateMachine.state_Register);
    }

    public override void Exit(){ }
}
