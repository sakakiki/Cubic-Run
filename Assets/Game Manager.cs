using TMPro;
using UnityEngine;

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

    //�X�e�[�g�}�V��
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //�萔�o�^�E�L��
    public Vector2 centerPos_World = new Vector2(5, 3);
    public Color screenCoverColor_Menu = Color.white - Color.black * 0.2f;
    public Color screenCoverColor_Play = Color.clear;



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
        Application.targetFrameRate = 60;
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
        highestLevel = 5;
        trainingLevel = highestLevel;

        //���B���x���܂ł̃g���[�j���O���[�h�{�^����z�u
        for (int i = 1; i <= highestLevel; i++)
            InputManager.Instance.AddLevelPanel(i);
    }



    private void Update()
    {
        //�X�e�[�g�}�V����Update�����s
        gameStateMachine.Update(Time.deltaTime);
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
            InputManager.Instance.button_LevelSelecters[trainingLevel - 1].PushButton();

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
        InputManager.Instance.button_LevelSelecters[trainingLevel - 1].enabled = true;

        //�g���[�j���O�̃��x�����X�V
        trainingLevel = level;
        playButtonText.SetText("�v���C - Lv." + level);

        //�w�i�̑��x��ύX
        TerrainManager.Instance.SetSpeed(5 + Mathf.Pow(level, 0.7f) * 3);
        TerrainManager.Instance.moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;
    }
}
