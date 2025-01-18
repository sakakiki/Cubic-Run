public class PlayerState_Model_TrainingResult: PlayerStateBase
{
    public PlayerState_Model_TrainingResult(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        //�Q�[���X�e�[�g�̑J�ڂɍ��킹�ăX�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
        else if (gameStateMachine.currentState == gameStateMachine.state_ResultToPlay)
            stateMachine.ChangeState(stateMachine.state_Model_ResultToPlay);
    }

    public override void Exit()
    {

    }
}
