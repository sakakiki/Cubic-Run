using UnityEngine;

public class GameStateState_ResultToPlay : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    private bool isMoveTerrein;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;
    private bool isResetScore;

    public GameStateState_ResultToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        startPos = GM.scoreMarkerTf_Result.position;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Menu;
        targetCoverColor = GM.screenCoverColor_Play;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
    }



    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //����t���O���Z�b�g
        isMoveTerrein = false;
        isResetScore = false;

        //�X�R�A�{�[�h�̖ڕW�ʒu���L��
        targetPos = GM.scoreMarkerTf_Play.position;

        //�v���C���[���g���l�����Ȃ牉�o�𑁂߂�
        if (TM.currentTerrainNum == 3)
            elapsedTime = 1;

        //�v���C���[����ʊO�Ȃ牉�o�𑁂߂�
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 1.5f;

        //��Q���̐������~
        TM.isCreateObstacle = false;
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
        TM.ManageMovingTerrain();

        //���O�v�Z
        float lerpValue = (elapsedTime - 1.5f) / 1.5f;

        //�X�R�A�{�[�h�̈ړ�
        scoreSetTf.position = Vector3.Lerp(startPos, targetPos, lerpValue);

        //�X�R�A�{�[�h�̉�]
        if (elapsedTime < 2.25)
            scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(360, 180, lerpValue);
        else
        {
            scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(180, 0, lerpValue);

            //�X�R�A�{�[�h�̃��Z�b�g
            if (!isResetScore)
            {
                isResetScore = true;

                //���x���̃��Z�b�g
                if (GM.isTraining)
                    GM.level = GM.trainingLevel - 1;
                else GM.level = 0;
                GM.levelText.SetText("");

                //�X�R�A�̃��Z�b�g
                GM.score = 0;
                PlayStateStateBase.scoreCorrection = GM.level * 5000;
                GM.scoreText.SetText("0");

                //���x���e�L�X�g�̈ʒu�E�傫���E�F����
                GM.levelTf.position = GM.centerPos_World;
                GM.levelText.fontSize = 300;
                GM.levelText.color = Color.clear;
            }
        }

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color =
            startCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UI����]
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, elapsedTime - 2);
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, elapsedTime - 2);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, elapsedTime - 2);

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //�X�R�A�{�[�h�̈ʒu���C��
        GM.scoreSetTf.position = GM.scoreMarkerTf_Play.position;
    }
}
