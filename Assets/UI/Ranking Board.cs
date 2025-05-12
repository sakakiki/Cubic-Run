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

    //タップ：タップ時に実行
    //マウス：クリック時に実行
    public async void OnPointerDown(PointerEventData eventData)
    {
        //メニュー画面でなければ何もしない
        if (gameStateMachine.currentState != gameStateMachine.state_Menu) return;

        //SEの再生
        AudioManager.Instance.PlaySE(AudioManager.SE.Close);

        //このスクリプトを無効化
        this.enabled = false;

        //必要に応じてランキングを取得
        await RankingManager.CheckUpdateNecessity(RankingType);

        //ランキング表示を更新
        UpdateRankingDisplay();

        //文字を濃く
        tabText.color = Color.black;

        //他方のスクリプトを有効化
        anotherTab.enabled = true;
    }



    //表示の更新
    public void UpdateRankingDisplay()
    {
        //スクリプトが有効状態なら何もしない
        if (this.enabled) return;

        //ランキング表示位置を1位の場所に戻す
        contentFieldRtf.localPosition = Vector2.zero;

        //パネルに色がついていればリセット
        for (int i = 0; i < panelCovers.Length; i++)
            panelCovers[i].color = Color.clear;

        //ランキング表示更新
        switch (RankingType)
        {
            case RankingType.HighScore:

                //トップ10ランキングの反映
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

                //ユーザーの表示を更新
                scoreNameText.SetText("ハイスコア");
                userScoreText.SetText((GM.highScore).ToString());
                userRankPercentileText.SetText(
                    RankingManager.userRankData_highScore.userRank + "位\n上位" +
                    RankingManager.userRankData_highScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

                //ユーザーがトップ10に入っていればパネルを目立たせる
                if (RankingManager.userRankData_highScore.userRank <= RankingManager.rankingList_highScore.Count
                    && RankingManager.userRankData_highScore.userRank > 0)
                    panelCovers[RankingManager.userRankData_highScore.userRank - 1].color 
                        = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;

                break;



            case RankingType.PlayerScore:

                //トップ10ランキングの反映
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

                //ユーザーの表示を更新
                scoreNameText.SetText("プレイヤースコア");
                userScoreText.SetText(GM.GetPlayerScore().ToString());
                userRankPercentileText.SetText(
                    RankingManager.userRankData_playerScore.userRank + "位\n上位" +
                    RankingManager.userRankData_playerScore.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

                //ユーザーがトップ10に入っていればパネルを目立たせる
                if (RankingManager.userRankData_playerScore.userRank <= RankingManager.rankingList_playerScore.Count
                    && RankingManager.userRankData_playerScore.userRank > 0)
                    panelCovers[RankingManager.userRankData_playerScore.userRank - 1].color
                        = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;

                break;
        }
    }



    //スクリプトが有効化（選択解除）時
    private void OnEnable()
    {
        //文字を薄く
        tabText.color = Color.gray;
    }
}
