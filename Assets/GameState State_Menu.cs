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
        //�n�`���Ǘ�
        TM.ManageMovingTerrain();
    }



    public override void Exit()
    {
        //���j���[��ʂ�UI�𖳌���
        IM.InputUISetActive_Menu(false);
    }
}
