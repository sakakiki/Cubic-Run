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
