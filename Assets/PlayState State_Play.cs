public class PlayStateState_Play : PlayStateStateBase
{
    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {

    }



    public override void Update()
    {
        if (TM.stageRightEdge < 30)
            TM.CreateTerrainRandom();
    }



    public override void Exit()
    {

    }
}
