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

        // Firestore�̃I�t���C���L���b�V����L���� 
        db.Settings.PersistenceEnabled = true;
        Debug.Log("Firestore�̃I�t���C���L���b�V�����L���ɂȂ�܂����B");
    }



    void Start()
    {
        GM = GameManager.Instance;

        //�L���b�V���N���A
        //�J���p
        //db.ClearPersistenceAsync();
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

            // �ۑ��f�[�^
            var updates = new Dictionary<string, object>
        {
            { "name", name },
            { "lastUpdated", FieldValue.ServerTimestamp } // �T�[�o�[������ۑ�
        };

            await docRef.SetAsync(updates, SetOptions.MergeAll);
            Debug.Log($"�v���C���[����ۑ����܂���: {name}");
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[���̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �v���C���[�����N��ۑ��i�N���E�h��̃����N��荂���ꍇ�̂ݍX�V�j
    /// </summary>
    public async Task SavePlayerRank(int newRank)
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
                int currentRank = snapshot.Exists ? snapshot.GetValue<int>("rank") : 0;

                if (newRank > currentRank)
                {
                    transaction.Set(docRef, new { rank = newRank }, SetOptions.MergeAll);
                    Debug.Log($"�v���C���[�����N���X�V: {newRank}");
                }
                else
                {
                    Debug.Log("�V�����v���C���[�����N�����݂̃v���C���[�����N�𒴂��Ă��Ȃ����ߍX�V���܂���ł����B");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[�����N�̕ۑ��Ɏ��s: " + e.Message);
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
    /// �v���C���[�X�R�A�̕ۑ�
    /// Queue<int> rankngScore ���Ƀf�[�^���������N���E�h�Ƀf�[�^�𔽉f������
    /// </summary>
    public async Task SavePlayerScore(float newRankingScore)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            //�����f�̃f�[�^���������J��Ԃ�
            while (GM.rankngScore.Count > 0)
            {
                //�f�[�^��1�Q�Ɓi���o���͂��Ȃ��j
                int resultScore = GM.rankngScore.Peek();

                //�r������
                await db.RunTransactionAsync(async transaction =>
                {
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                    int currentPlayerScore = snapshot.Exists ? snapshot.GetValue<int>("playerScore") : 0;
                    int newPlayerScore = currentPlayerScore + (int)((resultScore - currentPlayerScore) / 10f);
                    transaction.Set(docRef, new { playerScore = newPlayerScore }, SetOptions.MergeAll);
                });

                //�f�[�^�̔��f������������Queue����폜
                GM.rankngScore.Dequeue();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[�X�R�A�̍X�V�Ɏ��s: " + e.Message);
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
    /// �S�Ẵf�[�^�����[�h
    /// �N���E�h�f�[�^�̃��[�h��D��
    /// </summary>
    public async Task LoadAll()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            #region playerName�̃��[�h
            if (snapshot.Exists && snapshot.ContainsField("name"))
            {
                string name = snapshot.GetValue<string>("name");

                //���[�J���L���b�V���ɓ���
                GM.playerName = name;
            }
            #endregion

            #region playerRank�̃��[�h

            int cachedRank = 0;
            int serverRank = 0;

            try
            {
                // �܂��L���b�V������擾
                DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
                if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("rank"))
                {
                    cachedRank = Convert.ToInt32(cacheSnapshot.GetValue<double>("rank"));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[�L���b�V��] �n�C�X�R�A�̎擾�Ɏ��s: {e.Message}");
            }

            // ����Firestore�̍ŐV�f�[�^���擾
            if (snapshot.Exists && snapshot.ContainsField("rank"))
            {
                serverRank = Convert.ToInt32(snapshot.GetValue<double>("rank"));
            }

            // �L���b�V����Firestore�̒l���r���č����������[�J���L���b�V���ɓ���
            GM.highScore = Math.Max(cachedRank, serverRank);
            #endregion

            #region playerExp�̃��[�h
            if (snapshot.Exists && snapshot.ContainsField("experience"))
            {
                int experience = snapshot.GetValue<int>("experience");

                //���[�J���L���b�V���ɓ���
                GM.totalExp = experience;
            }
            #endregion

            #region highScore�̃��[�h

            int cachedHighScore = 0;
            int serverHighScore = 0;

            try
            {
                // �܂��L���b�V������擾
                DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
                if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("highScore"))
                {
                    cachedHighScore = Convert.ToInt32(cacheSnapshot.GetValue<double>("highScore"));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[�L���b�V��] �n�C�X�R�A�̎擾�Ɏ��s: {e.Message}");
            }

            // ����Firestore�̍ŐV�f�[�^���擾
            if (snapshot.Exists && snapshot.ContainsField("highScore"))
            {
                serverHighScore = Convert.ToInt32(snapshot.GetValue<double>("highScore"));
            }

            // �L���b�V����Firestore�̒l���r���č����������[�J���L���b�V���ɓ���
            GM.highScore = Math.Max(cachedHighScore, serverHighScore);
            #endregion

            #region isSkinUnlocked�̃��[�h
            if (snapshot.Exists && snapshot.ContainsField("skinUnlocks"))
            {
                List<int> skinData = snapshot.GetValue<List<int>>("skinUnlocks");

                // ���[�J���L���b�V���ɓ���
                GM.isSkinUnlocked = skinData.Select(value => value == 1).ToArray();
            }
            else
            {
                Debug.LogWarning("[Firestore] �X�L���̃A�����b�N��Ԃ�������܂���ł����B");
            }
            #endregion

        }
        catch (Exception e)
        {
            Debug.LogError("�f�[�^�̃��[�h�Ɏ��s: " + e.Message);
        }

        return;
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
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

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
    /// �v���C���[�����N�����[�h
    /// </summary>
    public async Task LoadPlayerRank()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        int cachedRank = 0;
        int serverRank = 0;

        try
        {
            // �܂��L���b�V������擾
            DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);
            if (cacheSnapshot.Exists && cacheSnapshot.ContainsField("rank"))
            {
                cachedRank = Convert.ToInt32(cacheSnapshot.GetValue<double>("rank"));
                Debug.Log($"[�L���b�V��] �擾�����v���C���[�����N: {cachedRank}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[�L���b�V��] �v���C���[�����N�̎擾�Ɏ��s: {e.Message}");
        }

        try
        {
            // ����Firestore�̍ŐV�f�[�^���擾
            DocumentSnapshot serverSnapshot = await docRef.GetSnapshotAsync(Source.Server);
            if (serverSnapshot.Exists && serverSnapshot.ContainsField("rank"))
            {
                serverRank = Convert.ToInt32(serverSnapshot.GetValue<double>("rank"));
                Debug.Log($"[Firestore] �擾�����v���C���[�����N: {serverRank}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Firestore] �v���C���[�����N�̎擾�Ɏ��s: {e.Message}");
        }

        // �L���b�V����Firestore�̒l���r���č��������̗p
        int finalRank = Math.Max(cachedRank, serverRank);
        Debug.Log($"[�ŏI�v���C���[�����N] �I�������v���C���[�����N: {finalRank}");

        //���[�J���L���b�V���ɓ���
        GM.playerRank = finalRank;
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
    /// �v���C���[�X�R�A�����[�h
    /// </summary>
    public async Task LoadPlayerScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("playerScore"))
            {
                int playerScore = snapshot.GetValue<int>("playerScore");
                Debug.Log($"���[�h�����v���C���[��: {playerScore}");

                //���[�J���L���b�V���ɓ���
                GM.playerScore = playerScore;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[�X�R�A�̃��[�h�Ɏ��s: " + e.Message);
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



    /// <summary>
    /// �V�K�f�[�^�쐬
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
            Debug.Log("[Firestore] �V�����v���C���[�f�[�^��ۑ�");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ERROR] �v���C���[�f�[�^�̕ۑ��Ɏ��s: {e.Message}");
        }
    }

}
