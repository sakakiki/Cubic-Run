public class TutorialStateState_Jump_1 : TutorialStateStateBase
{
    public TutorialStateState_Jump_1(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //障害物の生成
        TM.CreateTerrain(2, TM.stageRightEdge, 6, 2, TM.moveSpeed);
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (TM.nextTerrainNum == 2)
            stateMachine.ChangeState(stateMachine.state_Jump_2);
    }



    public override void Exit() { }
}
