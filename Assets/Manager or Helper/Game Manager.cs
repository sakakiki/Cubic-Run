using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

//GameManager�͏�����D����s
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static GameManager Instance;

    //Manager�n
    private TerrainManager TM;
    private FirestoreManager FSM;
    private SkinDataBase SDB;

    [Header("���̃L���b�V���iFirebase�ۑ��f�[�^�j")]
    public string playerName = "Noname";
    public int totalExp = 0;
    public int playerScore = 0;
    public List<int> trainingClearCounts = new List<int>();
    public int usingSkinID = 0;
    public int totalRunDistance = 0;
    [Space(30)]

    [Header("���̃L���b�V���iFirebase��ƃ��[�J���̗����ɕۑ��j")]
    public int highScore = 0;
    [Space(30)]

    [Header("���̃L���b�V���i���[�J���ۑ��f�[�^�j")]
    public bool isUnsavedHighScore;
    public Queue<int> rankingScoreQueue = new Queue<int>();   //playerScore�ɖ����f�̃����L���O���[�h�̃X�R�A
    public bool isTraining { get; private set; }
    public int trainingLevel;
    [Space(30)]

    [Header("�v���C���̃f�[�^��ێ�")]
    public int score;
    public int level;
    public int levelUpSpan {  get; private set; }
    public int playerRank = 0;  //totalExp����Z�o
    public int requiredExp;
    public int highestTrainingLevel { get; private set; } = 1;    // = trainingClearCounts.Count;
    public bool[] isSkinUnlocked { get; private set; } = new bool[16];    //totalExp��highestTrainingLevel����Z�o
    public int previousSkinID;
    public Color panelSelectedColor;
    public Vector2 centerPos_PlayerArea;
    public Queue<int> newSkinQueue = new Queue<int>();
    [Space(30)]

    [Header("�C���X�y�N�^�[����ݒ�")]
    public AuthManager AuthManager;
    public Transform playerTf;
    public PlayerController playerCon;
    public RectTransform menuHingeRtf_L;
    public RectTransform[] menuUIs_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform[] menuUIs_R;
    public RectTransform skinHingeRtf_U;
    public RectTransform[] skinUIs_U;
    public RectTransform skinHingeRtf_B;
    public RectTransform[] skinUIs_B;
    public RectTransform playHingeRtf_L;
    public RectTransform[] playUIs_L;
    public RectTransform playHingeRtf_R;
    public RectTransform[] playUIs_R;
    public RectTransform resultHingeRtf_L;
    public RectTransform[] resultUIs_L;
    public RectTransform resultHingeRtf_B;
    public RectTransform[] resultUIs_B;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public Transform levelTf;
    public Transform scoreSetTf;
    public Transform scoreGageTf;
    public RectTransform levelMarkerRtf_Play;
    public AnimationCurve scorePosY_PlaytoResult;
    public Transform scoreMarkerTf_Play;
    public Transform scoreMarkerTf_Result;
    public SpriteRenderer screenCover;
    public SpriteRenderer scoreGageSprite;
    public GameObject pauseUI;
    public TextMeshProUGUI countinueCountText;
    public GameObject[] countinueCircleSquares;
    public TextMeshProUGUI playButtonText;
    public TextMeshProUGUI retryButtonText;
    public GameObject resultTrainingUI;
    public TextMeshProUGUI clearRateText;
    public TextMeshProUGUI clearTimesNumText;
    public BoxCollider2D resultWallCol;
    [SerializeField] private Transform content_LevelSelecter;
    [SerializeField] private GameObject levelPanelPrefab;
    [HideInInspector] public List<Button_LevelSelecter> button_LevelSelecters = new List<Button_LevelSelecter>();
    public SkinSelecter skinSelecter;
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private GameObject skinModelCover;
    public RectTransform playerRankScaleRtf;
    public TextMeshProUGUI playerRankText;
    public TextMeshProUGUI addExpText;
    public TextMeshProUGUI requiredExpText;
    public SpriteRenderer expSprite;
    public GameObject unlockSkin;
    public SpriteRenderer unlockSkinModelSprite;
    public SpriteMask unlockSkinMask;
    public RectTransform unlockSkinModelRtf;
    public TextMeshProUGUI unlockSkinName;
    public RectTransform unlockSkinNameRtf;
    public TextMeshProUGUI unlockSkinMessage;
    public Sprite squarSprite;
    public Sprite cicleSprite;
    public SpriteRenderer frontScreenCover;
    public GameObject optionUIBase;
    public TextMeshProUGUI optionTitle;
    public GameObject optionUI_Account;

    //�X�e�[�g�}�V��
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //�萔�o�^�E�L��
    public int defaultFrameRate { get; private set; } = 120;
    public Vector2 centerPos_World { get; private set; } = new Vector2(5, 3);
    public Vector2 centerPos_PlayerArea_Result { get; private set; } = new Vector2(-1, 3);
    public Color screenCoverColor_Menu { get; private set; } = Color.white - Color.black * 0.2f;
    public Color screenCoverColor_Play { get; private set; } = Color.clear;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        #region ���^�[�{�b�N�X
        // ��i�c�����j�T�C�Y�Ɗ�A�X�y�N�g�䂩���������T�C�Y���Z�o
        var baseHorizontalSize = 5.622821f * 2560 / 1440;
        // ��������T�C�Y�ƑΏۃA�X�y�N�g��őΏۏc�����T�C�Y���Z�o
        var verticalSize = baseHorizontalSize / Camera.main.aspect;
        Camera.main.orthographicSize = verticalSize;
        #endregion

        //�ڕW�t���[�����[�g�ݒ�
        Application.targetFrameRate = defaultFrameRate;
    }



    private void Start()
    {
        //Manager�̃C���X�^���X�i�[
        TM = TerrainManager.Instance;
        FSM = FirestoreManager.Instance;
        SDB = SkinDataBase.Instance;

        //�X�e�[�g�}�V���̍쐬�E�i�[
        gameStateMachine = new GameStateStateMachine();
        gameStateMachine.Initialize(gameStateMachine.state_Login);

        //UI��Hinge�ɐڑ�
        for (int i = 0; i < menuUIs_L.Length; i++)
            menuUIs_L[i].SetParent(menuHingeRtf_L);
        for (int i = 0; i < menuUIs_R.Length; i++)
            menuUIs_R[i].SetParent(menuHingeRtf_R);
        for (int i = 0; i < skinUIs_B.Length; i++)
            skinUIs_B[i].SetParent(skinHingeRtf_B);
        for (int i = 0; i < skinUIs_U.Length; i++)
            skinUIs_U[i].SetParent(skinHingeRtf_U);
        for (int i = 0; i < playUIs_L.Length; i++)
            playUIs_L[i].SetParent(playHingeRtf_L);
        for (int i = 0; i < playUIs_R.Length; i++)
            playUIs_R[i].SetParent(playHingeRtf_R);
        for (int i = 0; i < resultUIs_L.Length; i++)
            resultUIs_L[i].SetParent(resultHingeRtf_L);
        for (int i = 0; i < resultUIs_B.Length; i++)
            resultUIs_B[i].SetParent(resultHingeRtf_B);

        //�����L���O���[�h�ŊJ�n
        SetTrainingMode(false);

        //�X�L���p�l���̐���
        skinSelecter.CreateSkinPanel();

        //�v���C���[�ړ��\�G���A�̒��S�̕ύX
        centerPos_PlayerArea = centerPos_World;
    }



    private void Update()
    {
        //�X�e�[�g�}�V����Update�����s
        gameStateMachine.Update(Time.deltaTime);
    }



    //�f�[�^�̃��[�h�ƃQ�[���ւ̔��f
    public async Task LoadAndReflect()
    {
        //���[�J���f�[�^�̃��[�h
        highScore = HighScoreManager.Load();
        rankingScoreQueue = RankingScoreManager.Load();
        isUnsavedHighScore = UnsavedHighScoreFlagManager.Load();

        //�N���E�h�ɖ����f�̃f�[�^������Δ��f
        if (isUnsavedHighScore)
            await FSM.SaveHighScore(highScore);
        if (rankingScoreQueue.Count > 0)
            await FSM.SavePlayerScore();

        //�N���E�h�f�[�^�̃��[�h
        //await FSM.SaveNewPlayerData();  //�V�K�A�J�E���g�쐬
        await FSM.LoadAll();


        //�v���C���[�����N�Z�o
        playerRank = CalculatePlayerRank(totalExp);

        //���̃����N�܂łɕK�v�Ȍo���l�ʂ��Z�o
        requiredExp = (playerRank + 1) * (playerRank + 2) / 2 * 100 - totalExp;


        //���N���A�̃g���[�j���O���[�h�̃��x����Count���m��
        if (trainingClearCounts[trainingClearCounts.Count - 1] > 0)
            trainingClearCounts.Add(0);

        //�g���[�j���O���[�h���B���x�����Z�o
        highestTrainingLevel = trainingClearCounts.Count;
        trainingLevel = highestTrainingLevel;

        //���B���x���܂ł̃g���[�j���O���[�h�{�^����z�u
        for (int i = 1; i <= highestTrainingLevel; i++)
            AddLevelPanel(i);

        //�N���A�񐔕\�����X�V
        UpdatePanelCount();


        //�\���X�L���ƃX�L���Z���N�^�[��]�ʂ̕ύX
        ChangePlayerSkin(usingSkinID);
        skinSelecter.SetWheelAngle(usingSkinID);

        //�X�L����S�ă��b�N
        for (int i = 0; i < isSkinUnlocked.Length; i++)
            isSkinUnlocked[i] = false;

        //�����X�L���̃��b�N����
        UnlockSkin(0);
    }



    //LevelSelecter�̃p�l����ǉ�
    public void AddLevelPanel(int level)
    {
        //�p�l���̃C���X�^���X�𐶐�
        RectTransform newPanelRtf = Instantiate(levelPanelPrefab).GetComponent<RectTransform>();

        //�X�N���[���r���[�̗v�f��
        newPanelRtf.SetParent(content_LevelSelecter);
        newPanelRtf.localScale = Vector3.one;
        newPanelRtf.anchoredPosition3D = newPanelRtf.anchoredPosition3D - Vector3.forward * newPanelRtf.anchoredPosition3D.z;
        newPanelRtf.localEulerAngles = Vector3.zero;

        //���̓X�N���v�g���Ǘ��\��
        Button_LevelSelecter button_LevelSelecter = newPanelRtf.GetComponent<Button_LevelSelecter>();
        button_LevelSelecter.SetLevel(level);
        button_LevelSelecters.Add(button_LevelSelecter);
    }



    //�����L���O���[�h - �g���[�j���O���[�h�؂�ւ�
    public void SetTrainingMode(bool isTraining)
    {
        //�ݒ��ύX
        this.isTraining = isTraining;

        //�g���[�j���O���[�h�֑J�ڎ�
        if (isTraining)
        {
            //�{�^���̃e�L�X�g��ύX
            playButtonText.SetText("�v���C - Lv." + trainingLevel);

            //�g���[�j���O���[�h�{�^��������
            button_LevelSelecters[trainingLevel - 1].PushButton();

            //���x���㏸�Ԋu��ύX
            levelUpSpan = 5000;

            //�w�i�̑��x��ύX
            TM.SetSpeed(5 + Mathf.Pow(trainingLevel, 0.7f) * 3);
            TM.moveSpeed = 5 + Mathf.Pow(trainingLevel, 0.7f) * 3;
        }

        //�����L���O���[�h�֑J�ڎ�
        else
        {
            //�{�^���̃e�L�X�g��ύX
            playButtonText.SetText("�v���C");

            //���x���㏸�Ԋu��ύX
            levelUpSpan = 2000;

            //�w�i�̑��x��ύX
            TM.SetSpeed(8);
            TM.moveSpeed = 8;
        }
    }



    //�g���[�j���O���[�h�̃v���C���x����ݒ�
    public void SetTrainingLevel(int level)
    {
        //���݂̑I��������
        button_LevelSelecters[trainingLevel - 1].Initialize();

        //�g���[�j���O�̃��x�����X�V
        trainingLevel = level;
        playButtonText.SetText("�v���C - Lv." + level);

        //�w�i�̑��x��ύX
        TM.SetSpeed(5 + Mathf.Pow(level, 0.7f) * 3);
        TM.moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;
    }



    //�g���[�j���O���[�h�̃��x����ǉ�
    public void AddTrainingLevel()
    {
        //�N���A�񐔕ۑ��ϐ��̒ǉ�
        trainingClearCounts.Add(0);

        //�ō����B���x�����X�V
        highestTrainingLevel++;

        //�g���[�j���O���[�h���x���I���p�l����ǉ�
        AddLevelPanel(highestTrainingLevel);

        //�g���[�j���O���[�h�̑I�����x�����X�V
        SetTrainingLevel(highestTrainingLevel);
        button_LevelSelecters[highestTrainingLevel - 1].PushButton();
    }



    //�N���A�񐔂̉��Z
    public async void AddClearCount(int level)
    {
        //�N���A�񐔂̉��Z
        trainingClearCounts[level - 1]++;

        //�p�l���ɕ\������Ă���񐔂̉��Z
        button_LevelSelecters[level - 1].clearTimesNumTMP.SetText(trainingClearCounts[level - 1] + "��");

        // Firestore�ɑ������f
        await FSM.SaveTrainingClearCount(level);
    }



    //�X�L����ύX
    public void ChangePlayerSkin(int skinID)
    {
        //GameManager���̃f�[�^���X�V
        usingSkinID = skinID;

        //�v���r���[�̃��f�����B���Ȃ�
        skinModelCover.SetActive(false);

        //�X�L���̕ύX
        playerCon.ChangeSkin(skinID);

        //�p�l���I�����̐F��ύX
        panelSelectedColor = SDB.skinData[skinID].UIColor;

        //�X�R�A�Q�[�W�̐F��ύX
        scoreGageSprite.color = SDB.skinData[skinID].UIColor - Color.black * 0.4f;

        //�o���l�o�[�F�̕ύX
        expSprite.color = Color.Lerp(SDB.skinData[skinID].UIColor, Color.gray + Color.white * 0.3f, 0.3f);

        //�{�^���̐F�̍X�V
        button_LevelSelecters[trainingLevel - 1].PushButton();

        //�\���X�L�����̕ύX
        skinNameText.SetText(SDB.skinData[skinID].name);

        //Crystal�X�L�����b�N���̗�O����
        if (!isSkinUnlocked[skinID])
            if (skinID % 8 == 7)
            {
                //�v���r���[�̃��f�����B��
                skinModelCover.SetActive(true);

                //���O��\�����Ȃ�
                skinNameText.SetText("�H�H�H");
            }
    }



    //�X�L���̃A�����b�N
    public void UnlockSkin(int skinID)
    {
        //���Ƀ��b�N�����ς݂Ȃ牽�����Ȃ�
        if (isSkinUnlocked[skinID]) return;

        //�A�����b�N��ۑ�
        isSkinUnlocked[skinID] = true;

        //�X�L���p�l���̃A�����b�N
        skinSelecter.UnlockPanel(skinID);
    }



    //�X�L���̃��b�N���������𐶐�
    public string GetUnlockSkinCondition(int skinID)
    {
        string condition = "�A�����b�N�����F\n";

        if (skinID < 8)
        {
            condition += "�v���C���[�����N";
            switch(skinID)
            {
                case 0: condition += "0"; break;
                case 1: condition += "1"; break;
                case 2: condition += "10"; break;
                case 3: condition += "20"; break;
                case 4: condition += "30"; break;
                case 5: condition += "40"; break;
                case 6: condition += "50"; break;
                case 7: condition += "100"; break;
            }
            condition += "�@���B";
        }
        else
        {
            condition += "�g���[�j���O���[�hLv.";
            switch (skinID)
            {
                case 8: condition += "1"; break;
                case 9: condition += "2"; break;
                case 10: condition += "3"; break;
                case 11: condition += "5"; break;
                case 12: condition += "7"; break;
                case 13: condition += "10"; break;
                case 14: condition += "15"; break;
                case 15: condition += "20"; break;
            }
            condition += "�@�N���A";
        }

        return condition;
    }



    //���l���o���l����v���C���[�����N���Z�o
    public int CalculatePlayerRank(int totalExp)
    {
        int playerRank = 0;

        while (totalExp > 0)
        {
            playerRank++;
            totalExp -= (playerRank + 1) * 100;
        }

        return playerRank;
    }



    //�X�L���A�����b�N��Ԃ̃`�F�b�N
    public void CheckSkinUnlock()
    {
        //Cube�X�L���̃`�F�b�N
        if (playerRank >= 1) UnlockSkin(1);
        if (playerRank >= 10) UnlockSkin(2);
        if (playerRank >= 20) UnlockSkin(3);
        if (playerRank >= 30) UnlockSkin(4);
        if (playerRank >= 40) UnlockSkin(5);
        if (playerRank >= 50) UnlockSkin(6);
        if (playerRank >= 100) UnlockSkin(7);

        //Sphere�X�L���̃`�F�b�N
        if (highestTrainingLevel > 1) UnlockSkin(8);
        if (highestTrainingLevel > 2) UnlockSkin(9);
        if (highestTrainingLevel > 3) UnlockSkin(10);
        if (highestTrainingLevel > 5) UnlockSkin(11);
        if (highestTrainingLevel > 7) UnlockSkin(12);
        if (highestTrainingLevel > 10) UnlockSkin(13);
        if (highestTrainingLevel > 15) UnlockSkin(14);
        if (highestTrainingLevel > 20) UnlockSkin(15);
    }



    //�g���[�j���O���[�h�p�l���̃N���A�񐔂̍X�V
    public void UpdatePanelCount()
    {
        for (int levelIndex = 0; levelIndex < highestTrainingLevel; levelIndex++)
            button_LevelSelecters[levelIndex].clearTimesNumTMP.SetText(trainingClearCounts[levelIndex] + "��");
    }



    //�v���C���ʂ̃Z�[�u
    public async void SaveResult()
    {
        #region ���s�����̉��Z
        int addDistance = 0;
        //�����L���O���[�h�̏ꍇ��LV.1����̋��������Z
        if (!isTraining)
            for (int calcLevel = 1; calcLevel < level; calcLevel++)
                addDistance += (int)((5 + Mathf.Pow(calcLevel, 0.7f) * 3) * 20);

        //�ŏI���x���̑��s���������Z
        addDistance += (int)((5 + Mathf.Pow(level, 0.7f) * 3) * (isTraining ? score / 100 : (score - (level - 1) * 2000) / 100));

        //���[�J���ɔ��f
        totalRunDistance += addDistance;

        //�N���E�h�ɕۑ�
        FSM.SaveRunDistance(addDistance);
        #endregion

        #region �o���l�̉��Z
        //�Q�[�����[�h�ɉ��������Z�ʎZ�o
        int addExp = isTraining ? addDistance / 10 : addDistance;

        //���[�J���ɔ��f
        totalExp += addExp;

        //�v���C���[�����N���X�V
        requiredExp -= addExp;
        while (requiredExp < 0)
        {
            playerRank++;
            requiredExp += (playerRank + 1) * 100;
        }

        //�N���E�h�ɕۑ�
        FSM.SaveExperience(addExp);
        #endregion

        //�g���[�j���O���[�h�Ȃ珈���I��
        if (isTraining) return;

        #region �n�C�X�R�A�̕ۑ�
        //�n�C�X�R�A���X�V���Ă����
        if (score > highScore)
        {
            //�n�C�X�R�A�̍X�V
            highScore = score;

            //���[�J���ɕۑ�
            HighScoreManager.Save(highScore);

            //�N���E�h�ɕۑ��i���s����΃t���O�����j
            FSM.SaveHighScore(highScore);
        }
        #endregion

        #region �v���C���[�X�R�A�̕ۑ�
        //Queue�ɃX�R�A���i�[
        rankingScoreQueue.Enqueue(score);

        //�N���E�h�ɕۑ�
        //���[�J���ւ̕ۑ��̓��\�b�h��
        FSM.SavePlayerScore();

        //�ϐ��ɂ����f
        await FSM.LoadPlayerScore();
        #endregion
    }
}
