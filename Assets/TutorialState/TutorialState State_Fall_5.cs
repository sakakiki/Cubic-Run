public class TutorialStateState_Fall_5 : TutorialStateStateBase
{
    public TutorialStateState_Fall_5(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (elapsedTime > 3)
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
    }



    public override void Exit() { }
}
