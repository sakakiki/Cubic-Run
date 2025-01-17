using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //自身のインスタンス
    public static GameManager Instance;

    //プレイ中のデータを保持
    public int score;
    public int level;
    public int levelUpSpan;
    public bool isTraining {  get; private set; }
    public int trainingLevel;
    public int highestLevel;

    //インスペクターから設定可能
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
    public GameObject pauseUI;
    public TextMeshProUGUI countinueCountText;
    public GameObject[] countinueCircleSquares;
    public TextMeshProUGUI playButtonText;

    //ステートマシン
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //定数登録・記憶
    public Vector2 centerPos_World = new Vector2(5, 3);
    public Color screenCoverColor_Menu = Color.white - Color.black * 0.2f;
    public Color screenCoverColor_Play = Color.clear;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameStateMachine = new GameStateStateMachine();
        gameStateMachine.Initialize(gameStateMachine.state_Menu);

        #region レターボックス
        // 基準（縦方向）サイズと基準アスペクト比から基準横方向サイズを算出
        var baseHorizontalSize = 5.622821f * 2560 / 1440;
        // 基準横方向サイズと対象アスペクト比で対象縦方向サイズを算出
        var verticalSize = baseHorizontalSize / Camera.main.aspect;
        Camera.main.orthographicSize = verticalSize;
        #endregion

        //目標フレームレート設定
        Application.targetFrameRate = 60;
    }



    private void Start()
    {
        //UIをHingeに接続
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

        //ランキングモードで開始
        SetTrainingMode(false);



        /* 以下開発用 */

        //Lv.10までのトレーニングモードボタンを配置
        for (int i = 1; i <= 10; i++)
            InputManager.Instance.AddLevelPanel(i);

        highestLevel = 10;
        trainingLevel = highestLevel;
    }



    private void Update()
    {
        gameStateMachine.Update(Time.deltaTime);
    }



    public void SetTrainingMode(bool isTraining)
    {
        //設定を変更
        this.isTraining = isTraining;

        //トレーニングモードへ遷移時
        if (isTraining)
        {
            //ボタンのテキストを変更
            playButtonText.SetText("プレイ - Lv." + trainingLevel);

            //トレーニングモードボタンを押下
            InputManager.Instance.button_LevelSelecters[trainingLevel - 1].PushButton();

            //レベル上昇間隔を変更
            levelUpSpan = 5000;
        }

        //ランキングモードへ遷移時
        else
        {
            //ボタンのテキストを変更
            playButtonText.SetText("プレイ");

            //レベル上昇間隔を変更
            levelUpSpan = 2000;
        }
    }



    public void SetTrainingLevel(int level)
    {
        //現在の選択を解除
        InputManager.Instance.button_LevelSelecters[trainingLevel - 1].enabled = true;

        //トレーニングのレベルを更新
        trainingLevel = level;
        playButtonText.SetText("プレイ - Lv." + level);
    }
}
