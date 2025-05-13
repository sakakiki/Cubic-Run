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
    private const int maxStamina = 3; // 自動回復によるスタミナ最大値（広告視聴で超過回復可能）
    private const int resetStaminaHour = 4; // スタミナ回復時刻

    private DateTime nextOnlineStaminaCheckTime = DateTime.MinValue; // 次回オンラインでスタミナをチェックする時間
    private int staminaCache = 0; // ローカル保存するスタミナ




    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        //GameManagerの格納
        GM = GameManager.Instance;

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Firestoreのオフラインキャッシュを有効化 
        db.Settings.PersistenceEnabled = true;
    }



    /// <summary>
    /// プレイヤー名を保存
    /// [オンライン専用]
    /// </summary>
    public async Task<string> SavePlayerName(string playerName)
    {
        if (auth.CurrentUser == null)
        {
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
    /// 経験値と走行距離をまとめて保存
    /// [オフライン対応]
    /// 両方とも累積加算
    /// </summary>
    public async Task SaveExperienceAndRunDistance(int additionalExp, int additionalDistance)
    {
        if (auth.CurrentUser == null)
            return;

        DocumentReference docRef = db.Collection("users")
                                    .Document(auth.CurrentUser.UserId);

        try
        {
            // 一度の UpdateAsync で両方のフィールドを increment
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "experience", FieldValue.Increment(additionalExp) },
                { "runDistance", FieldValue.Increment(additionalDistance) }
            });
        }
        catch (Exception e)
        {
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
            }
        }
        catch (Exception e)
        {
            // ローカルの Queue の内容を更新（失敗時でも保存する）
            RankingScoreManager.Save(GM.rankingScoreQueue);
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
        }
    }



    /// <summary>
    /// 全てのデータをロード
    /// [クラウドデータ優先]
    /// ローカルデータとの比較も含む
    /// </summary>
    public async Task<bool> LoadAll()
    {
        if (auth.CurrentUser == null)
        {
            return false;
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
                // クラウド取得に失敗した場合、キャッシュから取得（オフライン対応）
                snapshot = await docRef.GetSnapshotAsync(Source.Cache);
            }

            //データの存在をチェック
            if (!snapshot.Exists)
            {
                return false;
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

            #region playerScoreのロード
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
            return false;
        }

        return true;
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
        }

        //取得失敗時はListを初期値で埋める
        while(ranking.Count < RankingManager.RankingCount)
            ranking.Add(("Unknown", 0, 0, 0));

        return ranking;
    }
    
    /// <summary>
    /// Firestoreからユーザーの順位と上位何%かを取得（読み取り回数を最大2回に抑える）
    /// </summary>
    /// <param name="scoreType">"highScore" または "playerScore"</param>
    /// <param name="userScore">ユーザーのスコア</param>
    /// <returns>(順位, 上位%)</returns>
    public async Task<(int rank, float percentile)> GetUserRanking(string scoreType, int userScore)
    {
        int rank = -1;
        float percentile = 0;

        try
        {
            // ユーザーよりスコアが高い件数をCount()クエリで取得（1回の読み取り）
            AggregateQuery countQuery = db.Collection("users")
                .WhereGreaterThan(scoreType, userScore)
                .Count;

            AggregateQuerySnapshot countSnapshot = await countQuery.GetSnapshotAsync(AggregateSource.Server);
            int higherCount = (int)countSnapshot.Count;

            // 総ユーザー数を保存済みの「統計データ」ドキュメントから取得（1回の読み取り）
            DocumentSnapshot metaSnapshot = await db.Collection("metadata").Document("gameInfo").GetSnapshotAsync(Source.Server);
            int totalUsers = metaSnapshot.ContainsField("totalUsers") ? metaSnapshot.GetValue<int>("totalUsers") : 1;

            rank = higherCount + 1;
            percentile = (float)rank / (float)totalUsers * 100;
        }
        catch (Exception e)
        {
            // ログ出力など
        }

        return (rank, percentile);
    }




    /// <summary>
    /// スタミナの最新状態を取得
    /// スタミナ回復・広告視聴回数リセットもここで実行
    /// </summary>
    public async Task<int> CheckResetAndGetStamina()
    {
        // ローカルに保存されたスタミナリセット日時からスタミナ回復の必要があるかをチェック
        DateTime now = DateTime.Now;
        if (now < nextOnlineStaminaCheckTime) 
            return staminaCache;

        if (auth.CurrentUser == null)
        {
            return -1; //異常終了
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            //オンライン専用化
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Server);

            if (!snapshot.ContainsField("stamina") || !snapshot.ContainsField("lastUpdated"))
            {
                //スタミナのデータが存在しなければ最大値で初期化
                await docRef.SetAsync(new { stamina = maxStamina, lastUpdated = Timestamp.GetCurrentTimestamp() }, SetOptions.MergeAll);
            }

            //現在のスタミナを取得
            int currentStamina = snapshot.GetValue<int>("stamina");

            //前回のスタミナ情報更新日時
            Timestamp lastUpdatedTimestamp = snapshot.GetValue<Timestamp>("lastUpdated");
            DateTime lastUpdated = lastUpdatedTimestamp.ToDateTime().ToLocalTime();

            //直近のスタミナ回復日時
            DateTime lastReset = new DateTime(now.Year, now.Month, now.Day, resetStaminaHour, 0, 0, DateTimeKind.Local);
            if (now.Hour < resetStaminaHour)
            {
                lastReset = lastReset.AddDays(-1);
            }

            //前回のスタミナ情報更新が直近のスタミナ回復日時より前ならスタミナ回復
            if (lastUpdated < lastReset)
            {
                int newStamina = Mathf.Max(currentStamina, maxStamina);

                //同時に広告視聴回数もリセット
                await docRef.SetAsync(new { 
                    stamina = newStamina, 
                    lastUpdated = Timestamp.GetCurrentTimestamp(),
                    adWatchCount = 0 }, 
                    SetOptions.MergeAll);

                // 次回のオンラインスタミナ情報取得日時の保存
                nextOnlineStaminaCheckTime = lastReset.AddDays(1);

                // ローカルに保存するスタミナ
                staminaCache = newStamina;

                return newStamina;
            }

            // 次回のオンラインスタミナ情報取得日時の保存
            nextOnlineStaminaCheckTime = lastReset.AddDays(1);

            // ローカルに保存するスタミナ
            staminaCache = currentStamina;

            return currentStamina;
        }
        catch (Exception e)
        {
            return -1; //異常終了
        }
    }

    /// <summary>
    /// スタミナを消費
    /// </summary>
    public async Task<int> UseStamina()
    {
        if (auth.CurrentUser == null)
        {
            return -2; //異常終了
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            int returnValue = -2; // 変更が無ければ異常終了として扱う

            await db.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                if (!snapshot.Exists) return false;

                int currentStamina = snapshot.GetValue<int>("stamina");
                if (currentStamina <= 0)
                {
                    returnValue = -1;
                    return true;
                }

                transaction.Set(docRef, new { stamina = currentStamina - 1 }, SetOptions.MergeAll);
                returnValue = currentStamina - 1;
                return true;
            });

            staminaCache = (int)MathF.Max(0, returnValue); // ローカルに保存するスタミナ

            return returnValue;
        }
        catch (Exception e)
        {
            return -2; //異常終了
        }
    }

    /// <summary>
    /// スタミナを回復
    /// </summary>
    public async Task<int> AddStamina(int addNum)
    {
        if (auth.CurrentUser == null)
        {
            return -2; //異常終了
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            int returnValue = -2; // 変更が無ければ異常終了として扱う

            await db.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                if (!snapshot.Exists) return false;

                int currentStamina = snapshot.GetValue<int>("stamina");
                int currentAdWatchCount = snapshot.GetValue<int>("adWatchCount");

                //既に3回以上広告を視聴していれば回復しない
                if (currentAdWatchCount >= 3)
                {
                    return false;
                }

                //広告視聴回数を加算しつつスタミナ回復
                transaction.Set(docRef, new { 
                    stamina = currentStamina + 2, 
                    adWatchCount = currentAdWatchCount + 1 
                }, SetOptions.MergeAll);

                returnValue = currentStamina + 2;

                return true;
            });

            staminaCache = (int)MathF.Max(0, returnValue); // ローカルに保存するスタミナ

            return returnValue;
        }
        catch (Exception e)
        {
            return -2; //異常終了
        }
    }



    /// <summary>
    /// 広告視聴回数を取得
    /// </summary>
    public async Task<int> GetAdWatchCount()
    {
        if (auth.CurrentUser == null)
        {
            return 4; //異常終了
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            //オンライン専用化
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Server);


            if (snapshot.ContainsField("adWatchCount"))
            {
                return snapshot.GetValue<int>("adWatchCount");
            }
            else
            {
                //広告視聴回数のデータが存在しなければ初期値を返す
                return 0;
            }
        }
        catch (Exception e)
        {
            return 4; //異常終了
        }
    }





    /// <summary>
    /// 新規データ作成
    /// </summary>
    public async Task<bool> SaveNewPlayerData(bool isAccoundAdd)
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
        { "runDistance", 0 },
        { "stamina", 3 },
        { "adWatchCount", 0 },
        { "lastUpdated", Timestamp.GetCurrentTimestamp() }
    };

        //新規作成の場合はプレイヤー総数を加算
        if (isAccoundAdd)
        {
            DocumentReference docRef_meta = db.Collection("metadata").Document("gameInfo");
            try
            {
                await docRef_meta.UpdateAsync(new Dictionary<string, object>
                {
                    { "totalUsers", FieldValue.Increment(1) }
                });
            }
            catch (Exception e)
            {
                
            }
        }   

        try
        {
            await docRef.SetAsync(newData);

            #region ローカルデータも初期化
            HighScoreManager.Save(0);
            RankingScoreManager.Save(new Queue<int>());
            UnsavedHighScoreFlagManager.Save(false);
            GM.highScore = 0;
            GM.rankingScoreQueue.Clear();
            nextOnlineStaminaCheckTime = DateTime.MinValue;
            staminaCache = 0;
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
        }
        catch (System.Exception e)
        {

        }

        //プレイヤー総数の減算
        DocumentReference docRef_meta = db.Collection("metadata").Document("gameInfo");
        try
        {
            await docRef_meta.UpdateAsync(new Dictionary<string, object>
            {
                { "totalUsers", FieldValue.Increment(-1) }
            });
        }
        catch (Exception e)
        {
            
        }
    }



    /// <summary>
    /// バージョンチェック
    /// </summary>
    public async Task<bool> CheckVersion()
    {
        int minimumVersion = 0;
        try
        {
            // 最小バージョンを取得
            DocumentSnapshot metaSnapshot = await db.Collection("metadata").Document("gameInfo").GetSnapshotAsync(Source.Server);
            minimumVersion = metaSnapshot.ContainsField("minimumVersion") ? metaSnapshot.GetValue<int>("minimumVersion") : 0;
        }
        catch (Exception e)
        {
            return true; // オフラインと判断して通過
        }

        // 現在のバージョンと比較
        if (GameManager.gameVersion >= minimumVersion)
        {
            return true; // バージョンが適合
        }
        else
        {
            return false; // バージョンが不適合
        }
    }
}
