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
