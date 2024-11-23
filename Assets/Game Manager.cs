using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public RectTransform levelRtf;

    //�X�e�[�g�}�V��
    private PlayStateStateMachine stateMachine;

    //�萔�o�^�E�L��
    public Vector2 centerAPos_TopLeft = new Vector2(1920, -1080);
    public Vector2 levelAPos = new Vector2(450, -350);



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    void Start()
    {
        stateMachine = new PlayStateStateMachine();
        stateMachine.Initialize(stateMachine.state_LevelStart);
    }



    void Update()
    {
        stateMachine.Update();
    }
}
