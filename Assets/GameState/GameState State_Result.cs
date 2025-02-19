public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //���U���g��ʂ�UI��L����
        IM.InputUISetActive_Result(true);

        //�v���C���[��UI�̊��h�~�ǂ̗L����
        GM.resultWallCol.enabled = true;

        //�v���C���[�ړ��\�G���A�̒��S�̕ύX
        GM.centerPos_PlayerArea = GM.centerPos_PlayerArea_Result;
    }



    public override void Update(float deltaTime)
    {
        //���͂̎擾
        IM.GetInput_Result();

        //���͂ɉ������X�e�[�g�J��
        if (IM.is_Result_Title_Push)
            stateMachine.ChangeState(stateMachine.state_ResultToMenu);
        else if (IM.is_Result_Retry_Push)
            stateMachine.ChangeState(stateMachine.state_ResultToPlay);
    }



    public override void Exit()
    {
        //���U���g��ʂ�UI�𖳌���
        IM.InputUISetActive_Result(false);

        //�v���C���[��UI�̊��h�~�ǂ̖�����
        GM.resultWallCol.enabled = false;

        //�v���C���[�ړ��\�G���A�̒��S�̕ύX
        GM.centerPos_PlayerArea = GM.centerPos_World;
    }
}
