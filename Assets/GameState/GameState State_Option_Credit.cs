public class GameStateState_Option_Credit : GameStateState_OptionBase
{
    public GameStateState_Option_Credit(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�I�v�V�����̃^�C�g����ҏW
        GameManager.Instance.optionTitle.SetText("�N���W�b�g");

        //Account��UI��L����
        GameManager.Instance.optionUI_Credit.SetActive(true);
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //Volume��UI���\���E������
        GameManager.Instance.optionUI_Credit.SetActive(false);
    }
}
