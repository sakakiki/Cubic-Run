public class LoginStateState_Register : LoginStateStateBase
{
    public LoginStateState_Register(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public async override void Enter()
    {
        //匿名認証と新規アカウント作成
        switch (await AM.SignInAnonymously())
        {
            //正常終了していればゲーム開始処理へ
            case 0: stateMachine.ChangeState(stateMachine.state_Exit); break;

            //ネットワークエラーならメッセージ表示
            case 1: break;

            //予期せぬエラーならその旨を表示
            case 9:
            default: break;
        }
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
