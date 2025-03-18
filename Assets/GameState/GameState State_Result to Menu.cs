using UnityEngine;

public class GameStateState_ResultToMenu : GameStateStateBase
{
    private float elapsedTime;
    private bool isMoveStart;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_L;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;

    public GameStateState_ResultToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_L;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
    }



    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //����t���O���Z�b�g
        isMoveStart = false;

        //�v���C���[���g���l�����Ȃ牉�o�𑁂߂�
        if (TM.currentTerrainNum == 3)
            elapsedTime = 1;

        //�v���C���[����ʊO�Ȃ牉�o�𑁂߂�
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 1.5f;

        //�g���[�j���O���[�h�N���A���͉��o�𑁂߂�
        if (GM.score == 5000 && GM.isTraining)
            elapsedTime = 1.5f;

        //�����L���O���X�V
        GM.highScoreRankingBoard.UpdateRanking();
        GM.playerScoreRankingBoard.UpdateRanking();
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //1.5�b�ҋ@
        if (elapsedTime < 1.5) return;

        //BGM�̃{�����[���ύX
        audioSource_BGM.volume = (2.5f - elapsedTime) * AM.volume_BGM;

        //UI����J�n����
        if (!isMoveStart)
        {
            //�������s�ς݂̃t���O�𗧂Ă�
            isMoveStart = true;

            //�w�i�̒n�`�𓮂���
            if (GM.isTraining)
            {
                TM.moveSpeed = 5 + Mathf.Pow(GM.trainingLevel, 0.7f) * 3;
                TM.SetSpeed(5 + Mathf.Pow(GM.trainingLevel, 0.7f) * 3);
            }
            else
            {
                TM.moveSpeed = 8;
                TM.SetSpeed(8);
            }
        }

        //�n�`���Ǘ�
        //�n�`����J�n��0.1�b�Ԃ�Player���̔���̂��߂Ɏ��s���Ȃ�
        if (elapsedTime > 1.6)
            TM.ManageMovingTerrain();

        //���O�v�Z
        float lerpValue = (elapsedTime - 1.5f) / 1.5f;

        //UI����]
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, lerpValue);

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit()
    {
        //�X�R�A�{�[�h�̈ʒu���C��
        GM.scoreSetTf.position = GM.scoreMarkerTf_Play.position;
        GM.scoreSetTf.localScale = Vector3.one;
    }
}
