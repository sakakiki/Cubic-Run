using UnityEngine;

public class GameStateState_PlayToResult : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_B;
    private AnimationCurve curveScorePosY;

    public GameStateState_PlayToResult(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        targetPos = GM.scoreMarkerTf_Result.position;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_B = GM.resultHingeRtf_B;
        curveScorePosY = GM.scorePosY_PlaytoResult;
    }


    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //�n�`�̒�~
        TerrainManager.Instance.SetSpeed(0);

        //��ʃ^�b�v���o�{�^���̗L����
        IM.InputUISetActive_Screen(true);

        //�X�R�A�{�[�h�̈ړ��J�n�ʒu���L��
        startPos = GM.scoreMarkerTf_Play.position;
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��Ԃ̉��Z
        elapsedTime += deltaTime;

        //1�b�ԑҋ@
        if (elapsedTime < 1) return;

        //��ʂ��^�b�v�����Ή��o���΂�
        IM.GetInput_Screen();
        if (IM.is_Screen_Tap) elapsedTime = 4;

        //���O�v�Z
        float lerpValue = (elapsedTime - 1) / 2;

        //�X�R�A�{�[�h�̈ړ�
        scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(0, 3600, Mathf.Pow((elapsedTime - 1)/3, 0.3f));
        scoreSetTf.position = 
            Vector3.Lerp(startPos, targetPos, lerpValue) + 
            Vector3.up * curveScorePosY.Evaluate(lerpValue) * 3;

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UI����]
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, elapsedTime - 1);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 180, Vector3.zero, elapsedTime - 2);

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime >= 4)
            stateMachine.ChangeState(stateMachine.state_Result);
    }



    public override void Exit()
    {
        //��ʃ^�b�v���o�{�^���̖�����
        IM.InputUISetActive_Screen(false);

        //�X�R�A�{�[�h�̉�]�ʂ����Z�b�g
        scoreSetTf.eulerAngles = Vector3.zero;
    }
}
