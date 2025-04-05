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

    private const int playerNameMaxLength = 12; // �S�p2�����A���p1�����Ƃ��ăJ�E���g����ő啶����




    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Firestore�̃I�t���C���L���b�V����L���� 
        db.Settings.PersistenceEnabled = true;

        //GameManager�̊i�[
        GM = GameManager.Instance;
    }



    /// <summary>
    /// �v���C���[����ۑ�
    /// [�I�����C����p]
    /// </summary>
    public async Task<string> SavePlayerName(string playerName)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");

            return "�ُ�I��";
        }

        //�V�����v���C���[������������΋���
        if (CalculateTextLength(playerName) > playerNameMaxLength)
            return "�v���C���[�����������܂��B�Z���v���C���[���ōēx���������������B";

        try
        {
            // �l�b�g���[�N�ڑ����`�F�b�N�i�I�t���C���Ȃ瑦�G���[��Ԃ��j
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return "�l�b�g���[�N�������܂��̓I�t���C���̂��߁A�v���C���[����ۑ��ł��܂���ł����B";
            }

            // Firebase�̃l�b�g���[�N�𖾎��I�ɗL�����i�I�t���C���Ȃ�G���[���o��j
            await db.DisableNetworkAsync();
            await db.EnableNetworkAsync();

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

            await docRef.SetAsync(new { name = playerName }, SetOptions.MergeAll);

            //���[�J���ɂ����f
            GM.playerName = playerName;

            return "����I��";
        }
        catch (FirebaseException e)
        {
            return "�l�b�g���[�N�������܂��̓I�t���C���̂��߁A�v���C���[����ۑ��ł��܂���ł����B";
        }
        catch (Exception e)
        {
            return "�ُ�I��";
        }
    }

    /// <summary>
    /// �S�p��2�A���p��1�Ƃ��ĕ��������v�Z����
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
    /// �S�p�������ǂ����𔻒�
    /// </summary>
    bool IsFullWidth(char c)
    {
        return Regex.IsMatch(c.ToString(), @"[^\x00-\x7F]"); // ASCII�O�̕�����S�p�Ɣ���
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

            //�N���E�h�ɖ����f�̃t���O�����낷
            GM.isUnsavedHighScore = false;
            UnsavedHighScoreFlagManager.Save(false);
        }
        catch (Exception e)
        {
            Debug.LogError("�n�C�X�R�A�̕ۑ��Ɏ��s: " + e.Message);

            //�N���E�h�ɖ����f�̃t���O�𗧂Ă�
            GM.isUnsavedHighScore = true;
            UnsavedHighScoreFlagManager.Save(true);
        }
    }

    /// <summary>
    /// �v���C���[�X�R�A�̕ۑ�
    /// [�I�����C����p]
    /// Queue<int> rankingScoreQueue ���̃f�[�^��S�ăN���E�h�Ƀf�[�^�𔽉f������
    /// </summary>
    public async Task SavePlayerScore()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Firebase�F�؂���Ă��܂���");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);

        //�X�V��̃v���C���[�X�R�A
        int newPlayerScore = 0;

        try
        {
            // Queue�̃R�s�[�i�g�����U�N�V�����O�ň��S�Ƀf�[�^���擾�j
            List<int> tempScores = GM.rankingScoreQueue.ToList();

            bool transactionSuccess = await db.RunTransactionAsync(async transaction =>
            {
                // �v���C���[�X�R�A�̎擾
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
                newPlayerScore = snapshot.Exists ? snapshot.GetValue<int>("playerScore") : 0;

                // �����f�̃f�[�^�𔽉f
                foreach (int resultScore in tempScores)
                {
                    newPlayerScore += (resultScore - newPlayerScore) / 10;
                }

                transaction.Set(docRef, new { playerScore = newPlayerScore }, SetOptions.MergeAll);
                return true;
            });

            // �g�����U�N�V���������������ꍇ�̂�
            if (transactionSuccess)
            {
                // GM �� Queue ����ɂ���
                GM.rankingScoreQueue.Clear();

                //���[�J���f�[�^�ɔ��f
                GM.playerScore = newPlayerScore;

                // ���[�J���� Queue �̓��e���X�V
                RankingScoreManager.Save(GM.rankingScoreQueue);
                Debug.Log("Queue�̏������[�J���X�g���[�W�ɕۑ����܂���");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"�v���C���[�X�R�A�̍X�V�Ɏ��s: {e.Message}");

            // ���[�J���� Queue �̓��e���X�V�i���s���ł��ۑ�����j
            RankingScoreManager.Save(GM.rankingScoreQueue);
            Debug.Log("Queue�̏������[�J���X�g���[�W�ɕۑ����܂���");
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
    /// [�N���E�h�f�[�^�D��]
    /// ���[�J���f�[�^�Ƃ̔�r���܂�
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

            #region snapshot���擾�i�N���E�h��D�悵�A���s����΃L���b�V�����擾�j

            DocumentSnapshot snapshot;

            try
            {
                // �N���E�h�̍ŐV�f�[�^���擾�i�ʐM�������j
                snapshot = await docRef.GetSnapshotAsync(Source.Server);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"�N���E�h�f�[�^�̎擾�Ɏ��s: {e.Message}�B�L���b�V���f�[�^���g�p���܂��B");

                // �N���E�h�擾�Ɏ��s�����ꍇ�A�L���b�V������擾�i�I�t���C���Ή��j
                snapshot = await docRef.GetSnapshotAsync(Source.Cache);
            }

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

            #region highScore�̃��[�h

            int serverHighScore = 0;

            if (snapshot.Exists && snapshot.ContainsField("highScore"))
            {
                serverHighScore = snapshot.GetValue<int>("highScore");
            }

            // �N���E�h�f�[�^�ƃ��[�J���f�[�^�̒l���r���č����������[�J���ɓ���
            GM.highScore = Math.Max(GM.highScore, serverHighScore);
            #endregion

            #region playerExp�̃��[�h
            if (snapshot.ContainsField("playerScore"))
            {
                //���[�J���L���b�V���ɓ���
                GM.playerScore = snapshot.GetValue<int>("playerScore");
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

        }
        catch (Exception e)
        {
            Debug.LogError("�f�[�^�̃��[�h�Ɏ��s: " + e.Message);
        }

        return;
    }



    /// <summary>
    /// Firestore����w�肵���X�R�A�����L���O���擾
    /// </summary>
    /// <param name="scoreType">"highScore" �܂��� "playerScore"</param>
    /// <returns>�����L���O���10���̃��X�g</returns>
    public async Task<List<(string name, int score, int experience, int skin)>> GetTop10Ranking(string scoreType)
    {
        //�����L���O�̃��X�g
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
            Debug.LogError($"[Firestore] �����L���O�擾�Ɏ��s: {e.Message}");
        }

        //�擾���s����List�������l�Ŗ��߂�
        while(ranking.Count < RankingManager.RankingCount)
            ranking.Add(("Unknown", 0, 0, 0));

        return ranking;
    }
    
    
    
    /// <summary>
    /// Firestore���烆�[�U�[�̏��ʂƏ�ʉ�%�����擾
    /// </summary>
    /// <param name="scoreType">"highScore" �܂��� "playerScore"</param>
    /// <param name="userScore">���[�U�[�̃X�R�A</param>
    /// <returns>(����, ���%)</returns>
    public async Task<(int rank, float percentile)> GetUserRanking(string scoreType, int userScore)
    {
        int rank = -1;  //���ʂ�-1�Ȃ�G���[
        float percentile = 0;

        try
        {
            // ���[�U�[���X�R�A�����������擾
            Query countQuery = db.Collection("users").WhereGreaterThan(scoreType, userScore);
            QuerySnapshot countSnapshot = await countQuery.GetSnapshotAsync(Source.Server);
            int higherCount = countSnapshot.Count;

            // �����[�U�[�����擾
            Query totalQuery = db.Collection("users");
            QuerySnapshot totalSnapshot = await totalQuery.GetSnapshotAsync(Source.Server);
            int totalUsers = totalSnapshot.Count;

            // ���[�U�[�̏��ʂƃp�[�Z���^�C���v�Z
            rank = higherCount + 1;
            percentile = (float)rank / (float)totalUsers * 100;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Firestore] ���[�U�[���ʎ擾�Ɏ��s: {e.Message}");
        }

        return (rank, percentile);
    }




    /// <summary>
    /// �V�K�f�[�^�쐬
    /// </summary>
    public async Task<bool> SaveNewPlayerData()
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
        { "runDistance", 0 }
    };

        try
        {
            await docRef.SetAsync(newData);

            #region ���[�J���f�[�^��������
            HighScoreManager.Save(0);
            RankingScoreManager.Save(new Queue<int>());
            UnsavedHighScoreFlagManager.Save(false);
            GM.highScore = 0;
            GM.rankingScoreQueue.Clear();
            #endregion

            //����I��
            return true;
        }
        catch (Exception e)
        {
            //�ُ�I��
            return false;
        }
    }



    /// <summary>
    /// �f�[�^�폜
    /// </summary>
    public async Task DeleteDocument(string documentId)
    {
        try
        {
            DocumentReference docRef = db.Collection("users").Document(documentId);
            await docRef.DeleteAsync();
            Debug.Log($"�h�L�������g {documentId} ���폜���܂���");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�h�L�������g�폜�G���[: {e.Message}");
            Debug.Log(documentId);
        }
    }
}
