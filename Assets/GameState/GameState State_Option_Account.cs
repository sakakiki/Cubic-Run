public class GameStateState_Option_Account : GameStateState_OptionBase
{
    public GameStateState_Option_Account(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public async override void Enter()
    {
        base.Enter();

        //�I�v�V�����̃^�C�g����ҏW
        GameManager.Instance.optionTitle.SetText("�A�J�E���g");

        //Account��UI��L����
        GameManager.Instance.optionUI_Account.SetActive(true);

        //�A�J�E���g�̍ŐV��Ԃ��擾
        if (await AuthManager.Instance.UpdateUserData())
        {
            //�A�J�E���g�̏�Ԃɉ������\�����e�ύX
            if (AuthManager.Instance.GetIsAnonymous())
            {
                GameManager.Instance.optionUI_CurrentAccount.SetText("���ݒ�");
                GameManager.Instance.optionUI_Account_Status.SetText("���[���A�h���X���o�^");
                GameManager.Instance.optionUI_Account_Anonymous.SetActive(true);
                GameManager.Instance.optionUI_Account_Email.SetActive(false);
            }
            else
            {
                GameManager.Instance.optionUI_CurrentAccount.SetText(AuthManager.Instance.GetEmail());
                if (AuthManager.Instance.GetIsEmailVerified())
                    GameManager.Instance.optionUI_Account_Status.SetText("���[���A�h���X�o�^�ρE�F�؍�");
                else
                {
                    GameManager.Instance.optionUI_Account_Status.SetText("���[���A�h���X�o�^�ρE���F��");
                    GameManager.Instance.optionUI_SendEmail.SetActive(true);
                }

                GameManager.Instance.optionUI_Account_Anonymous.SetActive(false);
                GameManager.Instance.optionUI_Account_Email.SetActive(true);
            }

            //���ʂ̃{�^����\��
            GameManager.Instance.optionUI_DeleteData.SetActive(true);
        }
        else
        {
            GameManager.Instance.optionUI_Account_Status.SetText("�A�J�E���g�̏�Ԃ��擾�ł��܂���ł����B");
            GameManager.Instance.optionUI_Relogin.SetActive(true);
        }
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //Account��UI���\���E������
        GameManager.Instance.optionUI_Account_Status.SetText("");
        GameManager.Instance.optionUI_Account_Anonymous.SetActive(false);
        GameManager.Instance.optionUI_Account_Email.SetActive(false);
        GameManager.Instance.optionUI_SendEmail.SetActive(false);
        GameManager.Instance.optionUI_DeleteData.SetActive(false);
        GameManager.Instance.optionUI_Relogin.SetActive(false);
        GameManager.Instance.optionUI_Account.SetActive(false);
    }
}
