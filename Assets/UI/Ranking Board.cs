using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RankingBoard: MonoBehaviour, IPointerDownHandler
{
    private static GameManager GM;
    private static GameStateStateMachine gameStateMachine;
    private float rankingUpdateTime = -10;  //rankingUpdateSpan * -1 にする
    private float rankingUpdateSpan = 10;   //ランキング更新の最短間隔
    [SerializeField] RectTransform contentFieldRtf;
    [SerializeField] private string scoreType;
    [SerializeField] private RankingBoard anotherTab;
    [SerializeField] private TextMeshProUGUI tabText;
    private List<(string name, int score, int experience, int skin)> rankingList 
        = new List<(string name, int score, int experience, int skin)>();
    private (int userRank, float userPercentile) userRankData = (1, 0);
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

        //このスクリプトを無効化
        this.enabled = false;

        await UpdateRanking();

        //文字を濃く
        tabText.color = Color.black;

        //他方のスクリプトを有効化
        anotherTab.enabled = true;
    }



    //情報の取得・表示の更新
    public async Task UpdateRanking()
    {
        //スクリプトが有効状態なら何もしない
        if (this.enabled) return;

        //前回ランキング更新から指定時間以上経過していれば情報を再取得
        if (Time.time - rankingUpdateTime > rankingUpdateSpan)
        {
            //ランキング更新時刻を更新
            rankingUpdateTime = Time.time;

            //トップ10のリストを取得
            List<(string name, int score, int experience, int skin)> newList
                = await FirestoreManager.Instance.GetTop10Ranking(scoreType);
            //正常に取得できてれば更新
            if (newList.Count == playerNameTexts.Length)
                rankingList = newList;

            //ユーザーの順位と上位パーセントの取得
            (int rank, float percentile) newUserData =
                await FirestoreManager.Instance.GetUserRanking(
                    scoreType, 
                    scoreType == "highScore" ? GM.highScore : GM.GetPlayerScore());
            //正常に取得できてれば更新
            if (newUserData.rank != -1)
                userRankData = newUserData;
        }

        //ランキング表示位置を1位の場所に戻す
        contentFieldRtf.localPosition = Vector2.zero;

        //パネルに色がついていればリセット
        for (int i = 0; i < panelCovers.Length; i++)
            panelCovers[i].color = Color.clear;

        #region ランキング情報の反映
        //トップ10ランキングの反映
        for (int i = 0; rankingList.Count > i; i++)
        {
            scoreTexts[i].SetText("" + rankingList[i].score);
            playerNameTexts[i].SetText(rankingList[i].name);
            modelImage[i].sprite = (rankingList[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
            modelImage[i].color = SkinDataBase.Instance.skinData[rankingList[i].skin].skinColor;
            crystalImage[i].enabled = (rankingList[i].skin == 7 || rankingList[i].skin == 15);
            playerRankTexts[i].SetText("" + GM.CalculatePlayerRank(rankingList[i].experience));
        }

        //ユーザーの表示を更新
        scoreNameText.SetText(scoreType == "highScore" ? "ハイスコア" : "プレイヤースコア");
        userScoreText.SetText(((scoreType == "highScore") ? GM.highScore : GM.GetPlayerScore()).ToString());
        userRankPercentileText.SetText(
            userRankData.userRank + "位\n上位" + 
            userRankData.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

        //ユーザーがトップ10に入っていればパネルを目立たせる
        if (userRankData.userRank <= rankingList.Count)
            panelCovers[userRankData.userRank-1].color = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;
        #endregion
    }



    //スクリプトが有効化（選択解除）時
    private void OnEnable()
    {
        //文字を薄く
        tabText.color = Color.gray;
    }
}
