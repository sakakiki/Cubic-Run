using UnityEngine;

public class GameStateState_MenuToPlay : GameStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer screenCover;
    private Color startColor;
    private Color targetColor;

    public GameStateState_MenuToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        screenCover = GM.screenCover;
        startColor = GM.screenCoverColor_Menu;
        targetColor = GM.screenCoverColor_Play;
    }



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

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color = Color.Lerp(startColor, targetColor, elapsedTime - 1);

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
