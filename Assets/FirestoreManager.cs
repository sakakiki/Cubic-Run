using Firebase.Firestore;
using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;

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
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }

    public async Task SavePlayerData(string name, int highScore, int experience)
    {
        if (auth.CurrentUser == null) return;

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        await docRef.SetAsync(new
        {
            name = name,
            highScore = highScore,
            experience = experience
        });

        Debug.Log("データ保存完了");
    }

    public async Task<PlayerData> LoadPlayerData()
    {
        if (auth.CurrentUser == null) return null;

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            PlayerData data = snapshot.ConvertTo<PlayerData>();
            Debug.Log($"データ読み込み成功: {data.name}, HS: {data.highScore}, EXP: {data.experience}");
            return data;
        }
        else
        {
            Debug.Log("データなし");
            return null;
        }
    }
}

public class PlayerData
{
    public string name;
    public int highScore;
    public int experience;
}
