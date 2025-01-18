public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //���U���g��ʂ�UI��L����
        IM.InputUISetActive_Result(true);

        //�n�C�X�R�A�̍X�V
        if (!GM.isTraining && GM.score > GM.highScore)
            GM.highScore = GM.score;
    }



    public override void Update(float deltaTime)
    {
        //���͂̎擾
        IM.GetInput_Result();

        //���͂ɉ������X�e�[�g�J��
        if (IM.is_Result_Title_Tap)
            stateMachine.ChangeState(stateMachine.state_ResultToMenu);
        else if (IM.is_Result_Retry_Tap)
            stateMachine.ChangeState(stateMachine.state_ResultToPlay);
    }



    public override void Exit()
    {
        //���U���g��ʂ�UI�𖳌���
        IM.InputUISetActive_Result(false);
    }
}
