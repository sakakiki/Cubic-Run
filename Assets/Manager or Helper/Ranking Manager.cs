using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public static int RankingCount = 10;   //ランキングの表示数

    private static float rankingUpdateSpan = 60;   //ハイスコアランキング更新の最短間隔
    private static float rankingUpdateTime_highScore = 0;
    private static float rankingUpdateTime_playerScore = 0;

    public enum RankingType
    {
        HighScore,
        PlayerScore
    }

    public static List<(string name, int score, int experience, int skin)> rankingList_highScore
        = new List<(string name, int score, int experience, int skin)>();
    public static List<(string name, int score, int experience, int skin)> rankingList_playerScore
        = new List<(string name, int score, int experience, int skin)>();
    public static (int userRank, float userPercentile) userRankData_highScore = (0, 100);
    public static (int userRank, float userPercentile) userRankData_playerScore = (0, 100);



    //ランキング更新の必要性を確認
    public static async Task CheckUpdateNecessity(RankingType rankingType)
    {
        //前回ランキング更新から指定時間以上経過していなければ何もしない
        switch (rankingType)
        {
            case RankingType.HighScore:
                if (Time.time - rankingUpdateTime_highScore < rankingUpdateSpan) return;
                break;

            case RankingType.PlayerScore:
                if (Time.time - rankingUpdateTime_playerScore < rankingUpdateSpan) return;
                break;
        }

        //ランキングの更新
        await UpdateRanking(rankingType);
    }



    //ランキングを取得し更新
    public static async Task<bool> UpdateRanking(RankingType rankingType)
    {
        switch (rankingType)
        {
            case RankingType.HighScore:

                //トップ10のリストを取得
                List<(string name, int score, int experience, int skin)> newList_highScore
                    = await FirestoreManager.Instance.GetTop10Ranking("highScore");

                //正常に取得できていれば更新
                if (newList_highScore.Count == RankingCount)
                    RankingManager.rankingList_highScore = newList_highScore;

                //ユーザーの順位と上位パーセントの取得
                (int rank, float percentile) newUserData_highScore =
                    await FirestoreManager.Instance.GetUserRanking("highScore", GameManager.Instance.highScore);

                //正常に取得できていれば更新
                if (newUserData_highScore.rank != -1)
                    userRankData_highScore = newUserData_highScore;
                //そうでなければエラーを返す
                else return false;

                //ランキング更新時間を記憶
                rankingUpdateTime_highScore = Time.time;

                return true;



            case RankingType.PlayerScore:

                //トップ10のリストを取得
                List<(string name, int score, int experience, int skin)> newList_playerScore
                    = await FirestoreManager.Instance.GetTop10Ranking("playerScore");

                //正常に取得できていれば更新
                if (newList_playerScore.Count == RankingCount)
                    rankingList_playerScore = newList_playerScore;

                //ユーザーの順位と上位パーセントの取得
                (int rank, float percentile) newUserData_playerScore =
                    await FirestoreManager.Instance.GetUserRanking("playerScore", GameManager.Instance.GetPlayerScore());

                //正常に取得できていれば更新
                if (newUserData_playerScore.rank != -1)
                    userRankData_playerScore = newUserData_playerScore;
                //そうでなければエラーを返す
                else return false;

                //ランキング更新時間を記憶
                rankingUpdateTime_playerScore = Time.time;

                return true;

            default: return false;
        }
    }
}
