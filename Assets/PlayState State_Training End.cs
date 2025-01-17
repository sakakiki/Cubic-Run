public class PlayStateState_TrainingEnd: PlayStateStateBase
{
    public PlayStateState_TrainingEnd(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //障害物生成を無効化
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //基準スコアに到達すれば
        if (GM.score > 5000)
        {
            //スコアを補正
            GM.score = 5000;
            GM.scoreText.SetText("5000");

            //ステート遷移
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        }
    }



    public override void Exit()
    {

    }
}
