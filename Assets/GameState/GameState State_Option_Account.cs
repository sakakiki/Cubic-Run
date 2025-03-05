public class GameStateState_Option_Account : GameStateState_OptionBase
{
    public GameStateState_Option_Account(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //オプションのタイトルを編集
        GameManager.Instance.optionTitle.SetText("アカウント");

        //AccountのUIを有効化
        GameManager.Instance.optionUI_Account.SetActive(true);

        //アカウントの状態に応じた表示内容変更
        if (AuthManager.Instance.GetIsAnonymous())
        {
            GameManager.Instance.optionUI_Account_Status.SetText("メールアドレス未登録");
            GameManager.Instance.optionUI_Account_Anonymous.SetActive(true);
            GameManager.Instance.optionUI_Account_Email.SetActive(false);
        }
        else
        {
            GameManager.Instance.optionUI_Account_Status.SetText("メールアドレス登録済");
            GameManager.Instance.optionUI_Account_Anonymous.SetActive(false);
            GameManager.Instance.optionUI_Account_Email.SetActive(true);
        }
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //AccountのUIを無効化
        GameManager.Instance.optionUI_Account.SetActive(false);
    }
}
