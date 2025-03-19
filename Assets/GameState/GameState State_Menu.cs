public class GameStateState_Menu : GameStateStateBase
{
    public GameStateState_Menu(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //���j���[��ʂ�UI��L����
        IM.InputUISetActive_Menu(true);

        //��Q���̐�����L����
        TM.isCreateObstacle = true;

        //BGM�����j���[��ʂ̂��̂łȂ���ΕύX���čĐ�
        if (audioSource_BGM.clip != AM.BGM_Menu)
        {
            audioSource_BGM.volume = AM.volume_BGM;
            audioSource_BGM.clip = AM.BGM_Menu;
            audioSource_BGM.Play();
        }
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
        else if (IM.is_Menu_Skin_Push)
            stateMachine.ChangeState(stateMachine.state_MenuToSkin);
        else if (IM.is_Menu_Account_Push)
            stateMachine.ChangeState(stateMachine.state_Account);
        else if (IM.is_Menu_Volume_Push)
            stateMachine.ChangeState(stateMachine.state_Volume);
        else if (IM.is_Menu_Button_Push)
            stateMachine.ChangeState(stateMachine.state_Button);
        else if (IM.is_Menu_Credit_Push)
            stateMachine.ChangeState(stateMachine.state_Credit);
    }



    public override void Exit()
    {
        //���j���[��ʂ�UI�𖳌���
        IM.InputUISetActive_Menu(false);
    }
}
