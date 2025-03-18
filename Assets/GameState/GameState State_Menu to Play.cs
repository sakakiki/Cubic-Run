using UnityEngine;

public class GameStateState_MenuToPlay : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_L;
    public RectTransform playHingeRtf_R;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;

    public GameStateState_MenuToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_L;
        playHingeRtf_R = GM.playHingeRtf_R;
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

        //���x���̃��Z�b�g
        if (GM.isTraining)
            GM.level = GM.trainingLevel - 1;
        else GM.level = 0;
        GM.levelText.SetText("");

        //�X�R�A�̃��Z�b�g
        GM.score = 0;
        PlayStateStateBase.scoreCorrection = GM.level * 5000;
        GM.scoreText.SetText("0");

        //�X�R�A�Q�[�W���Z�b�g
        GM.scoreGageTf.localScale = Vector3.right + Vector3.forward;
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //���O�v�Z
        float lerpValue = (elapsedTime - 0.5f) / 1.5f;

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //UI����]
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color = 
            startCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //BGM�̃{�����[���ύX
        audioSource_BGM.volume = (2 - elapsedTime) * AM.volume_BGM;

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //���x���e�L�X�g�̈ʒu�E�傫���E�F����
        GM.levelTf.position = GM.centerPos_World;
        GM.levelText.fontSize = 300;
        GM.levelText.color = Color.clear;
    }
}
