using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameStateState_PauseToMenu : GameStateStateBase
{
    private float elapsedTime;
    private bool isMoveStart;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_L;
    public RectTransform playHingeRtf_R;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;

    public GameStateState_PauseToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_R;
        playHingeRtf_R = GM.playHingeRtf_L;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
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

        //�w�i�̒n�`���~
        TM.moveSpeed = 0;
        TM.SetSpeed(0);

        //�����L���O���X�V
        GM.highScoreRankingBoard.UpdateRanking();
        GM.playerScoreRankingBoard.UpdateRanking();
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //1.5�b�ҋ@
        if (elapsedTime < 1) return;

        //BGM�̃{�����[���ύX
        audioSource_BGM.volume = 2 - elapsedTime;

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

            //���x���̃e�L�X�g��Z���W���C��
            Vector3 levelPos = GM.levelTf.position;
            levelPos.z = GM.scoreSetTf.position.z;
            GM.levelTf.position = levelPos;
        }

        //�n�`���Ǘ�
        //�n�`����J�n��0.1�b�Ԃ�Player���̔���̂��߂Ɏ��s���Ȃ�
        if (elapsedTime > 1.6)
            TM.ManageMovingTerrain();

        //���O�v�Z
        float lerpValue = (elapsedTime - 1) / 1.5f;

        //UI����]
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2.5)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit() { }
}
