public class GameStateState_CheckStamina : GameStateStateBase
{
    private int remainingStamina;



    public GameStateState_CheckStamina(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public async override void Enter()
    {
        //�����̃X�^�~�i�c�ʂ��擾
        remainingStamina = await FirestoreManager.Instance.UseStamina();

        //�X�^�~�i�s���ʒm
        if (remainingStamina == -1)
        {
            PopupUIManager.Instance.SetupPopupMessage(
                "�X�^�~�i�s��",
                "�X�^�~�i������܂���B\n�X�^�~�i�͖���4:00�ɑS�񕜂��܂��B");
            stateMachine.ChangeState(stateMachine.state_Menu);
            return;
        }

        //�G���[�ʒm
        else if (remainingStamina == -2)
        {
            PopupUIManager.Instance.SetupMessageBand("�X�^�~�i��񂪎擾�ł��܂���ł����B", 2);
            stateMachine.ChangeState(stateMachine.state_Menu);
            return;
        }

        //���߃X�^�~�i����̏ꍇ�̏���
        else if (remainingStamina >= 3)
            GM.UpdateOverStamina(remainingStamina);

        //SE�Đ�
        AM.PlaySE(AM.SE_UseStama);

        //���Ȃ���΃X�e�[�g�J��
        stateMachine.ChangeState(stateMachine.state_MenuToPlay);
    }



    public override void Update(float deltaTime) { }



    public override void Exit()
    {
        //���X�e�[�g�ɕϐ��̎󂯓n��
        GameStateState_MenuToPlay.remainingStamina = remainingStamina;
    }
}
