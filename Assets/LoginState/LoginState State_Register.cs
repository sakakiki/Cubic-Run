public class LoginStateState_Register : LoginStateStateBase
{
    public LoginStateState_Register(LoginStateStateMachine stateMachine) : base(stateMachine) { }

    public async override void Enter()
    {
        //�����F�؂ƐV�K�A�J�E���g�쐬
        switch (await AM.SignInAnonymously())
        {
            //����I�����Ă���΃Q�[���J�n������
            case 0: stateMachine.ChangeState(stateMachine.state_Exit); break;

            //�l�b�g���[�N�G���[�Ȃ烁�b�Z�[�W�\��
            case 1:
                PopupUIManager.Instance.SetupPopupMessage(
                    "�l�b�g���[�N�G���[",
                    "�l�b�g���[�N��̃G���[���������܂����B\n" +
                    "�ʐM�����m�F�̏�Ď��s���Ă��������B",
                    Enter); 
                break;

            //�\�����ʃG���[�Ȃ炻�̎|��\��
            case 9:
            default:
                PopupUIManager.Instance.SetupPopupMessage(
                    "�ʐM�G���[",
                    "�G���[���������܂����B\n" +
                    "���Ԃ��󂯂čĎ��s���Ă��������B",
                    Enter); 
                break;
        }
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
