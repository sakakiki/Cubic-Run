public class TutorialStateState_Fall_3 : TutorialStateStateBase
{
    public TutorialStateState_Fall_3(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (TM.nextTerrainNum == 6)
            stateMachine.ChangeState(stateMachine.state_Fall_4);
    }



    public override void Exit() { }
}
