using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static RankingManager;

public class RankingBoard: MonoBehaviour, IPointerDownHandler
{
    private static GameManager GM;
    private static GameStateStateMachine gameStateMachine;
    [SerializeField] RectTransform contentFieldRtf;
    [SerializeField] private RankingType RankingType;
    [SerializeField] private RankingBoard anotherTab;
    [SerializeField] private TextMeshProUGUI tabText;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private Image[] modelImage;
    [SerializeField] private Image[] crystalImage;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts;
    [SerializeField] private TextMeshProUGUI[] playerRankTexts;
    [SerializeField] private Image[] panelCovers;
    [SerializeField] private TextMeshProUGUI scoreNameText;
    [SerializeField] private TextMeshProUGUI userScoreText;
    [SerializeField] private TextMeshProUGUI userRankPercentileText;

    public void Start()
    {
        if (GM == null) 
            GM = GameManager.Instance;
        if (gameStateMachine == null)
            gameStateMachine = GameManager.Instance.gameStateMachine;
    }

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public async void OnPointerDown(PointerEventData eventData)
    {
        //���j���[��ʂłȂ���Ή������Ȃ�
        if (gameStateMachine.currentState != gameStateMachine.state_Menu) return;

        //SE�̍Đ�
        AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Panel);

        //���̃X�N���v�g�𖳌���
        this.enabled = false;

        await UpdateRanking();

        //������Z��
        tabText.color = Color.black;

        //�����̃X�N���v�g��L����
        anotherTab.enabled = true;
    }



    //���̎擾�E�\���̍X�V
    public async Task UpdateRanking()
    {
        //�X�N���v�g���L����ԂȂ牽�����Ȃ�
        if (this.enabled) return;

        //�p�x�����t�������L���O�X�V
        await RankingManager.CheckUpdateNecessity(RankingType);

        //�����L���O�\���ʒu��1�ʂ̏ꏊ�ɖ߂�
        contentFieldRtf.localPosition = Vector2.zero;

        //�p�l���ɐF�����Ă���΃��Z�b�g
        for (int i = 0; i < panelCovers.Length; i++)
            panelCovers[i].color = Color.clear;

        //�����L���O�\���X�V
        switch (RankingType)
        {
            case RankingType.HighScore:

                //�g�b�v10�����L���O�̔��f
                for (int i = 0; RankingManager.rankingList_highScore.Count > i; i++)
                {
                    scoreTexts[i].SetText(RankingManager.rankingList_highScore[i].score.ToString());
                    playerNameTexts[i].SetText(RankingManager.rankingList_highScore[i].name);
                    modelImage[i].sprite = (RankingManager.rankingList_highScore[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
                    modelImage[i].color = SkinDataBase.Instance.skinData[RankingManager.rankingList_highScore[i].skin].skinColor;
                    crystalImage[i].enabled 
                        = (RankingManager.rankingList_highScore[i].skin == 7 || RankingManager.rankingList_highScore[i].skin == 15);
                    playerRankTexts[i].SetText(GM.CalculatePlayerRank(RankingManager.rankingList_highScore[i].experience).ToString());
                }

                //���[�U�[�̕\�����X�V
                scoreNameText.SetText("�n�C�X�R�A");
                userScoreText.SetText((GM.highScore).ToString());
                userRankPercentileText.SetText(
                    RankingManager.userRankData_highScore.userRank + "��\n���" +
                    RankingManager.userRankData_highScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

                //���[�U�[���g�b�v10�ɓ����Ă���΃p�l����ڗ�������
                if (RankingManager.userRankData_highScore.userRank <= RankingManager.rankingList_highScore.Count
                    && RankingManager.userRankData_highScore.userRank > 0)
                    panelCovers[RankingManager.userRankData_highScore.userRank - 1].color 
                        = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;

                break;



            case RankingType.PlayerScore:

                //�g�b�v10�����L���O�̔��f
                for (int i = 0; RankingManager.rankingList_playerScore.Count > i; i++)
                {
                    scoreTexts[i].SetText(RankingManager.rankingList_playerScore[i].score.ToString());
                    playerNameTexts[i].SetText(RankingManager.rankingList_playerScore[i].name);
                    modelImage[i].sprite = (RankingManager.rankingList_playerScore[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
                    modelImage[i].color = SkinDataBase.Instance.skinData[RankingManager.rankingList_playerScore[i].skin].skinColor;
                    crystalImage[i].enabled
                        = (RankingManager.rankingList_playerScore[i].skin == 7 || RankingManager.rankingList_playerScore[i].skin == 15);
                    playerRankTexts[i].SetText(GM.CalculatePlayerRank(RankingManager.rankingList_playerScore[i].experience).ToString());
                }

                //���[�U�[�̕\�����X�V
                scoreNameText.SetText("�v���C���[�X�R�A");
                userScoreText.SetText(GM.GetPlayerScore().ToString());
                userRankPercentileText.SetText(
                    RankingManager.userRankData_playerScore.userRank + "��\n���" +
                    RankingManager.userRankData_playerScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

                //���[�U�[���g�b�v10�ɓ����Ă���΃p�l����ڗ�������
                if (RankingManager.userRankData_playerScore.userRank <= RankingManager.rankingList_playerScore.Count
                    && RankingManager.userRankData_playerScore.userRank > 0)
                    panelCovers[RankingManager.userRankData_playerScore.userRank - 1].color
                        = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;

                break;
        }
    }



    //�X�N���v�g���L�����i�I�������j��
    private void OnEnable()
    {
        //�����𔖂�
        tabText.color = Color.gray;
    }
}
