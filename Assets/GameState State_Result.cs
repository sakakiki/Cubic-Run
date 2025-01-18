public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //リザルト画面のUIを有効化
        IM.InputUISetActive_Result(true);

        //ハイスコアの更新
        if (!GM.isTraining && GM.score > GM.highScore)
            GM.highScore = GM.score;
    }



    public override void Update(float deltaTime)
    {
        //入力の取得
        IM.GetInput_Result();

        //入力に応じたステート遷移
        if (IM.is_Result_Title_Tap)
            stateMachine.ChangeState(stateMachine.state_ResultToMenu);
        else if (IM.is_Result_Retry_Tap)
            stateMachine.ChangeState(stateMachine.state_ResultToPlay);
    }



    public override void Exit()
    {
        //リザルト画面のUIを無効化
        IM.InputUISetActive_Result(false);
    }
}
