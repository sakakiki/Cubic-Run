public class PlayStateState_Play : PlayStateStateBase
{
    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {

    }



    public override void Update()
    {
        if (GM.stageRightEdge < 30)
            GM.CreateObstacleRandom();
    }



    public override void Exit()
    {

    }
}
