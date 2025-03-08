public class GameStateState_Option_Account : GameStateState_OptionBase
{
    public GameStateState_Option_Account(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public async override void Enter()
    {
        base.Enter();

        //オプションのタイトルを編集
        GameManager.Instance.optionTitle.SetText("アカウント");

        //AccountのUIを有効化
        GameManager.Instance.optionUI_Account.SetActive(true);

        //アカウントの最新状態を取得
        if (await AuthManager.Instance.UpdateUserData())
        {
            //アカウントの状態に応じた表示内容変更
            if (AuthManager.Instance.GetIsAnonymous())
            {
                GameManager.Instance.optionUI_CurrentAccount.SetText("未設定");
                GameManager.Instance.optionUI_Account_Status.SetText("メールアドレス未登録");
                GameManager.Instance.optionUI_Account_Anonymous.SetActive(true);
                GameManager.Instance.optionUI_Account_Email.SetActive(false);
            }
            else
            {
                GameManager.Instance.optionUI_CurrentAccount.SetText(AuthManager.Instance.GetEmail());
                if (AuthManager.Instance.GetIsEmailVerified())
                    GameManager.Instance.optionUI_Account_Status.SetText("メールアドレス登録済・認証済");
                else
                {
                    GameManager.Instance.optionUI_Account_Status.SetText("メールアドレス登録済・未認証");
                    GameManager.Instance.optionUI_SendEmail.SetActive(true);
                }

                GameManager.Instance.optionUI_Account_Anonymous.SetActive(false);
                GameManager.Instance.optionUI_Account_Email.SetActive(true);
            }

            //共通のボタンを表示
            GameManager.Instance.optionUI_DeleteData.SetActive(true);
        }
        else
        {
            GameManager.Instance.optionUI_Account_Status.SetText("アカウントの状態を取得できませんでした。");
            GameManager.Instance.optionUI_Relogin.SetActive(true);
        }
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //AccountのUIを非表示・無効化
        GameManager.Instance.optionUI_Account_Status.SetText("");
        GameManager.Instance.optionUI_Account_Anonymous.SetActive(false);
        GameManager.Instance.optionUI_Account_Email.SetActive(false);
        GameManager.Instance.optionUI_SendEmail.SetActive(false);
        GameManager.Instance.optionUI_DeleteData.SetActive(false);
        GameManager.Instance.optionUI_Relogin.SetActive(false);
        GameManager.Instance.optionUI_Account.SetActive(false);
    }
}
