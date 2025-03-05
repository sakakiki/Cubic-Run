public class GameStateState_Option_Account : GameStateState_OptionBase
{
    public GameStateState_Option_Account(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�I�v�V�����̃^�C�g����ҏW
        GameManager.Instance.optionTitle.SetText("�A�J�E���g");

        //Account��UI��L����
        GameManager.Instance.optionUI_Account.SetActive(true);

        //�A�J�E���g�̏�Ԃɉ������\�����e�ύX
        if (AuthManager.Instance.GetIsAnonymous())
        {
            GameManager.Instance.optionUI_Account_Status.SetText("���[���A�h���X���o�^");
            GameManager.Instance.optionUI_Account_Anonymous.SetActive(true);
            GameManager.Instance.optionUI_Account_Email.SetActive(false);
        }
        else
        {
            GameManager.Instance.optionUI_Account_Status.SetText("���[���A�h���X�o�^��");
            GameManager.Instance.optionUI_Account_Anonymous.SetActive(false);
            GameManager.Instance.optionUI_Account_Email.SetActive(true);
        }
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //Account��UI�𖳌���
        GameManager.Instance.optionUI_Account.SetActive(false);
    }
}
