using System.Globalization;
using TMPro;
using UnityEngine;

public class GameStateState_PlayToResult : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 targetScale;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;
    private AnimationCurve curveScorePosY;
    private RectTransform playerRankScaleRtf;
    private TextMeshProUGUI playerRankText;
    private TextMeshProUGUI addExpText;
    private TextMeshProUGUI requiredExpText;
    private int beforePlayRequiredExp;
    private int addExp;
    private int beforePlayRank;
    private TextMeshProUGUI highScoreFluctuationText;
    private TextMeshProUGUI playerScoreFluctuationText;
    private bool isSetText;
    private int beforeHighScore;
    private int beforePlayerScore;
    private int newPlayerScore;
    private TextMeshProUGUI highScoreText;
    private TextMeshProUGUI playerScoreText;
    private bool isAudioPlay;
    private int levelUpCount_state;

    public GameStateState_PlayToResult(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
        curveScorePosY = GM.scorePosY_PlaytoResult;
        playerRankScaleRtf = GM.playerRankScaleRtf;
        playerRankText = GM.playerRankText;
        addExpText = GM.addExpText;
        requiredExpText = GM.requiredExpText;
        highScoreFluctuationText = GM.scoreFluctuation_highScore;
        playerScoreFluctuationText = GM.scoreFluctuation_playerScore;
        highScoreText = GM.resultHighScoreText;
        playerScoreText = GM.resultPlayerScoreText;
    }


    public override async void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //�t���O���Z�b�g
        isSetText = false;
        isAudioPlay = false;

        //�ϐ����Z�b�g
        levelUpCount_state = 0;

        //�Q�[�����[�h�ɉ�����UI�̐؂�ւ�
        GM.resultRankingUI.SetActive(!GM.isTraining);
        GM.resultTrainingUI.SetActive(GM.isTraining);

        //�Q�[�����[�h�ɉ������ړ���E�X�P�[���̕ύX
        targetPos = GM.isTraining ? GM.scoreMarkerTf_Result_Trainig.position : GM.scoreMarkerTf_Result_Ranking.position;
        targetScale = GM.isTraining ? Vector3.one * 1.5f : Vector3.one * 2 / Camera.main.aspect;

        //UI�̓��e�ύX
        if (GM.isTraining)
        {
            GM.clearRateText.SetText((GM.score / 5000f * 100).ToString("F1", CultureInfo.CurrentCulture) + "%");
            GM.clearTimesNumText.SetText("���j�񐔁F" + GM.trainingClearCounts[GM.level - 1] + "��");
        }

        //�g���[�j���O���[�h�N���A��������
        if (GM.score != 5000 || !GM.isTraining)
        {
            //�n�`�̒�~
            TerrainManager.Instance.SetSpeed(0);

            //���g���C�{�^���̃e�L�X�g��ύX
            GM.retryButtonText.SetText("���g���C");
        }

        //��ʃ^�b�v���o�{�^���̗L����
        IM.InputUISetActive_Screen(true);

        //�X�R�A�{�[�h�̈ړ��J�n�ʒu���L��
        startPos = GM.scoreMarkerTf_Play.position;

        //�v���C�O�̃����N�A�b�v�܂łɕK�v�Ȍo���l�ʂ��L��
        beforePlayRequiredExp = GM.requiredExp;

        //�v���C�O�̌o���l�ʂ��L��
        int beforePlaytotalExp = GM.totalExp;

        //�v���C�O�̃v���C���[�����N���L���E�\��
        beforePlayRank = GM.playerRank; 
        playerRankText.SetText("�v���C���[�����N�@" + beforePlayRank);

        //�o���l�Ɋւ���\���̍X�V
        requiredExpText.SetText("" + GM.requiredExp);
        playerRankScaleRtf.localScale = Vector3.one - Vector3.right * (GM.requiredExp / (float)((GM.playerRank+1) * 100));

        //�����L���O���[�h�Ȃ�v���C�O�̃X�R�A���L���E�L��
        if (!GM.isTraining)
        {
            beforeHighScore = GM.highScore;
            beforePlayerScore = GM.GetPlayerScore();
            highScoreText.SetText(beforeHighScore.ToString());
            playerScoreText.SetText(beforePlayerScore.ToString());
        }

        //�v���C���ʂ̃Z�[�u
        await GM.SaveResult();

        //�l���o���l�ʂ̎Z�o
        addExp = GM.totalExp - beforePlaytotalExp;

        //�X�V��̃v���C���[�X�R�A���L��
        newPlayerScore = GM.GetPlayerScore();

        //�X�L���J�����o�̕K�v�����`�F�b�N
        if (beforePlayRank < 10 && 10 <= GM.playerRank) GM.newSkinQueue.Enqueue(2);
        if (beforePlayRank < 20 && 20 <= GM.playerRank) GM.newSkinQueue.Enqueue(3);
        if (beforePlayRank < 30 && 30 <= GM.playerRank) GM.newSkinQueue.Enqueue(4);
        if (beforePlayRank < 40 && 40 <= GM.playerRank) GM.newSkinQueue.Enqueue(5);
        if (beforePlayRank < 50 && 50 <= GM.playerRank) GM.newSkinQueue.Enqueue(6);
        if (beforePlayRank < 100 && 100 <= GM.playerRank) GM.newSkinQueue.Enqueue(7);
        if (GM.score == 5000 && GM.isTraining && GM.trainingClearCounts[GM.level-1] == 1)
        {
            switch (GM.level)
            {
                case 1: GM.newSkinQueue.Enqueue(8); break;
                case 2: GM.newSkinQueue.Enqueue(9); break;
                case 3: GM.newSkinQueue.Enqueue(10); break;
                case 5: GM.newSkinQueue.Enqueue(11); break;
                case 7: GM.newSkinQueue.Enqueue(12); break;
                case 10: GM.newSkinQueue.Enqueue(13); break;
                case 15: GM.newSkinQueue.Enqueue(14); break;
                case 20: GM.newSkinQueue.Enqueue(15); break;
                default: break;
            }
        }

        //�����L���O�X�V
        GM.resultRankingBoard.UpdateRanking();

        //�o���l�E�X�R�A�ϓ��\�����Z�b�g
        addExpText.SetText("");
        highScoreFluctuationText.SetText("");
        playerScoreFluctuationText.SetText("");
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��Ԃ̉��Z
        elapsedTime += deltaTime;

        //�g���[�j���O���[�h�N���A���͒n�`�̊Ǘ�
        if (GM.score == 5000 && GM.isTraining)
            TM.ManageMovingTerrain();

        //�J�n��1�b��
        if (elapsedTime < 1)
        {
            //BGM�̃{�����[���ύX
            audioSource_BGM.volume = (1 - elapsedTime) * AM.volume_BGM;

            //�c��̏������΂�
            return;
        }

        //���U���g�pBGM�̍Đ�
        else if (!isAudioPlay)
        {
            //���s��1��̂�
            isAudioPlay = true;

            //BGM�Đ�
            audioSource_BGM.volume = AM.volume_BGM;
            audioSource_BGM.clip = AM.BGM_Result;
            audioSource_BGM.Play();
        }

        //��ʂ��^�b�v�����Ή��o���΂�
        IM.GetInput_Screen();
        if (IM.is_Screen_Tap) elapsedTime = 5;

        //���O�v�Z
        float lerpValue = (elapsedTime - 1) / 2;

        //�X�R�A�{�[�h�̈ړ��C��]�C�X�P�[���ύX
        scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(0, 3600, Mathf.Pow((elapsedTime - 1)/3, 0.3f));
        scoreSetTf.position = 
            Vector3.Lerp(startPos, targetPos, lerpValue) + 
            Vector3.up * curveScorePosY.Evaluate(lerpValue) * 3;
        scoreSetTf.localScale = Vector3.Lerp(Vector3.one, targetScale, lerpValue);

        //�X�N���[���J�o�[�̐F��ύX
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UI����]
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, elapsedTime - 1);
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * 120, Vector3.zero, elapsedTime - 2);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 120, Vector3.zero, elapsedTime - 2);

        if (elapsedTime < 3) return;

        //�e�L�X�g�\��
        if (!isSetText)
        {
            //���s��1��̂�
            isSetText = true;

            //�l���o���l�ʂ̕\��
            //�g���[�j���O���[�h�������̂ݗ�O����
            if (GM.score == 5000 && GM.isTraining)
                addExpText.SetText("+" + addExp + "Exp�i�����{�[�i�X+" + GM.level * 50 + "Exp�j");
            else
                addExpText.SetText("+" + addExp + "Exp");

            //SE�̍Đ�
            AM.audioSource_SE.PlayOneShot(AM.SE_GetExp);

            if (!GM.isTraining)
            {
                //�X�R�A�ϓ��\��
                if (GM.highScore > beforeHighScore)
                    highScoreFluctuationText.SetText("<color=#00951F>+" + (GM.highScore - beforeHighScore) + "</color>");
                else highScoreFluctuationText.SetText("");
                if (GM.GetPlayerScore() > beforePlayerScore)
                    playerScoreFluctuationText.SetText("<color=#00951F>��" + (GM.GetPlayerScore() - beforePlayerScore) + "</color>");
                else if (GM.GetPlayerScore() < beforePlayerScore)
                    playerScoreFluctuationText.SetText("<color=#BA3C46>��" + (beforePlayerScore - GM.GetPlayerScore()) + "</color>");
                else playerScoreFluctuationText.SetText("<color=\"black\">�}0</color>");
            }
        }

        //�\�����郉���N�A�b�v�܂łɕK�v�Ȍo���l�ʂ��Z�o
        float displayAddExpF = Mathf.Lerp(0, addExp, Mathf.Pow(elapsedTime - 3, 0.35f));
        float displayRequiredExpF = beforePlayRequiredExp - (int)displayAddExpF;

        //�\���v���C���[�����N�̍X�V
        int displayRank = beforePlayRank;
        int levelUpCount_local = 0;
        while (displayRequiredExpF < 0)
        {
            displayRank++;
            displayRequiredExpF += (displayRank + 1) * 100;
            playerRankText.SetText("�v���C���[�����N�@" + beforePlayRank + " >> <b>" + displayRank + "</b>");
            levelUpCount_local++;
            if (levelUpCount_local > levelUpCount_state)
            {
                //SE�̍Đ�
                AM.audioSource_SE.PlayOneShot(AM.SE_LevelUp);
                levelUpCount_state++;
            }
        }

        //�o���l�Ɋւ���\���̍X�V
        requiredExpText.SetText("" + (int)displayRequiredExpF);
        playerRankScaleRtf.localScale = Vector3.one - Vector3.right * (displayRequiredExpF / (float)((displayRank+1) * 100));

        //�����L���O���[�h�Ȃ�X�R�A�\���̕ϓ�
        if (!GM.isTraining)
        {
            highScoreText.SetText("" + (int)Mathf.Lerp(beforeHighScore, GM.highScore, elapsedTime - 3));
            playerScoreText.SetText("" + (int)Mathf.Lerp(beforePlayerScore, newPlayerScore, elapsedTime - 3));
        }

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime >= 5)
        {
            if (GM.newSkinQueue.Count > 0) stateMachine.ChangeState(stateMachine.state_UnlockSkin);
            else stateMachine.ChangeState(stateMachine.state_Result);
        }
    }



    public override void Exit()
    {
        //�n�`�̒�~
        TerrainManager.Instance.SetSpeed(0);

        //��ʃ^�b�v���o�{�^���̖�����
        IM.InputUISetActive_Screen(false);

        //�X�R�A�{�[�h�̉�]�ʂ����Z�b�g
        scoreSetTf.eulerAngles = Vector3.zero;

        //�v���C���[�����N�Ɋւ���\�����m��
        if (beforePlayRank != GM.playerRank)
            playerRankText.SetText("�v���C���[�����N�@" + beforePlayRank + " >> <b>" + GM.playerRank + "</b>");
        requiredExpText.SetText("" + GM.requiredExp);
        playerRankScaleRtf.localScale = Vector3.one - Vector3.right * (GM.requiredExp / (float)((GM.playerRank + 1) * 100));

        //SE�̒�~
        AM.audioSource_SE.Stop();
    }
}
