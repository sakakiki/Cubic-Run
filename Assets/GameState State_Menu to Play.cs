public class GameStateState_MenuToPlay : GameStateStateBase
{
    private float elapsedTime;

    public GameStateState_MenuToPlay(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //��Q���̐����𖳌���
        TM.isCreateObstacle = false;

        //�w�i�̒n�`������
        TM.moveSpeed = 20;
        TM.SetSpeed(20);
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //�n�`������
        TM.moveSpeed = 8;
        TM.SetSpeed(8);
    }
}
