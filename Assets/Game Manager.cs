using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //自身のインスタンス
    public static GameManager Instance;

    //インスペクターから設定可能
    public int score = 0;
    public int level = 0;
    public int levelUpSpan = 2000;
    public Transform playerTf;
    public PlayerController playerCon;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public RectTransform levelRtf;

    //ステートマシン
    private PlayStateStateMachine stateMachine;

    //定数登録・記憶
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
