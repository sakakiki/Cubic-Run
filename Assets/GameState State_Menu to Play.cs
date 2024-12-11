using UnityEngine;

public class GameStateState_MenuToPlay : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_L;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;

    public GameStateState_MenuToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_L;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Menu;
        targetCoverColor = GM.screenCoverColor_Play;
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

        //UI����]
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, (elapsedTime -1) / 1.5f);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, (elapsedTime -1) / 1.5f);
        playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, (elapsedTime - 1) / 1.5f);

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color = Color.Lerp(startCoverColor, targetCoverColor, (elapsedTime - 1) / 1.5f);

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2.5)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //�n�`������
        TM.moveSpeed = 8;
        TM.SetSpeed(8);
    }
}
