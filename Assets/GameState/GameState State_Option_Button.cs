public class GameStateState_Option_Button : GameStateState_OptionBase
{
    public GameStateState_Option_Button(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //オプションのタイトルを編集
        GameManager.Instance.optionTitle.SetText("ボタン配置変更");

        //UIを有効化
        GameManager.Instance.optionUI_Button.SetActive(true);
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //VolumeのUIを非表示・無効化
        GameManager.Instance.optionUI_Button.SetActive(false);
    }
}
