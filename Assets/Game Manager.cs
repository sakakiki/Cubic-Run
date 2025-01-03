using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static GameManager Instance;

    //�C���X�y�N�^�[����ݒ�\
    public int score = 0;
    public int level = 0;
    public int levelUpSpan = 2000;
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
    public RectTransform resultHingeRtf_B;
    public RectTransform[] resultUIs_B;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public Transform levelTf;
    public Transform scoreSetTf;
    public RectTransform levelMarkerRtf_Play;
    public AnimationCurve scorePosY_PlaytoResult;
    public Transform scoreMarkerTf_Play;
    public Transform scoreMarkerTf_Result;
    public SpriteRenderer screenCover;
    public GameObject PauseUI;
    public TextMeshProUGUI countinueCountText;
    public GameObject[] countinueCircleSquares;

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



    void Start()
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
        for (int i = 0; i < resultUIs_B.Length; i++)
            resultUIs_B[i].SetParent(resultHingeRtf_B);
    }



    void Update()
    {
        gameStateMachine.Update(Time.deltaTime);
    }
}
