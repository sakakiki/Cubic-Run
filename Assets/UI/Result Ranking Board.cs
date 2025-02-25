using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultRankingBoard: MonoBehaviour
{
    private static GameManager GM;
    [SerializeField] RectTransform contentFieldRtf_highScore;
    [SerializeField] RectTransform contentFieldRtf_playerScore;
    private List<(string name, int score, int experience, int skin)> rankingList_highScore
        = new List<(string name, int score, int experience, int skin)>();
    private List<(string name, int score, int experience, int skin)> rankingList_playerScore
        = new List<(string name, int score, int experience, int skin)>();
    private (int userRank, float userPercentile) userRankData_highScore = (1, 0);
    private (int userRank, float userPercentile) userRankData_playerScore = (1, 0);
    [SerializeField] private TextMeshProUGUI[] scoreTexts_highScore;
    [SerializeField] private TextMeshProUGUI[] scoreTexts_playerScore;
    [SerializeField] private Image[] modelImage_highScore;
    [SerializeField] private Image[] modelImage_playerScore;
    [SerializeField] private Image[] crystalImage_highScore;
    [SerializeField] private Image[] crystalImage_playerScore;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts_highScore;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts_playerScore;
    [SerializeField] private TextMeshProUGUI[] playerRankTexts_highScore;
    [SerializeField] private TextMeshProUGUI[] playerRankTexts_playerScore;
    [SerializeField] private Image[] panelCovers_highScore;
    [SerializeField] private Image[] panelCovers_playerScore;
    [SerializeField] private TextMeshProUGUI userRankPercentileText_highScore;
    [SerializeField] private TextMeshProUGUI userRankPercentileText_playerScore;

    public void Start()
    {
        GM = GameManager.Instance;
    }



    //���̎擾�E�\���̍X�V
    public async Task UpdateRanking()
    {
        #region �����L���O�擾
        //�g�b�v10�̃��X�g���擾
        List<(string name, int score, int experience, int skin)> newList_highScore
            = await FirestoreManager.Instance.GetTop10Ranking("highScore");
        List<(string name, int score, int experience, int skin)> newList_playerScore
            = await FirestoreManager.Instance.GetTop10Ranking("playerScore");
        //����Ɏ擾�ł��Ă�΍X�V
        if (newList_highScore.Count == playerNameTexts_highScore.Length)
            rankingList_highScore = newList_highScore;
        if (newList_playerScore.Count == playerNameTexts_playerScore.Length)
            rankingList_playerScore = newList_playerScore;

        //���[�U�[�̏��ʂƏ�ʃp�[�Z���g�̎擾
        (int rank, float percentile) newUserData_highScore =
            await FirestoreManager.Instance.GetUserRanking("highScore", GM.highScore);
        (int rank, float percentile) newUserData_playerScore =
            await FirestoreManager.Instance.GetUserRanking("playerScore", GM.GetPlayerScore());
        //����Ɏ擾�ł��Ă�΍X�V
        if (newUserData_highScore.rank != -1)
            userRankData_highScore = newUserData_highScore;
        if (newUserData_playerScore.rank != -1)
            userRankData_playerScore = newUserData_playerScore;
        #endregion

        //�����L���O�\���ʒu��1�ʂ̏ꏊ�ɖ߂�
        contentFieldRtf_highScore.localPosition = Vector2.zero;
        contentFieldRtf_playerScore.localPosition = Vector2.zero;

        //�p�l���ɐF�����Ă���΃��Z�b�g
        for (int i = 0; i < panelCovers_highScore.Length; i++)
            panelCovers_highScore[i].color = Color.clear;
        for (int i = 0; i < panelCovers_playerScore.Length; i++)
            panelCovers_playerScore[i].color = Color.clear;

        #region �����L���O���̔��f
        //�g�b�v10�����L���O�̔��f
        for (int i = 0; rankingList_highScore.Count > i; i++)
        {
            scoreTexts_highScore[i].SetText(rankingList_highScore[i].score.ToString());
            playerNameTexts_highScore[i].SetText(rankingList_highScore[i].name);
            modelImage_highScore[i].sprite = (rankingList_highScore[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
            modelImage_highScore[i].color = SkinDataBase.Instance.skinData[rankingList_highScore[i].skin].skinColor;
            crystalImage_highScore[i].enabled = (rankingList_highScore[i].skin == 7 || rankingList_highScore[i].skin == 15);
            playerRankTexts_highScore[i].SetText(GM.CalculatePlayerRank(rankingList_highScore[i].experience).ToString());
        }
        for (int i = 0; rankingList_playerScore.Count > i; i++)
        {
            scoreTexts_playerScore[i].SetText(rankingList_playerScore[i].score.ToString());
            playerNameTexts_playerScore[i].SetText(rankingList_playerScore[i].name);
            modelImage_playerScore[i].sprite = (rankingList_playerScore[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
            modelImage_playerScore[i].color = SkinDataBase.Instance.skinData[rankingList_playerScore[i].skin].skinColor;
            crystalImage_playerScore[i].enabled = (rankingList_playerScore[i].skin == 7 || rankingList_playerScore[i].skin == 15);
            playerRankTexts_playerScore[i].SetText(GM.CalculatePlayerRank(rankingList_playerScore[i].experience).ToString());
        }

        //���[�U�[�̕\�����X�V
        userRankPercentileText_highScore.SetText(
            userRankData_highScore.userRank + "��\n���" +
            userRankData_highScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");
        userRankPercentileText_playerScore.SetText(
            userRankData_playerScore.userRank + "��\n���" +
            userRankData_playerScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

        //���[�U�[���g�b�v10�ɓ����Ă���΃p�l����ڗ�������
        if (userRankData_highScore.userRank <= rankingList_highScore.Count)
            panelCovers_highScore[userRankData_highScore.userRank - 1].color 
                = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;
        if (userRankData_playerScore.userRank <= rankingList_playerScore.Count)
            panelCovers_playerScore[userRankData_playerScore.userRank - 1].color 
                = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;
        #endregion
    }
}
