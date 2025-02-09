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

        // Firestore�̃I�t���C���L���b�V����L����
        //FirebaseFirestoreSettings settings = FirebaseFirestoreSettings { PersistenceEnabled = true };
        //db = FirebaseFirestore.GetInstance(FirebaseApp.DefaultInstance, settings.ToString());

        //Debug.Log("Firestore�̃I�t���C���L���b�V�����L���ɂȂ�܂����B");
    }


    public async Task SavePlayerData(string name, int highScore, int experience)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
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

            // ���[�J���������݂������A�I�t���C�������f�[�^��ۑ�
            await docRef.SetAsync(playerData, SetOptions.MergeAll);

            Debug.Log("�f�[�^�ۑ������i�I�t���C���Ή��j");
        }
        catch (System.Exception e)
        {
            Debug.LogError("�f�[�^�ۑ��Ɏ��s: " + e.Message);
        }
    }


    public async Task LoadPlayerData()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            // �܂��̓��[�J���L���b�V������擾���A�Ȃ���΃I�����C���Ŏ擾
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Cache);

            if (!snapshot.Exists)
            {
                Debug.LogWarning("���[�J���L���b�V���Ƀf�[�^������܂���B�I�����C������擾�����݂܂��B");
                snapshot = await docRef.GetSnapshotAsync(Source.Server);
            }

            if (snapshot.Exists)
            {
                string name = snapshot.GetValue<string>("name");
                int highScore = snapshot.GetValue<int>("highScore");
                int experience = snapshot.GetValue<int>("experience");

                Debug.Log($"�v���C���[�f�[�^: {name}, �n�C�X�R�A: {highScore}, �o���l: {experience}");
            }
            else
            {
                Debug.LogWarning("�v���C���[�f�[�^��������܂���B");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("�f�[�^�擾�Ɏ��s: " + e.Message);
        }
    }
}

public class PlayerData
{
    public string name;
    public int highScore;
    public int experience;
}
