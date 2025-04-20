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



    //情報の取得・表示の更新
    public async Task UpdateRanking()
    {
        //ランキング更新
        await RankingManager.UpdateRanking(RankingManager.RankingType.HighScore);
        await RankingManager.UpdateRanking(RankingManager.RankingType.PlayerScore);

        //ランキング表示位置を1位の場所に戻す
        contentFieldRtf_highScore.localPosition = Vector2.zero;
        contentFieldRtf_playerScore.localPosition = Vector2.zero;

        //パネルに色がついていればリセット
        for (int i = 0; i < panelCovers_highScore.Length; i++)
            panelCovers_highScore[i].color = Color.clear;
        for (int i = 0; i < panelCovers_playerScore.Length; i++)
            panelCovers_playerScore[i].color = Color.clear;

        #region ランキング情報の反映
        //トップ10ランキングの反映
        for (int i = 0; RankingManager.rankingList_highScore.Count > i; i++)
        {
            scoreTexts_highScore[i].SetText(RankingManager.rankingList_highScore[i].score.ToString());
            playerNameTexts_highScore[i].SetText(RankingManager.rankingList_highScore[i].name);
            modelImage_highScore[i].sprite = (RankingManager.rankingList_highScore[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
            modelImage_highScore[i].color = SkinDataBase.Instance.skinData[RankingManager.rankingList_highScore[i].skin].skinColor;
            crystalImage_highScore[i].enabled 
                = (RankingManager.rankingList_highScore[i].skin == 7 || RankingManager.rankingList_highScore[i].skin == 15);
            playerRankTexts_highScore[i].SetText(
                GM.CalculatePlayerRank(RankingManager.rankingList_highScore[i].experience).ToString());
        }
        for (int i = 0; RankingManager.rankingList_playerScore.Count > i; i++)
        {
            scoreTexts_playerScore[i].SetText(RankingManager.rankingList_playerScore[i].score.ToString());
            playerNameTexts_playerScore[i].SetText(RankingManager.rankingList_playerScore[i].name);
            modelImage_playerScore[i].sprite 
                = (RankingManager.rankingList_playerScore[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
            modelImage_playerScore[i].color 
                = SkinDataBase.Instance.skinData[RankingManager.rankingList_playerScore[i].skin].skinColor;
            crystalImage_playerScore[i].enabled 
                = (RankingManager.rankingList_playerScore[i].skin == 7 || RankingManager.rankingList_playerScore[i].skin == 15);
            playerRankTexts_playerScore[i].SetText(
                GM.CalculatePlayerRank(RankingManager.rankingList_playerScore[i].experience).ToString());
        }

        //ユーザーの表示を更新
        userRankPercentileText_highScore.SetText(
            RankingManager.userRankData_highScore.userRank + "位\n上位" +
            RankingManager.userRankData_highScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");
        userRankPercentileText_playerScore.SetText(
            RankingManager.userRankData_playerScore.userRank + "位\n上位" +
            RankingManager.userRankData_playerScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

        //ユーザーがトップ10に入っていればパネルを目立たせる
        if (RankingManager.userRankData_highScore.userRank <= RankingManager.rankingList_highScore.Count
                    && RankingManager.userRankData_highScore.userRank > 0)
            panelCovers_highScore[RankingManager.userRankData_highScore.userRank - 1].color 
                = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;
        if (RankingManager.userRankData_playerScore.userRank <= RankingManager.rankingList_playerScore.Count
                    && RankingManager.userRankData_playerScore.userRank > 0)
            panelCovers_playerScore[RankingManager.userRankData_playerScore.userRank - 1].color 
                = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;
        #endregion
    }
}
