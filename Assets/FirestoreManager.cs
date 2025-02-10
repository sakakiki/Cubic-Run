using Firebase.Firestore;
using Firebase.Auth;
using Firebase;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

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
    /// </summary>
    public async Task SavePlayerName(string name)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            // 保存データ
            var updates = new Dictionary<string, object>
        {
            { "name", name },
            { "lastUpdated", FieldValue.ServerTimestamp } // サーバー時刻を保存
        };

            await docRef.SetAsync(updates, SetOptions.MergeAll);
            Debug.Log($"プレイヤー名を保存しました: {name}");
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤー名の保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// プレイヤーランクを保存（クラウド上のランクより高い場合のみ更新）
    /// </summary>
    public async Task SavePlayerRank(int newRank)
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
                int currentRank = snapshot.Exists ? snapshot.GetValue<int>("rank") : 0;

                if (newRank > currentRank)
                {
                    transaction.Set(docRef, new { rank = newRank }, SetOptions.MergeAll);
                    Debug.Log($"プレイヤーランクを更新: {newRank}");
                }
                else
                {
                    Debug.Log("新しいプレイヤーランクが現在のプレイヤーランクを超えていないため更新しませんでした。");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤーランクの保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// 獲得経験値を保存（累積加算）
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

            Debug.Log($"経験値を {additionalExp} 加算しました。");
        }
        catch (Exception e)
        {
            Debug.LogError("経験値の保存に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// ハイスコアを保存（現在のスコアより高い場合のみ更新）
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
                    Debug.Log($"ハイスコアを更新: {newScore}");
                }
                else
                {
                    Debug.Log("新しいスコアが現在のハイスコアを超えていないため更新しませんでした。");
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
    /// Queue<int> rankngScore 内にデータがある限りクラウドにデータを反映させる
    /// </summary>
    public async Task SavePlayerScore(float newRankingScore)
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
            while (GM.rankngScore.Count > 0)
            {
                //データを1つ参照（取り出しはしない）
                int resultScore = GM.rankngScore.Peek();

                //排他制御
                await db.RunTransactionAsync(async transaction =>
                {
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                    int currentPlayerScore = snapshot.Exists ? snapshot.GetValue<int>("playerScore") : 0;
                    int newPlayerScore = currentPlayerScore + (int)((resultScore - currentPlayerScore) / 10f);
                    transaction.Set(docRef, new { playerScore = newPlayerScore }, SetOptions.MergeAll);
                });

                //データの反映が完了したらQueueから削除
                GM.rankngScore.Dequeue();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤースコアの更新に失敗: " + e.Message);
        }
    }

    /// <summary>
    /// スキンのロック状態を保存
    /// MergeSkinUnlocks()内での呼び出しに限る
    /// </summary>
    private async Task SaveSkinUnlocks(List<bool> skinUnlocks)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        // boolリストを 0/1 のリストに変換
        List<int> skinData = skinUnlocks.Select(unlocked => unlocked ? 1 : 0).ToList();

        try
        {
            await docRef.SetAsync(new Dictionary<string, object>
        {
            { "skinUnlocks", skinData }
        }, SetOptions.MergeAll);

            Debug.Log($"[Firestore] スキンのアンロック状態を保存しました");
        }
        catch (Exception e)
        {
            Debug.LogError($"[エラー] スキンのアンロック状態保存に失敗: {e.Message}");
        }
    }



    /// <summary>
    /// 全てのデータをロード
    /// クラウドデータのロードを優先
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
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            #region playerNameのロード
            if (snapshot.Exists && snapshot.ContainsField("name"))
            {
                string name = snapshot.GetValue<string>("name");

                //ローカルキャッシュに同期
                GM.playerName = name;
            }
            #endregion

            #region playerRankのロード

            int cachedRank = 0;
            int serverRank = 0;

            try
            {
                // まずキャッシュから取得
                DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
                if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("rank"))
                {
                    cachedRank = Convert.ToInt32(cacheSnapshot.GetValue<double>("rank"));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[キャッシュ] ハイスコアの取得に失敗: {e.Message}");
            }

            // 次にFirestoreの最新データを取得
            if (snapshot.Exists && snapshot.ContainsField("rank"))
            {
                serverRank = Convert.ToInt32(snapshot.GetValue<double>("rank"));
            }

            // キャッシュとFirestoreの値を比較して高い方をローカルキャッシュに同期
            GM.highScore = Math.Max(cachedRank, serverRank);
            #endregion

            #region playerExpのロード
            if (snapshot.Exists && snapshot.ContainsField("experience"))
            {
                int experience = snapshot.GetValue<int>("experience");

                //ローカルキャッシュに同期
                GM.totalExp = experience;
            }
            #endregion

            #region highScoreのロード

            int cachedHighScore = 0;
            int serverHighScore = 0;

            try
            {
                // まずキャッシュから取得
                DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
                if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("highScore"))
                {
                    cachedHighScore = Convert.ToInt32(cacheSnapshot.GetValue<double>("highScore"));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[キャッシュ] ハイスコアの取得に失敗: {e.Message}");
            }

            // 次にFirestoreの最新データを取得
            if (snapshot.Exists && snapshot.ContainsField("highScore"))
            {
                serverHighScore = Convert.ToInt32(snapshot.GetValue<double>("highScore"));
            }

            // キャッシュとFirestoreの値を比較して高い方をローカルキャッシュに同期
            GM.highScore = Math.Max(cachedHighScore, serverHighScore);
            #endregion

            #region isSkinUnlockedのロード
            if (snapshot.Exists && snapshot.ContainsField("skinUnlocks"))
            {
                List<int> skinData = snapshot.GetValue<List<int>>("skinUnlocks");

                // ローカルキャッシュに同期
                GM.isSkinUnlocked = skinData.Select(value => value == 1).ToArray();
            }
            else
            {
                Debug.LogWarning("[Firestore] スキンのアンロック状態が見つかりませんでした。");
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
    /// プレイヤー名をロード
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
                string name = snapshot.GetValue<string>("name");
                Debug.Log($"ロードしたプレイヤー名: {name}");

                //ローカルキャッシュに同期
                GM.playerName = name;
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
    /// プレイヤーランクをロード
    /// </summary>
    public async Task LoadPlayerRank()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        int cachedRank = 0;
        int serverRank = 0;

        try
        {
            // まずキャッシュから取得
            DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
            if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("rank"))
            {
                cachedRank = Convert.ToInt32(cacheSnapshot.GetValue<double>("rank"));
                Debug.Log($"[キャッシュ] 取得したプレイヤーランク: {cachedRank}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[キャッシュ] プレイヤーランクの取得に失敗: {e.Message}");
        }

        try
        {
            // 次にFirestoreの最新データを取得
            DocumentSnapshot serverSnapshot = await docRef.GetSnapshotAsync(Source.Server);
            if (serverSnapshot.Exists && serverSnapshot.ContainsField("rank"))
            {
                serverRank = Convert.ToInt32(serverSnapshot.GetValue<double>("rank"));
                Debug.Log($"[Firestore] 取得したプレイヤーランク: {serverRank}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Firestore] プレイヤーランクの取得に失敗: {e.Message}");
        }

        // キャッシュとFirestoreの値を比較して高い方を採用
        int finalRank = Math.Max(cachedRank, serverRank);
        Debug.Log($"[最終プレイヤーランク] 選択したプレイヤーランク: {finalRank}");

        //ローカルキャッシュに同期
        GM.playerRank = finalRank;
        return;
    }

    /// <summary>
    /// 経験値をロード
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
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Cache);

            if (snapshot.Exists && snapshot.ContainsField("experience"))
            {
                int experience = snapshot.GetValue<int>("experience");
                Debug.Log($"ロードした経験値: {experience}");

                //ローカルキャッシュに同期
                GM.totalExp = experience;
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
                cachedHighScore = Convert.ToInt32(cacheSnapshot.GetValue<double>("highScore"));
                Debug.Log($"[キャッシュ] 取得したハイスコア: {cachedHighScore}");
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
                serverHighScore = Convert.ToInt32(serverSnapshot.GetValue<double>("highScore"));
                Debug.Log($"[Firestore] 取得したハイスコア: {serverHighScore}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Firestore] ハイスコアの取得に失敗: {e.Message}");
        }

        // キャッシュとFirestoreの値を比較して高い方を採用
        int finalHighScore = Math.Max(cachedHighScore, serverHighScore);
        Debug.Log($"[最終ハイスコア] 選択したハイスコア: {finalHighScore}");

        //ローカルキャッシュに同期
        GM.highScore = finalHighScore;
        return;
    }

    /// <summary>
    /// プレイヤースコアをロード
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
                int playerScore = snapshot.GetValue<int>("playerScore");
                Debug.Log($"ロードしたプレイヤー名: {playerScore}");

                //ローカルキャッシュに同期
                GM.playerScore = playerScore;
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
    /// スキンのロック状態をマージ（true:アンロック状態を優先）
    /// </summary>
    public async Task MergeSkinUnlocks()
    {
        List<bool> localSkinUnlocks = new List<bool>(GM.isSkinUnlocked);

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            List<bool> mergedSkinUnlocks = new List<bool>();

            if (snapshot.Exists && snapshot.ContainsField("skinUnlocks"))
            {
                List<int> cloudData = snapshot.GetValue<List<int>>("skinUnlocks");
                List<bool> cloudSkinUnlocks = cloudData.Select(value => value == 1).ToList();

                // ローカルとクラウドのデータをマージ（開放状態を優先）
                int count = Math.Max(cloudSkinUnlocks.Count, localSkinUnlocks.Count);
                for (int i = 0; i < count; i++)
                {
                    bool localValue = i < localSkinUnlocks.Count ? localSkinUnlocks[i] : false;
                    bool cloudValue = i < cloudSkinUnlocks.Count ? cloudSkinUnlocks[i] : false;

                    mergedSkinUnlocks.Add(localValue || cloudValue);
                }

                Debug.Log($"[同期] スキンデータを統合しました");
            }
            else
            {
                // クラウドデータがない場合はローカルデータをそのまま使う
                //mergedSkinUnlocks = new List<bool>(localSkinUnlocks);
                //Debug.Log($"[Firestore] クラウドデータがないため、ローカルデータを使用します");

                // クラウドデータがない場合は終了
                return;
            }

            // 統合したデータをFirestoreに保存
            await SaveSkinUnlocks(mergedSkinUnlocks);

            //ローカルキャッシュにも同期
            GM.isSkinUnlocked = mergedSkinUnlocks.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"[エラー] スキンのマージ処理に失敗: {e.Message}");
        }
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
        { "rank", GM.playerRank },
        { "experience", GM.totalExp },
        { "highScore", GM.highScore },
        { "playerScore", GM.playerScore },
        { "trainingMax", GM.highestTrainingLevel },
        { "trainingCount", GM.trainingClearCounts },
        { "usingSkin", GM.usingSkinID },
        { "skinUnlocks", GM.isSkinUnlocked.Select(unlocked => unlocked ? 1 : 0).ToList() }
    };

        try
        {
            await docRef.SetAsync(newData, SetOptions.MergeAll);
            Debug.Log("[Firestore] 新しいプレイヤーデータを保存");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ERROR] プレイヤーデータの保存に失敗: {e.Message}");
        }
    }

}
