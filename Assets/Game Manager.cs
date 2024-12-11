using TMPro;
using UnityEngine;

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
    public RectTransform menuHingeRtf_L;
    public RectTransform[] menuUIs_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform[] menuUIs_R;
    public RectTransform playHingeRtf_L;
    public RectTransform[] playUIs_L;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public RectTransform levelRtf;
    public SpriteRenderer screenCover;

    //ステートマシン
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //定数登録・記憶
    public Vector2 centerPos_World = new Vector2(5, 3);
    public Vector2 centerAPos_TopLeft = new Vector2(1920, -1080);
    public Vector2 levelAPos = new Vector2(450, -350);
    public Color screenCoverColor_Menu = Color.white - Color.black * 0.2f;
    public Color screenCoverColor_Play = Color.clear;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameStateMachine = new GameStateStateMachine();
        gameStateMachine.Initialize(gameStateMachine.state_Menu);
    }



    void Start()
    {
        //UIをHingeに接続
        for (int i = 0; i < menuUIs_L.Length; i++)
            menuUIs_L[i].SetParent(menuHingeRtf_L);
        for (int i = 0; i < menuUIs_R.Length; i++)
            menuUIs_R[i].SetParent(menuHingeRtf_R);
        for (int i = 0; i < playUIs_L.Length; i++)
            playUIs_L[i].SetParent(playHingeRtf_L);
    }



    void Update()
    {
        gameStateMachine.Update(Time.deltaTime);
    }
}
