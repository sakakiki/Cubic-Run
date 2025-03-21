public class TutorialStateState_SmallJump_4 : TutorialStateStateBase
{
    public TutorialStateState_SmallJump_4(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Fall_1);
    }



    public override void Exit() { }
}
