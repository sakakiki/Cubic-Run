public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //���U���g��ʂ�UI��L����
        IM.InputUISetActive_Result(true);
    }



    public override void Update(float deltaTime)
    {
        //���͂̎擾
        IM.GetInput_Result();

        //���͂ɉ������X�e�[�g�J��
        //if (IM.is_Menu_Play_Push)
        //    stateMachine.ChangeState(stateMachine.state_MenuToPlay);
    }



    public override void Exit()
    {
        //���U���g��ʂ�UI�𖳌���
        IM.InputUISetActive_Result(false);
    }
}
