public class GameStateState_TutorialRetry : GameStateStateBase
{
    private float elapsedTime;
    private bool isMoveTerrein;

    public GameStateState_TutorialRetry(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = - 1;

        //����t���O���Z�b�g
        isMoveTerrein = false;

        //�n�`�̒�~
        TM.SetSpeed(0);

        //�v���C���[���g���l�����Ȃ牉�o�𑁂߂�
        if (TM.currentTerrainNum == 3)
            elapsedTime = 0;

        //�v���C���[����ʊO�Ȃ牉�o�𑁂߂�
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 0.5f;
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //1.5�b�ҋ@
        if (elapsedTime < 1.5) return;

        //�n�`��������
        if (!isMoveTerrein)
        {
            isMoveTerrein = true;
            TM.moveSpeed = 30;
            TM.SetSpeed(30);
        }

        //�n�`���Ǘ�
        //�n�`����J�n��0.1�b�Ԃ�Player���̔���̂��߂Ɏ��s���Ȃ�
        if (elapsedTime > 1.6)
            TM.ManageMovingTerrain();

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Tutorial);
    }



    public override void Exit() { }
}
