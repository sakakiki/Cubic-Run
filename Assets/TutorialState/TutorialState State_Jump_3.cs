public class TutorialStateState_Jump_3 : TutorialStateStateBase
{
    public TutorialStateState_Jump_3(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Squat_1);
    }



    public override void Exit() { }
}
