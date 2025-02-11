using Firebase.Firestore;
using Firebase.Auth;
using Firebase;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using static UnityEngine.InputManagerEntry;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }

    private GameManager GM;
    private FirebaseFirestore db;
    private FirebaseAuth auth;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Firestoreのオフラインキャッシュを有効化 
        db.Settings.PersistenceEnabled = true;
        Debug.Log("Firestoreのオフラインキャッシュが有効になりました。");
    }



    void Start()
    {
        GM = GameManager.Instance;

        //キャッシュクリア
        //開発用
        //db.ClearPersistenceAsync();
    }



    /// <summary>
    /// プレイヤー名を保存
    /// [オンライン専用]
    /// </summary>
    public async Task SavePlayerName(string playerName)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            // Firebaseのネットワークを有効化（オフラインならエラーが出る）
            await db.EnableNetworkAsync();

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            await docRef.SetAsync(new { name = playerName }, SetOptions.MergeAll);
        }
        catch (FirebaseException e)
        {
            Debug.LogWarning("ネットワークが無効またはオフラインのため、プレイヤー名の保存をスキップしました。" + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤー名の保存に失敗: " + e.Message);
        }
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
        }
        catch (Exception e)
        {
            Debug.LogError("ハイスコアの保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// プレイヤースコアの保存
    /// [オンライン専用]
    /// Queue<int> rankingScore 内にデータがある限りクラウドにデータを反映させる
    /// </summary>
    public async Task SavePlayerScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            //未反映のデータがある限り繰り返し
            while (GM.rankingScoreQueue.Count > 0)
            {
                //データを1つ参照（取り出しはしない）
                int resultScore = GM.rankingScoreQueue.Peek();

                //排他制御
                await db.RunTransactionAsync(async transaction =>
                {
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                    int currentPlayerScore = snapshot.Exists ? snapshot.GetValue<int>("playerScore") : 0;
                    int newPlayerScore = currentPlayerScore + (int)((resultScore - currentPlayerScore) / 10f);
                    transaction.Set(docRef, new { playerScore = newPlayerScore }, SetOptions.MergeAll);
                });

                //データの反映が完了したらQueueから削除
                GM.rankingScoreQueue.Dequeue();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤースコアの更新に失敗: " + e.Message);
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
    /// [クラウドデータ優先，クラウドデータ・キャッシュデータ両取得（比較含む）]
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

            #region クラウドのsnapshotを取得
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

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


            #region キャッシュのsnapshotを取得
            DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);

            //キャッシュデータの存在をチェック
            if (!cacheSnapshot.Exists)
            {
                Debug.LogWarning("[Cache] プレイヤーデータが存在しません");
                return;
            }
            #endregion

            #region highScoreのロード

            int cachedHighScore = 0;
            int serverHighScore = 0;

            // まずキャッシュから取得
            if (cacheSnapshot.ContainsField("highScore"))
            {
                cachedHighScore = cacheSnapshot.GetValue<int>("highScore");
            }

            // 次にFirestoreの最新データを取得
            if (snapshot.Exists && snapshot.ContainsField("highScore"))
            {
                serverHighScore = snapshot.GetValue<int>("highScore");
            }

            // キャッシュとFirestoreの値を比較して高い方をローカルキャッシュに同期
            GM.highScore = Math.Max(cachedHighScore, serverHighScore);
            #endregion

        }
        catch (Exception e)
        {
            Debug.LogError("データのロードに失敗: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// プレイヤー名をロード
    /// [クラウドデータ優先]
    /// </summary>
    public async Task LoadPlayerName()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("name"))
            {
                //ローカルキャッシュに同期
                GM.playerName = snapshot.GetValue<string>("name");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤー名のロードに失敗: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// 経験値をロード
    /// [クラウドデータ優先]
    /// </summary>
    public async Task LoadExperience()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("experience"))
            {
                //ローカルキャッシュに同期
                GM.totalExp = snapshot.GetValue<int>("experience");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("経験値のロードに失敗: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// ハイスコアをロード
    /// [クラウドデータ・キャッシュデータ両取得]
    /// 両取得時は値の比較も実行（高い方を採用）
    /// </summary>
    public async Task LoadHighScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        int cachedHighScore = 0;
        int serverHighScore = 0;

        try
        {
            // まずキャッシュから取得
            DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
            if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("highScore"))
            {
                cachedHighScore = cacheSnapshot.GetValue<int>("highScore");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[キャッシュ] ハイスコアの取得に失敗: {e.Message}");
        }

        try
        {
            // 次にFirestoreの最新データを取得
            DocumentSnapshot serverSnapshot = await docRef.GetSnapshotAsync(Source.Server);
            if (serverSnapshot.Exists && serverSnapshot.ContainsField("highScore"))
            {
                serverHighScore = serverSnapshot.GetValue<int>("highScore");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Firestore] ハイスコアの取得に失敗: {e.Message}");
        }

        // キャッシュとFirestoreの値を比較して高い方をローカルキャッシュに同期
        GM.highScore = Math.Max(cachedHighScore, serverHighScore);
        return;
    }

    /// <summary>
    /// プレイヤースコアをロード
    /// [クラウドデータ優先]
    /// </summary>
    public async Task LoadPlayerScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("playerScore"))
            {
                //ローカルキャッシュに同期
                GM.playerScore = snapshot.GetValue<int>("playerScore");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤースコアのロードに失敗: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// トレーニングモードレベルクリア回数をロード
    /// [クラウドデータ優先]
    /// </summary>
    public async Task LoadTrainingClearCounts()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists && snapshot.ContainsField("trainingCount"))
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
        }
        catch (Exception e)
        {
            Debug.LogError("トレーニングモードのクリア回数のロードに失敗: " + e.Message);
        }
    }

    /// <summary>
    /// 使用中のスキンをロード
    /// [クラウドデータ優先]
    /// </summary>
    public async Task LoadUsingSkin()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("usingSkin"))
            {
                //ローカルキャッシュに同期
                GM.usingSkinID = snapshot.GetValue<int>("usingSkin");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("使用中のスキンのロードに失敗: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// 走行距離をロード
    /// [クラウドデータ優先]
    /// </summary>
    public async Task LoadRunDistance()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("runDistance"))
            {
                //ローカルキャッシュに同期
                GM.totalRunDistance = snapshot.GetValue<int>("runDistance");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("走行距離のロードに失敗: " + e.Message);
        }

        return;
    }




    /// <summary>
    /// 新規データ作成
    /// </summary>
    public async Task SaveNewPlayerData(string playerName)
    {
        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        Dictionary<string, object> newData = new Dictionary<string, object>
    {
        { "name", playerName },
        { "experience", GM.totalExp },
        { "highScore", GM.highScore },
        { "playerScore", GM.playerScore },
        { "trainingCount", GM.trainingClearCounts },
        { "usingSkin", GM.usingSkinID },
        { "runDistance", GM.totalRunDistance }
    };

        try
        {
            await docRef.SetAsync(newData, SetOptions.MergeAll);

            // 読み込みエラーを防ぐために配列に値を格納
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { $"trainingCount.{0}", 0 }
        });

            Debug.Log("[Firestore] 新しいプレイヤーデータを保存");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ERROR] プレイヤーデータの保存に失敗: {e.Message}");
        }
    }

}
