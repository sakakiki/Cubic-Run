public class PlayerState_Model_TrainingResult: PlayerStateBase
{
    public PlayerState_Model_TrainingResult(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        //ゲームステートの遷移に合わせてステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
        else if (gameStateMachine.currentState == gameStateMachine.state_ResultToPlay)
            stateMachine.ChangeState(stateMachine.state_Model_ResultToPlay);
    }

    public override void Exit()
    {

    }
}
