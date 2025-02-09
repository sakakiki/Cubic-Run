using Firebase.Firestore;
using Firebase.Auth;
using Firebase;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }

    private FirebaseFirestore db;
    [HideInInspector] public FirebaseAuth auth;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }




    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        db = FirebaseFirestore.DefaultInstance;

        // Firestoreのオフラインキャッシュを有効化
        //FirebaseFirestoreSettings settings = FirebaseFirestoreSettings { PersistenceEnabled = true };
        //db = FirebaseFirestore.GetInstance(FirebaseApp.DefaultInstance, settings.ToString());

        //Debug.Log("Firestoreのオフラインキャッシュが有効になりました。");
    }


    public async Task SavePlayerData(string name, int highScore, int experience)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            Dictionary<string, object> playerData = new Dictionary<string, object>
        {
            { "name", name },
            { "highScore", highScore },
            { "experience", experience }
        };

            // ローカル書き込みを許可し、オフライン時もデータを保存
            await docRef.SetAsync(playerData, SetOptions.MergeAll);

            Debug.Log("データ保存完了（オフライン対応）");
        }
        catch (System.Exception e)
        {
            Debug.LogError("データ保存に失敗: " + e.Message);
        }
    }


    public async Task LoadPlayerData()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase認証されていません");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            // まずはローカルキャッシュから取得し、なければオンラインで取得
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Cache);

            if (!snapshot.Exists)
            {
                Debug.LogWarning("ローカルキャッシュにデータがありません。オンラインから取得を試みます。");
                snapshot = await docRef.GetSnapshotAsync(Source.Server);
            }

            if (snapshot.Exists)
            {
                string name = snapshot.GetValue<string>("name");
                int highScore = snapshot.GetValue<int>("highScore");
                int experience = snapshot.GetValue<int>("experience");

                Debug.Log($"プレイヤーデータ: {name}, ハイスコア: {highScore}, 経験値: {experience}");
            }
            else
            {
                Debug.LogWarning("プレイヤーデータが見つかりません。");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("データ取得に失敗: " + e.Message);
        }
    }
}

public class PlayerData
{
    public string name;
    public int highScore;
    public int experience;
}
