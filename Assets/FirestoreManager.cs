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
    /// [�I�����C����p]
    /// </summary>
    public async Task SavePlayerName(string playerName)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            // Firebase�̃l�b�g���[�N��L�����i�I�t���C���Ȃ�G���[���o��j
            await db.EnableNetworkAsync();

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            await docRef.SetAsync(new { name = playerName }, SetOptions.MergeAll);
        }
        catch (FirebaseException e)
        {
            Debug.LogWarning("�l�b�g���[�N�������܂��̓I�t���C���̂��߁A�v���C���[���̕ۑ����X�L�b�v���܂����B" + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[���̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �l���o���l�̉��Z��ۑ�
    /// [�I�t���C���Ή�]
    /// �ݐω��Z
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
        }
        catch (Exception e)
        {
            Debug.LogError("�o���l�̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �n�C�X�R�A��ۑ�
    /// [�I�����C����p]
    /// ���݂̃X�R�A��荂���ꍇ�̂ݍX�V
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
    /// [�I�����C����p]
    /// Queue<int> rankingScore ���Ƀf�[�^���������N���E�h�Ƀf�[�^�𔽉f������
    /// </summary>
    public async Task SavePlayerScore()
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
            while (GM.rankingScoreQueue.Count > 0)
            {
                //�f�[�^��1�Q�Ɓi���o���͂��Ȃ��j
                int resultScore = GM.rankingScoreQueue.Peek();

                //�r������
                await db.RunTransactionAsync(async transaction =>
                {
                    DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                    int currentPlayerScore = snapshot.Exists ? snapshot.GetValue<int>("playerScore") : 0;
                    int newPlayerScore = currentPlayerScore + (int)((resultScore - currentPlayerScore) / 10f);
                    transaction.Set(docRef, new { playerScore = newPlayerScore }, SetOptions.MergeAll);
                });

                //�f�[�^�̔��f������������Queue����폜
                GM.rankingScoreQueue.Dequeue();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�v���C���[�X�R�A�̍X�V�Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �g���[�j���O���[�h���x���N���A�񐔂̉��Z��ۑ�
    /// [�I�t���C���Ή�]
    /// ��1���Z
    /// </summary>
    public async Task SaveTrainingClearCount(int level)
    {
        //���x�����C���f�b�N�X�ɕ␳
        level--;

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        string fieldName = $"trainingCount.{level}";

        try
        {
            // �w�背�x���̃J�E���g��1���₷
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { fieldName, FieldValue.Increment(1) }
        });
        }
        catch (Exception e)
        {
            Debug.LogError($"���x�� {level} �̃N���A�񐔂̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �g�p���̃X�L����ۑ�
    /// [�I�t���C���Ή�]
    /// �I�t���C�����s���̓T�[�o�[���ւ̍X�V���ҋ@����A�ォ��̍X�V���㏑�����邱�Ƃ�����
    /// </summary>
    public async Task SaveUsingSkin()
    {
        //GameManager�̒l���̗p
        int skinID = GM.usingSkinID;

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        try
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            // �ۑ��f�[�^
            await docRef.SetAsync(new { usingSkin = skinID }, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            Debug.LogError("�g�p���X�L���̕ۑ��Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// ���s�����̉��Z��ۑ�
    /// [�I�t���C���Ή�]
    /// �ݐω��Z
    /// </summary>
    public async Task SaveRunDistance(int additionalDistance)
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
            { "runDistance", FieldValue.Increment(additionalDistance) }
        });
        }
        catch (Exception e)
        {
            Debug.LogError("���s�����̕ۑ��Ɏ��s: " + e.Message);
        }
    }





    /// <summary>
    /// �S�Ẵf�[�^�����[�h
    /// [�N���E�h�f�[�^�D��C�N���E�h�f�[�^�E�L���b�V���f�[�^���擾�i��r�܂ށj]
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

            #region �N���E�h��snapshot���擾
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            //�f�[�^�̑��݂��`�F�b�N
            if (!snapshot.Exists)
            {
                Debug.LogWarning("[Firestore] �v���C���[�f�[�^�����݂��܂���");
                return;
            }
            #endregion

            #region playerName�̃��[�h
            if (snapshot.ContainsField("name"))
            {
                //���[�J���L���b�V���ɓ���
                GM.playerName = snapshot.GetValue<string>("name");
            }
            #endregion

            #region playerExp�̃��[�h
            if (snapshot.ContainsField("experience"))
            {
                //���[�J���L���b�V���ɓ���
                GM.totalExp = snapshot.GetValue<int>("experience");
            }
            #endregion

            #region trainingCount�̃��[�h
            if (snapshot.ContainsField("trainingCount"))
            {
                Dictionary<string, object> rawData = snapshot.GetValue<Dictionary<string, object>>("trainingCount");

                // Firestore�̎����f�[�^�� List<int> �ɕϊ�
                foreach (var kvp in rawData)
                {
                    int index = int.Parse(kvp.Key); // key�� "0", "1", "2", ... �̂悤�ȕ�����Ȃ̂ŕϊ�
                    int value = Convert.ToInt32(kvp.Value);

                    // �K�v�Ȃ烊�X�g���g��
                    while (GM.trainingClearCounts.Count <= index)
                    {
                        GM.trainingClearCounts.Add(0);
                    }

                    GM.trainingClearCounts[index] = value;
                }
            }
            #endregion

            #region usingSkin�̃��[�h
            if (snapshot.ContainsField("usingSkin"))
            {
                //���[�J���L���b�V���ɓ���
                GM.usingSkinID = snapshot.GetValue<int>("usingSkin");
            }
            #endregion

            #region runDistance�̃��[�h
            if (snapshot.ContainsField("runDistance"))
            {
                //���[�J���L���b�V���ɓ���
                GM.totalRunDistance = snapshot.GetValue<int>("runDistance");
            }
            #endregion


            #region �L���b�V����snapshot���擾
            DocumentSnapshot cacheSnapshot = await docRef.GetSnapshotAsync(Source.Cache);

            //�L���b�V���f�[�^�̑��݂��`�F�b�N
            if (!cacheSnapshot.Exists)
            {
                Debug.LogWarning("[Cache] �v���C���[�f�[�^�����݂��܂���");
                return;
            }
            #endregion

            #region highScore�̃��[�h

            int cachedHighScore = 0;
            int serverHighScore = 0;

            // �܂��L���b�V������擾
            if (cacheSnapshot.ContainsField("highScore"))
            {
                cachedHighScore = cacheSnapshot.GetValue<int>("highScore");
            }

            // ����Firestore�̍ŐV�f�[�^���擾
            if (snapshot.Exists && snapshot.ContainsField("highScore"))
            {
                serverHighScore = snapshot.GetValue<int>("highScore");
            }

            // �L���b�V����Firestore�̒l���r���č����������[�J���L���b�V���ɓ���
            GM.highScore = Math.Max(cachedHighScore, serverHighScore);
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
    /// [�N���E�h�f�[�^�D��]
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
                //���[�J���L���b�V���ɓ���
                GM.playerName = snapshot.GetValue<string>("name");
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
    /// �o���l�����[�h
    /// [�N���E�h�f�[�^�D��]
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
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("experience"))
            {
                //���[�J���L���b�V���ɓ���
                GM.totalExp = snapshot.GetValue<int>("experience");
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
    /// [�N���E�h�f�[�^�E�L���b�V���f�[�^���擾]
    /// ���擾���͒l�̔�r�����s�i���������̗p�j
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
                cachedHighScore = cacheSnapshot.GetValue<int>("highScore");
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
                serverHighScore = serverSnapshot.GetValue<int>("highScore");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Firestore] �n�C�X�R�A�̎擾�Ɏ��s: {e.Message}");
        }

        // �L���b�V����Firestore�̒l���r���č����������[�J���L���b�V���ɓ���
        GM.highScore = Math.Max(cachedHighScore, serverHighScore);
        return;
    }

    /// <summary>
    /// �v���C���[�X�R�A�����[�h
    /// [�N���E�h�f�[�^�D��]
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
                //���[�J���L���b�V���ɓ���
                GM.playerScore = snapshot.GetValue<int>("playerScore");
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
    /// �g���[�j���O���[�h���x���N���A�񐔂����[�h
    /// [�N���E�h�f�[�^�D��]
    /// </summary>
    public async Task LoadTrainingClearCounts()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists && snapshot.ContainsField("trainingCount"))
            {
                Dictionary<string, object> rawData = snapshot.GetValue<Dictionary<string, object>>("trainingCount");

                // Firestore�̎����f�[�^�� List<int> �ɕϊ�
                foreach (var kvp in rawData)
                {
                    int index = int.Parse(kvp.Key); // key�� "0", "1", "2", ... �̂悤�ȕ�����Ȃ̂ŕϊ�
                    int value = Convert.ToInt32(kvp.Value);

                    // �K�v�Ȃ烊�X�g���g��
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
            Debug.LogError("�g���[�j���O���[�h�̃N���A�񐔂̃��[�h�Ɏ��s: " + e.Message);
        }
    }

    /// <summary>
    /// �g�p���̃X�L�������[�h
    /// [�N���E�h�f�[�^�D��]
    /// </summary>
    public async Task LoadUsingSkin()
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

            if (snapshot.Exists && snapshot.ContainsField("usingSkin"))
            {
                //���[�J���L���b�V���ɓ���
                GM.usingSkinID = snapshot.GetValue<int>("usingSkin");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�g�p���̃X�L���̃��[�h�Ɏ��s: " + e.Message);
        }

        return;
    }

    /// <summary>
    /// ���s���������[�h
    /// [�N���E�h�f�[�^�D��]
    /// </summary>
    public async Task LoadRunDistance()
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

            if (snapshot.Exists && snapshot.ContainsField("runDistance"))
            {
                //���[�J���L���b�V���ɓ���
                GM.totalRunDistance = snapshot.GetValue<int>("runDistance");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("���s�����̃��[�h�Ɏ��s: " + e.Message);
        }

        return;
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

            // �ǂݍ��݃G���[��h�����߂ɔz��ɒl���i�[
            await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { $"trainingCount.{0}", 0 }
        });

            Debug.Log("[Firestore] �V�����v���C���[�f�[�^��ۑ�");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ERROR] �v���C���[�f�[�^�̕ۑ��Ɏ��s: {e.Message}");
        }
    }

}
