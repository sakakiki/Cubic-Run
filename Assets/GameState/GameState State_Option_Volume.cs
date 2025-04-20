public class GameStateState_Option_Volume : GameStateState_OptionBase
{
    public GameStateState_Option_Volume(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //オプションのタイトルを編集
        GameManager.Instance.optionTitle.SetText("ボリューム");

        //AccountのUIを有効化
        GameManager.Instance.optionUI_Volume.SetActive(true);
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //VolumeのUIを非表示・無効化
        GameManager.Instance.optionUI_Volume.SetActive(false);
    }
}
