public class GameStateState_Menu : GameStateStateBase
{
    public GameStateState_Menu(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //�e�{�^���̓��̓t���O�����Z�b�g
        IM.GetInput_Menu();

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

    }
}
