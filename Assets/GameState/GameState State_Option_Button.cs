public class GameStateState_Option_Button : GameStateState_OptionBase
{
    public GameStateState_Option_Button(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�I�v�V�����̃^�C�g����ҏW
        GameManager.Instance.optionTitle.SetText("�{�^���z�u�ύX");

        //UI��L����
        GameManager.Instance.optionUI_Button.SetActive(true);
    }



    public override void Update(float deltaTime)
    {
        base .Update(deltaTime);
    }



    public override void Exit()
    {
        base.Exit();

        //Volume��UI���\���E������
        GameManager.Instance.optionUI_Button.SetActive(false);
    }
}
