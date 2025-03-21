public class TutorialStateState_Attack_3 : TutorialStateStateBase
{
    public TutorialStateState_Attack_3(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_SmallJump_1);
    }



    public override void Exit() { }
}
