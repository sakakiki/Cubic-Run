public class TutorialStateState_Squat_3 : TutorialStateStateBase
{
    public TutorialStateState_Squat_3(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Attack_1);
    }



    public override void Exit() { }
}
