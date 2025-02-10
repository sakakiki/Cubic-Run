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
    }




    void Start()
    {
        GM = GameManager.Instance;

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        //キャッシュクリア
        //開発用
        //db.ClearPersistenceAsync();

        // Firestoreのオフラインキャッシュを有効化 
        db.Settings.PersistenceEnabled = true;

        Debug.Log("Firestoreのオフラインキャッシュが有効になりました。");
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
            await docRef.SetAsync(new { name }, SetOptions.MergeAll);
            Debug.Log($"プレイヤー名を保存しました: {name}");
        }
        catch (Exception e)
        {
            Debug.LogError("プレイヤー名の保存に失敗: " + e.Message);
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
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Cache);

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
}
