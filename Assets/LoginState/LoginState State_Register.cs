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
            case 1:
                PopupUIManager.Instance.SetupPopupMessage(
                    "ネットワークエラー",
                    "ネットワーク上のエラーが発生しました。\n" +
                    "通信環境を確認の上再試行してください。",
                    Enter); 
                break;

            //予期せぬエラーならその旨を表示
            case 9:
            default:
                PopupUIManager.Instance.SetupPopupMessage(
                    "通信エラー",
                    "エラーが発生しました。\n" +
                    "時間を空けて再試行してください。",
                    Enter); 
                break;
        }
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
