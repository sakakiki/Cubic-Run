public class GameStateState_Menu : GameStateStateBase
{
    public GameStateState_Menu(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //���j���[��ʂ�UI��L����
        IM.InputUISetActive_Menu(true);

        //��Q���̐�����L����
        TM.isCreateObstacle = true;
    }



    public override void Update(float deltaTime)
    {
        //���͂̎擾
        IM.GetInput_Menu();

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //���͂ɉ������X�e�[�g�J��
        if (IM.is_Menu_Play_Push)
            stateMachine.ChangeState(stateMachine.state_MenuToPlay);
    }



    public override void Exit()
    {
        //���j���[��ʂ�UI�𖳌���
        IM.InputUISetActive_Menu(false);
    }
}
