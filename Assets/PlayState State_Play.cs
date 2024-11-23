public class PlayStateState_Play : PlayStateStateBase
{
    public int levelEndScore;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //障害物生成を有効化
        TM.isCreateObstacle = true;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //基準スコアに到達すればステート遷移
        if (GM.score > levelEndScore)
            stateMachine.ChangeState(stateMachine.state_LevelEnd);
    }



    public override void Exit()
    {

    }
}
