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

        //�L���b�V���N���A
        //�J���p
        //db.ClearPersistenceAsync();

        // Firestore�̃I�t���C���L���b�V����L���� 
        db.Settings.PersistenceEnabled = true;

        Debug.Log("Firestore�̃I�t���C���L���b�V�����L���ɂȂ�܂����B");
    }

    /// <summary>
    /// �v���C���[����ۑ�
    /// </summary>
    public async Task SavePlayerName(string name)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            await docRef.SetAsync(new { name }, SetOptions.MergeAll);
            Debug.Log($"�v���C���[����ۑ����܂���: {name}");
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[���̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �n�C�X�R�A��ۑ��i���݂̃X�R�A��荂���ꍇ�̂ݍX�V�j
    /// </summary>
    public async Task SaveHighScore(int newScore)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
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
                    Debug.Log($"�n�C�X�R�A���X�V: {newScore}");
                }
                else
                {
                    Debug.Log("�V�����X�R�A�����݂̃n�C�X�R�A�𒴂��Ă��Ȃ����ߍX�V���܂���ł����B");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError("�n�C�X�R�A�̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �l���o���l��ۑ��i�ݐω��Z�j
    /// </summary>
    public async Task SaveExperience(int additionalExp)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "experience", FieldValue.Increment(additionalExp) }
        });

            Debug.Log($"�o���l�� {additionalExp} ���Z���܂����B");
        }
        catch (Exception e)
        {
            Debug.LogError("�o���l�̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �X�L���̃��b�N��Ԃ�ۑ�
    /// MergeSkinUnlocks()���ł̌Ăяo���Ɍ���
    /// </summary>
    private async Task SaveSkinUnlocks(List<bool> skinUnlocks)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        // bool���X�g�� 0/1 �̃��X�g�ɕϊ�
        List<int> skinData = skinUnlocks.Select(unlocked => unlocked ? 1 : 0).ToList();

        try
        {
            await docRef.SetAsync(new Dictionary<string, object>
        {
            { "skinUnlocks", skinData }
        }, SetOptions.MergeAll);

            Debug.Log($"[Firestore] �X�L���̃A�����b�N��Ԃ�ۑ����܂���");
        }
        catch (Exception e)
        {
            Debug.LogError($"[�G���[] �X�L���̃A�����b�N��ԕۑ��Ɏ��s: {e.Message}");
        }
    }


    /// <summary>
    /// �v���C���[�������[�h
    /// </summary>
    public async Task LoadPlayerName()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Cache);

            if (snapshot.Exists && snapshot.ContainsField("name"))
            {
                string name = snapshot.GetValue<string>("name");
                Debug.Log($"���[�h�����v���C���[��: {name}");

                //���[�J���L���b�V���ɓ���
                GM.playerName = name;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[���̃��[�h�Ɏ��s: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// �n�C�X�R�A�����[�h
    /// </summary>
    public async Task LoadHighScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        int cachedHighScore = 0;
        int serverHighScore = 0;

        try
        {
            // �܂��L���b�V������擾
            DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
            if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("highScore"))
            {
                cachedHighScore = Convert.ToInt32(cacheSnapshot.GetValue<double>("highScore"));
                Debug.Log($"[�L���b�V��] �擾�����n�C�X�R�A: {cachedHighScore}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[�L���b�V��] �n�C�X�R�A�̎擾�Ɏ��s: {e.Message}");
        }

        try
        {
            // ����Firestore�̍ŐV�f�[�^���擾
            DocumentSnapshot serverSnapshot = await docRef.GetSnapshotAsync(Source.Server);
            if (serverSnapshot.Exists && serverSnapshot.ContainsField("highScore"))
            {
                serverHighScore = Convert.ToInt32(serverSnapshot.GetValue<double>("highScore"));
                Debug.Log($"[Firestore] �擾�����n�C�X�R�A: {serverHighScore}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Firestore] �n�C�X�R�A�̎擾�Ɏ��s: {e.Message}");
        }

        // �L���b�V����Firestore�̒l���r���č��������̗p
        int finalHighScore = Math.Max(cachedHighScore, serverHighScore);
        Debug.Log($"[�ŏI�n�C�X�R�A] �I�������n�C�X�R�A: {finalHighScore}");

        //���[�J���L���b�V���ɓ���
        GM.highScore = finalHighScore;
        return;
    }

    /// <summary>
    /// �o���l�����[�h
    /// </summary>
    public async Task LoadExperience()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Cache);

            if (snapshot.Exists && snapshot.ContainsField("experience"))
            {
                int experience = snapshot.GetValue<int>("experience");
                Debug.Log($"���[�h�����o���l: {experience}");

                //���[�J���L���b�V���ɓ���
                GM.totalExp = experience;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�o���l�̃��[�h�Ɏ��s: " + e.Message);
        }

        return;
    }



    /// <summary>
    /// �X�L���̃��b�N��Ԃ��}�[�W�itrue:�A�����b�N��Ԃ�D��j
    /// </summary>
    public async Task MergeSkinUnlocks()
    {
        List<bool> localSkinUnlocks = new List<bool>(GM.isSkinUnlocked);

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
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

                // ���[�J���ƃN���E�h�̃f�[�^���}�[�W�i�J����Ԃ�D��j
                int count = Math.Max(cloudSkinUnlocks.Count, localSkinUnlocks.Count);
                for (int i = 0; i < count; i++)
                {
                    bool localValue = i < localSkinUnlocks.Count ? localSkinUnlocks[i] : false;
                    bool cloudValue = i < cloudSkinUnlocks.Count ? cloudSkinUnlocks[i] : false;

                    mergedSkinUnlocks.Add(localValue || cloudValue);
                }

                Debug.Log($"[����] �X�L���f�[�^�𓝍����܂���");
            }
            else
            {
                // �N���E�h�f�[�^���Ȃ��ꍇ�̓��[�J���f�[�^�����̂܂܎g��
                //mergedSkinUnlocks = new List<bool>(localSkinUnlocks);
                //Debug.Log($"[Firestore] �N���E�h�f�[�^���Ȃ����߁A���[�J���f�[�^���g�p���܂�");

                // �N���E�h�f�[�^���Ȃ��ꍇ�͏I��
                return;
            }

            // ���������f�[�^��Firestore�ɕۑ�
            await SaveSkinUnlocks(mergedSkinUnlocks);

            //���[�J���L���b�V���ɂ�����
            GM.isSkinUnlocked = mergedSkinUnlocks.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"[�G���[] �X�L���̃}�[�W�����Ɏ��s: {e.Message}");
        }
    }
}
