using Firebase.Firestore;
using Firebase.Auth;
using Firebase;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }

    private GameManager GM;
    private FirebaseFirestore db;
    private FirebaseAuth auth;

    private const int playerNameMaxLength = 12; // 全角2文字、半角1文字としてカウントする最大文字数




    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Firestoreのオフラインキャッシュを有効化 
        db.Settings.PersistenceEnabled = true;

        //GameManagerの格納
        GM = GameManager.Instance;
    }



    /// <summary>
    /// プレイヤー名を保存
    /// [オンライン専用]
    /// </summary>
    public async Task<string> SavePlayerName(string playerName)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");

            return "異常終了";
        }

        //新しいプレイヤー名が長すぎれば拒否
        if (CalculateTextLength(playerName) > playerNameMaxLength)
            return "プレイヤー名が長すぎます。短いプレイヤー名で再度お試しください。";

        try
        {
            // ネットワーク接続をチェック（オフラインなら即エラーを返す）
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return "ネットワークが無効またはオフラインのため、プレイヤー名を保存できませんでした。";
            }

            // Firebaseのネットワークを明示的に有効化（オフラインならエラーが出る）
            await db.DisableNetworkAsync();
            await db.EnableNetworkAsync();

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            await docRef.SetAsync(new { name = playerName }, SetOptions.MergeAll);

            //ローカルにも反映
            GM.playerName = playerName;

            return "正常終了";
        }
        catch (FirebaseException e)
        {
            return "ネットワークが無効またはオフラインのため、プレイヤー名を保存できませんでした。";
        }
        catch (Exception e)
        {
            return "異常終了";
        }
    }

    /// <summary>
    /// 全角を2、半角を1として文字数を計算する
    /// </summary>
    int CalculateTextLength(string text)
    {
        int count = 0;
        foreach (char c in text)
        {
            count += IsFullWidth(c) ? 2 : 1;
        }
        return count;
    }

    /// <summary>
    /// 全角文字かどうかを判定
    /// </summary>
    bool IsFullWidth(char c)
    {
        return Regex.IsMatch(c.ToString(), @"[^\x00-\x7F]"); // ASCII外の文字を全角と判定
    }

    /// <summary>
    /// 獲得経験値の加算を保存
    /// [オフライン対応]
    /// 累積加算
    /// </summary>
    public async Task SaveExperience(int additionalExp)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "experience", FieldValue.Increment(additionalExp) }
        });
        }
        catch (Exception e)
        {
            Debug.LogError("経験値の保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// ハイスコアを保存
    /// [オンライン専用]
    /// 現在のスコアより高い場合のみ更新
    /// </summary>
    public async Task SaveHighScore(int newScore)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            await db.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                int currentScore = snapshot.Exists ? snapshot.GetValue<int>("highScore") : 0;

                if (newScore > currentScore)
                {
                    transaction.Set(docRef, new { highScore = newScore }, SetOptions.MergeAll);
                }
            });

            //クラウドに未反映のフラグをおろす
            GM.isUnsavedHighScore = false;
            UnsavedHighScoreFlagManager.Save(false);
        }
        catch (Exception e)
        {
            Debug.LogError("ハイスコアの保存に失敗: " + e.Message);

            //クラウドに未反映のフラグを立てる
            GM.isUnsavedHighScore = true;
            UnsavedHighScoreFlagManager.Save(true);
        }
    }

    /// <summary>
    /// プレイヤースコアの保存
    /// [オンライン専用]
    /// Queue<int> rankingScoreQueue 内のデータを全てクラウドにデータを反映させる
    /// </summary>
    public async Task SavePlayerScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        //更新後のプレイヤースコア
        int newPlayerScore = 0;

        try
        {
            // Queueのコピー（トランザクション外で安全にデータを取得）
            List<int> tempScores = GM.rankingScoreQueue.ToList();

            bool transactionSuccess = await db.RunTransactionAsync(async transaction =>
            {
                // プレイヤースコアの取得
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                newPlayerScore = snapshot.Exists ? snapshot.GetValue<int>("playerScore") : 0;

                // 未反映のデータを反映
                foreach (int resultScore in tempScores)
                {
                    newPlayerScore += (resultScore - newPlayerScore) / 10;
                }

                transaction.Set(docRef, new { playerScore = newPlayerScore }, SetOptions.MergeAll);
                return true;
            });

            // トランザクションが成功した場合のみ
            if (transactionSuccess)
            {
                // GM の Queue を空にする
                GM.rankingScoreQueue.Clear();

                //ローカルデータに反映
                GM.playerScore = newPlayerScore;

                // ローカルの Queue の内容を更新
                RankingScoreManager.Save(GM.rankingScoreQueue);
                Debug.Log("Queueの情報をローカルストレージに保存しました");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"プレイヤースコアの更新に失敗: {e.Message}");

            // ローカルの Queue の内容を更新（失敗時でも保存する）
            RankingScoreManager.Save(GM.rankingScoreQueue);
            Debug.Log("Queueの情報をローカルストレージに保存しました");
        }
    }

    /// <summary>
    /// トレーニングモードレベルクリア回数の加算を保存
    /// [オフライン対応]
    /// 回数1加算
    /// </summary>
    public async Task SaveTrainingClearCount(int level)
    {
        //レベルをインデックスに補正
        level--;

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        string fieldName = $"trainingCount.{level}";

        try
        {
            // 指定レベルのカウントを1増やす
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { fieldName, FieldValue.Increment(1) }
        });
        }
        catch (Exception e)
        {
            Debug.LogError($"レベル {level} のクリア回数の保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// 使用中のスキンを保存
    /// [オフライン対応]
    /// オフライン実行時はサーバー側への更新が待機され、後からの更新を上書きすることがある
    /// </summary>
    public async Task SaveUsingSkin()
    {
        //GameManagerの値を採用
        int skinID = GM.usingSkinID;

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            // 保存データ
            await docRef.SetAsync(new { usingSkin = skinID }, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Debug.LogError("使用中スキンの保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// 走行距離の加算を保存
    /// [オフライン対応]
    /// 累積加算
    /// </summary>
    public async Task SaveRunDistance(int additionalDistance)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "runDistance", FieldValue.Increment(additionalDistance) }
        });
        }
        catch (Exception e)
        {
            Debug.LogError("走行距離の保存に失敗: " + e.Message);
        }
    }



    /// <summary>
    /// 全てのデータをロード
    /// [クラウドデータ優先]
    /// ローカルデータとの比較も含む
    /// </summary>
    public async Task LoadAll()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            #region snapshotを取得（クラウドを優先し、失敗すればキャッシュを取得）

            DocumentSnapshot snapshot;

            try
            {
                // クラウドの最新データを取得（通信が発生）
                snapshot = await docRef.GetSnapshotAsync(Source.Server);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"クラウドデータの取得に失敗: {e.Message}。キャッシュデータを使用します。");

                // クラウド取得に失敗した場合、キャッシュから取得（オフライン対応）
                snapshot = await docRef.GetSnapshotAsync(Source.Cache);
            }

            //データの存在をチェック
            if (!snapshot.Exists)
            {
                Debug.LogWarning("[Firestore] プレイヤーデータが存在しません");
                return;
            }
            #endregion

            #region playerNameのロード
            if (snapshot.ContainsField("name"))
            {
                //ローカルキャッシュに同期
                GM.playerName = snapshot.GetValue<string>("name");
            }
            #endregion

            #region playerExpのロード
            if (snapshot.ContainsField("experience"))
            {
                //ローカルキャッシュに同期
                GM.totalExp = snapshot.GetValue<int>("experience");
            }
            #endregion

            #region highScoreのロード

            int serverHighScore = 0;

            if (snapshot.Exists && snapshot.ContainsField("highScore"))
            {
                serverHighScore = snapshot.GetValue<int>("highScore");
            }

            // クラウドデータとローカルデータの値を比較して高い方をローカルに同期
            GM.highScore = Math.Max(GM.highScore, serverHighScore);
            #endregion

            #region playerExpのロード
            if (snapshot.ContainsField("playerScore"))
            {
                //ローカルキャッシュに同期
                GM.playerScore = snapshot.GetValue<int>("playerScore");
            }
            #endregion

            #region trainingCountのロード
            if (snapshot.ContainsField("trainingCount"))
            {
                Dictionary<string, object> rawData = snapshot.GetValue<Dictionary<string, object>>("trainingCount");

                // Firestoreの辞書データを List<int> に変換
                foreach (var kvp in rawData)
                {
                    int index = int.Parse(kvp.Key); // keyは "0", "1", "2", ... のような文字列なので変換
                    int value = Convert.ToInt32(kvp.Value);

                    // 必要ならリストを拡張
                    while (GM.trainingClearCounts.Count <= index)
                    {
                        GM.trainingClearCounts.Add(0);
                    }

                    GM.trainingClearCounts[index] = value;
                }
            }
            #endregion

            #region usingSkinのロード
            if (snapshot.ContainsField("usingSkin"))
            {
                //ローカルキャッシュに同期
                GM.usingSkinID = snapshot.GetValue<int>("usingSkin");
            }
            #endregion

            #region runDistanceのロード
            if (snapshot.ContainsField("runDistance"))
            {
                //ローカルキャッシュに同期
                GM.totalRunDistance = snapshot.GetValue<int>("runDistance");
            }
            #endregion

        }
        catch (Exception e)
        {
            Debug.LogError("データのロードに失敗: " + e.Message);
        }

        return;
    }



    /// <summary>
    /// Firestoreから指定したスコアランキングを取得
    /// </summary>
    /// <param name="scoreType">"highScore" または "playerScore"</param>
    /// <returns>ランキング上位10名のリスト</returns>
    public async Task<List<(string name, int score, int experience, int skin)>> GetTop10Ranking(string scoreType)
    {
        //ランキングのリスト
        List<(string name, int score, int experience, int skin)> ranking
            = new List<(string name, int score, int experience, int skin)>();

        try
        {
            Query query = db.Collection("users")
                .OrderByDescending(scoreType)
                .Limit(RankingManager.RankingCount);

            QuerySnapshot snapshot = await query.GetSnapshotAsync(Source.Server);

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                string name = doc.ContainsField("name") ? doc.GetValue<string>("name") : "Unknown";
                int score = doc.ContainsField(scoreType) ? doc.GetValue<int>(scoreType) : 0;
                int experience = doc.ContainsField("experience") ? doc.GetValue<int>("experience") : 0;
                int skin = doc.ContainsField("usingSkin") ? doc.GetValue<int>("usingSkin") : 0;

                ranking.Add((name, score, experience, skin));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Firestore] ランキング取得に失敗: {e.Message}");
        }

        //取得失敗時はListを初期値で埋める
        while(ranking.Count < RankingManager.RankingCount)
            ranking.Add(("Unknown", 0, 0, 0));

        return ranking;
    }
    
    
    
    /// <summary>
    /// Firestoreからユーザーの順位と上位何%かを取得
    /// </summary>
    /// <param name="scoreType">"highScore" または "playerScore"</param>
    /// <param name="userScore">ユーザーのスコア</param>
    /// <returns>(順位, 上位%)</returns>
    public async Task<(int rank, float percentile)> GetUserRanking(string scoreType, int userScore)
    {
        int rank = -1;  //順位が-1ならエラー
        float percentile = 0;

        try
        {
            // ユーザーよりスコアが高い数を取得
            Query countQuery = db.Collection("users").WhereGreaterThan(scoreType, userScore);
            QuerySnapshot countSnapshot = await countQuery.GetSnapshotAsync(Source.Server);
            int higherCount = countSnapshot.Count;

            // 総ユーザー数を取得
            Query totalQuery = db.Collection("users");
            QuerySnapshot totalSnapshot = await totalQuery.GetSnapshotAsync(Source.Server);
            int totalUsers = totalSnapshot.Count;

            // ユーザーの順位とパーセンタイル計算
            rank = higherCount + 1;
            percentile = (float)rank / (float)totalUsers * 100;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Firestore] ユーザー順位取得に失敗: {e.Message}");
        }

        return (rank, percentile);
    }




    /// <summary>
    /// 新規データ作成
    /// </summary>
    public async Task<bool> SaveNewPlayerData()
    {
        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        Dictionary<string, object> newData = new Dictionary<string, object>
    {
        { "name", "Noname" },
        { "experience", 0 },
        { "highScore", 0 },
        { "playerScore", 0 },
        { "trainingCount", new Dictionary<string, object> { { "0", 0 } } },
        { "usingSkin", 0 },
        { "runDistance", 0 }
    };

        try
        {
            await docRef.SetAsync(newData);

            #region ローカルデータも初期化
            HighScoreManager.Save(0);
            RankingScoreManager.Save(new Queue<int>());
            UnsavedHighScoreFlagManager.Save(false);
            GM.highScore = 0;
            GM.rankingScoreQueue.Clear();
            #endregion

            //正常終了
            return true;
        }
        catch (Exception e)
        {
            //異常終了
            return false;
        }
    }



    /// <summary>
    /// データ削除
    /// </summary>
    public async Task DeleteDocument(string documentId)
    {
        try
        {
            DocumentReference docRef = db.Collection("users").Document(documentId);
            await docRef.DeleteAsync();
            Debug.Log($"ドキュメント {documentId} を削除しました");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ドキュメント削除エラー: {e.Message}");
            Debug.Log(documentId);
        }
    }
}
