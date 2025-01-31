using TMPro;
using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static GameManager Instance;

    //�v���C���̃f�[�^��ێ�
    public int score;
    public int level;
    public int levelUpSpan;
    public int highScore;
    public bool isTraining {  get; private set; }
    public int trainingLevel;
    public int highestLevel;
    public List<int> clearTimesNum = new List<int>();
    public Vector2 centerPos_PlayerArea;

    //�C���X�y�N�^�[����ݒ�\
    public Transform playerTf;
    public PlayerController playerCon;
    public RectTransform menuHingeRtf_L;
    public RectTransform[] menuUIs_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform[] menuUIs_R;
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

    //�X�e�[�g�}�V��
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //�萔�o�^�E�L��
    public int defaultFrameRate = 120;
    public Vector2 centerPos_World { get; private set; } = new Vector2(5, 3);
    public Vector2 centerPos_PlayerArea_Result { get; private set; } = new Vector2(-1, 3);
    public Color screenCoverColor_Menu { get; private set; } = Color.white - Color.black * 0.2f;
    public Color screenCoverColor_Play { get; private set; } = Color.clear;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameStateMachine = new GameStateStateMachine();
        gameStateMachine.Initialize(gameStateMachine.state_Menu);

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
        //UI��Hinge�ɐڑ�
        for (int i = 0; i < menuUIs_L.Length; i++)
            menuUIs_L[i].SetParent(menuHingeRtf_L);
        for (int i = 0; i < menuUIs_R.Length; i++)
            menuUIs_R[i].SetParent(menuHingeRtf_R);
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



        /* �ȉ��J���p */

        //���B���x����ݒ�
        highestLevel = 1;
        trainingLevel = highestLevel;

        for (int i = 1; i <= highestLevel; i++)
        {
            //���B���x���܂ł̃g���[�j���O���[�h�{�^����z�u
            AddLevelPanel(i);

            //�N���A�񐔕ۑ��ϐ��̒ǉ�
            clearTimesNum.Add(0);
        }

        //�v���C���[�ړ��\�G���A�̒��S�̕ύX
        centerPos_PlayerArea = centerPos_World;
    }



    private void Update()
    {
        //�X�e�[�g�}�V����Update�����s
        gameStateMachine.Update(Time.deltaTime);
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
            TerrainManager.Instance.SetSpeed(5 + Mathf.Pow(trainingLevel, 0.7f) * 3);
            TerrainManager.Instance.moveSpeed = 5 + Mathf.Pow(trainingLevel, 0.7f) * 3;
        }

        //�����L���O���[�h�֑J�ڎ�
        else
        {
            //�{�^���̃e�L�X�g��ύX
            playButtonText.SetText("�v���C");

            //���x���㏸�Ԋu��ύX
            levelUpSpan = 2000;

            //�w�i�̑��x��ύX
            TerrainManager.Instance.SetSpeed(8);
            TerrainManager.Instance.moveSpeed = 8;
        }
    }



    public void SetTrainingLevel(int level)
    {
        //���݂̑I��������
        button_LevelSelecters[trainingLevel - 1].Initialize();

        //�g���[�j���O�̃��x�����X�V
        trainingLevel = level;
        playButtonText.SetText("�v���C - Lv." + level);

        //�w�i�̑��x��ύX
        TerrainManager.Instance.SetSpeed(5 + Mathf.Pow(level, 0.7f) * 3);
        TerrainManager.Instance.moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;
    }



    public void AddTrainingLevel()
    {
        //�ō����B���x�����X�V
        highestLevel++;

        //�g���[�j���O���[�h���x���I���p�l����ǉ�
        AddLevelPanel(highestLevel);

        //�g���[�j���O���[�h�̑I�����x�����X�V
        SetTrainingLevel(highestLevel);
        button_LevelSelecters[highestLevel - 1].PushButton();

        //�N���A�񐔕ۑ��ϐ��̒ǉ�
        clearTimesNum.Add(0);
    }



    public void AddClearTimesNum(int level)
    {
        //�N���A�񐔂̉��Z
        clearTimesNum[level - 1]++;

        //�p�l���ɕ\������Ă���񐔂̉��Z
        button_LevelSelecters[level - 1].clearTimesNumTMP.SetText(clearTimesNum[level - 1] + "��");
    }
}
