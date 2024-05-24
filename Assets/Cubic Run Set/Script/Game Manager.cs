using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using Unity.VisualScripting;
using static ModelScript;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    private void Awake()
    {
        #region シングルトンパターン
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
        #endregion
    }

    #region インスペクター登録
    public int StartLevel = 1;
    public int LevelUpScore = 2000;

    [SerializeField] private RectTransform canvas;

    [SerializeField] private GameObject ScreenCover;

    [SerializeField] private GameObject MenuModel_Cube;
    [SerializeField] private GameObject MenuModel_Ball;

    [SerializeField] private GameObject[] MenuButtons;
    /*
    0:プレイ
    1:チュートリアル
    2:スキン
    3:オプション
    */

    [SerializeField] private GameObject[] PauseButtons;
    /*
    0:コンティニュー
    1:リタイア
    2:オプション
    */

    [SerializeField]
    private GameObject[] SkinSelectButtons;
    /*
    0:OK
    1:キャンセル
    */

    [SerializeField] private GameObject HingeLeft;
    [SerializeField] private GameObject HingeRight;
    [SerializeField] private GameObject HingeUnder;
    [SerializeField] private GameObject HingeUp;

    [SerializeField] private GameObject[] ObjectSet_Menu_MoveToLeft;
    [SerializeField] private GameObject[] ObjectSet_Menu_MoveToRight;

    [SerializeField] private GameObject PlayerObjectSet_Cube;
    [SerializeField] private GameObject PlayerObjectSet_Ball;

    [SerializeField] private GameObject[] UISet_Play;
    /*
    0:Score & Level
    1:Score Set
    2:Level
    3:UI Pause Button
    */

    [SerializeField] private GameObject PauseUI;

    [SerializeField] private GameObject CountCircle;

    [SerializeField] private TextMeshProUGUI CountinueCountText;

    [SerializeField] private GameObject[] ObstacleSet;
    /*
    0:_Default
    1:_Hole
    2:_Tunnel
    3:_Cave
    4:_Hill
    5:_Enemy
     */

    [SerializeField] private TextMeshProUGUI SkinName;
    [SerializeField] private GameObject SkinSelecter;
    [SerializeField] private GameObject SkinSelecterWheel;
    [SerializeField] private SkinData[] SkinList;
    [SerializeField] private GameObject[] SkinSelecterBoards;
    #endregion

    #region ステート管理

    #region ゲームステート
    public enum GameState
    {
        Menu,
        ExitMenu,
        EnterMenu,
        GameStart,
        Play,
        SkinSelect,
        Tutorial,
        Option
    }

    [System.NonSerialized] public GameState currentGameState = GameState.Menu;
    private bool gameStateEnter = true;
    private GameState connectedMode;

    private void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        gameStateEnter = true;
    }
    #endregion

    #region プレイステート
    public enum PlayState
    {
        Play,
        LevelUp,
        Pause,
        GameOver
    }

    [System.NonSerialized] public PlayState currentPlayState = PlayState.LevelUp;
    private bool playStateEnter = true;
    private PlayState previousPlayState;

    private void ChangePlayState(PlayState newPlayState)
    {
        previousPlayState = currentPlayState;
        currentPlayState = newPlayState;
        playStateEnter = previousPlayState != PlayState.Pause;
    }
    #endregion

    private float stateEnterTime;
    #endregion

    #region ユーザー設定変数
    [SerializeField] private SkinData UseSkin;
    [System.NonSerialized] public int skinNumber;
    private GameObject MenuModel;
    private GameObject PlayerObjectSet;
    #endregion

    #region オブジェクトプール用変数
    private ObjectPool<GameObject>[] Pool;
    private int createObjectNumber;
    #endregion

    #region オブスタクル制御用変数
    private float moveSpeed;
    private List<GameObject> obstacles = new List<GameObject>();
    private List<int> obstacleNumbers = new List<int>();
    private GameObject previousObstacle;
    private int chooseObstacle;
    private int previousObstacleNum1;
    private int previousObstacleNum2;
    private float stageRightEdge;
    private float obstacleWidth;
    private float obstacleHeight;
    #endregion

    #region UI制御用変数
    private Color colorChange = new Color(0, 0, 0, 1f);
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI levelText;
    private RectTransform scoreboardRectTransform;
    private RectTransform pauseButtonRectTransform;
    private RectTransform scoreRectTransform;
    private RectTransform levelRectTransform;
    private Vector2 centerPosition_top_left;
    private Vector2 scoreboardPosition_Menu = new Vector2(-1000, 600);
    private Vector2 pauseButtonPosition_Menu = new Vector2(120, 120);
    private Vector2 levelTextPosition_Play = new Vector2(300, -260);
    private GameObject[] CounterCircleRects = new GameObject[30];
    private int countinueCountDown = 4;
    private int countinueCounterRectNum = 0;
    private Rigidbody2D modelRb;
    private bool processFlag;
    private SkinData previewSkin;
    private int useSkinNumber;
    private Vector3 selectorEulerAngles;
    private bool isUseSkin;
    private Vector3 skinSelectScale;
    private Vector2 modelVelocity;
    #endregion

    #region プレイ時処理用変数
    [System.NonSerialized] public GameObject Player;
    private int level;
    private float playStartTime;
    private int score;
    private int scoreCorrection;
    #endregion

    void Start()
    {
        Application.targetFrameRate = 60;

        #region 障害物オブジェクトプール
        Pool = new ObjectPool<GameObject>[ObstacleSet.Length];
        for (int i = 0; i < ObstacleSet.Length; i++)
        {
            createObjectNumber = i;
            Pool[createObjectNumber] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(ObstacleSet[createObjectNumber]),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: true) ;
        }
        #endregion

        #region 変数登録
        scoreText = UISet_Play[1].transform.Find("Score").GetComponent<TextMeshProUGUI>();
        levelText = UISet_Play[2].GetComponent<TextMeshProUGUI>();
        scoreboardRectTransform = UISet_Play[0].GetComponent<RectTransform>();
        pauseButtonRectTransform = UISet_Play[3].GetComponent<RectTransform>();
        scoreRectTransform = UISet_Play[1].transform.Find("Score").GetComponent<RectTransform>();
        levelRectTransform = UISet_Play[2].GetComponent<RectTransform>();
        centerPosition_top_left = new Vector2 (canvas.sizeDelta.x/2, -canvas.sizeDelta.y/2);
        #endregion

        #region UI-Hinge接続
        foreach(GameObject obj in ObjectSet_Menu_MoveToLeft)
        {
            obj.transform.SetParent(HingeLeft.transform);
        }
        foreach(GameObject obj in ObjectSet_Menu_MoveToRight)
        {
            obj.transform.SetParent (HingeRight.transform);
        }
        #endregion

        #region カウントサークル配置
        foreach (Transform countRect in CountCircle.transform)
        {
            countRect.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(-(float)Math.Sin(Math.PI * countinueCounterRectNum / 15), (float)Math.Cos(Math.PI * countinueCounterRectNum / 15)) * 200;
            countRect.GetComponent<RectTransform>().eulerAngles = new Vector3 (0, 0, countinueCounterRectNum * 12);
            CounterCircleRects[countinueCounterRectNum++] = countRect.gameObject;
        }
        countinueCounterRectNum = 30;
        CountCircle.SetActive(false);
        #endregion

        CountinueCountText.enabled = false;


        for (int i = 0; i < SkinSelecterBoards.Length; i++)
        {
            SkinSelecterBoards[i].GetComponent<SkinSelecterBoardScript>().modelColor = SkinList[i].color;
            SkinSelecterBoards[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = SkinList[i].name;
        }
    }


    void LateUpdate()
    {
        #region レターボックス
        // 基準（縦方向）サイズと基準アスペクト比から基準横方向サイズを算出
        var baseHorizontalSize = 5.5f * 16/9;
        // 基準横方向サイズと対象アスペクト比で対象縦方向サイズを算出
        var verticalSize = baseHorizontalSize / Camera.main.aspect;
        Camera.main.orthographicSize = verticalSize;
        #endregion

        switch (currentGameState)
        {
            #region メニュー画面
            case GameState.Menu:
                if(gameStateEnter)
                {
                    gameStateEnter = false;
                    scoreboardRectTransform.anchoredPosition = scoreboardPosition_Menu;
                    pauseButtonRectTransform.anchoredPosition = pauseButtonPosition_Menu;
                    ScreenCover.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                    switch (UseSkin.bodyType)
                    {
                        case SkinData.BodyType.Cube: MenuModel = MenuModel_Cube; break;
                        case SkinData.BodyType.Ball: MenuModel = MenuModel_Ball; break;
                    }
                    MenuModel.transform.Find("Menu Model Skin").GetComponent<SpriteRenderer>().color = UseSkin.color;
                    modelRb = MenuModel.GetComponent<Rigidbody2D>();
                    MenuModel.SetActive(true);
                    modelRb.isKinematic = true;
                    MenuModel.transform.localScale = Vector3.one;
                    modelRb.velocity = Vector3.zero;
                    modelRb.gravityScale = 5;
                    MenuModel.GetComponent<RectTransform>().anchoredPosition = Vector3.up * 200;
                    MenuModel.GetComponent<Collider2D>().enabled = true;
                    SkinName.text = UseSkin.name;
                    currentModelState = ModelState.Idle;
                    MenuModelEyeScript.modelScale = 1;
                    HingeLeft.transform.eulerAngles = Vector3.zero;
                    HingeRight.transform.eulerAngles = Vector3.zero;
                    HingeUnder.transform.eulerAngles = Vector3.up * 90;
                    HingeUp.transform.eulerAngles = Vector3.up * 90;
                    foreach (GameObject button in MenuButtons)
                        button.GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                    if(obstacles.Count == 0)
                        ObstacleCreate(0, 0, 25, 1, 8);
                }

                RandomObstacleCreate(8);
                DeleteObstacle();

                for(int buttonNum = 0; buttonNum < MenuButtons.Length; buttonNum++)
                {
                    if (MenuButtons[buttonNum].GetComponent<ButtonScript_Normal>().isButtonPushed)
                    {
                        MenuButtons[buttonNum].GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                        switch (buttonNum)
                        {
                            case 0: connectedMode = GameState.Play; break;
                            case 1: connectedMode = GameState.Tutorial; break;
                            case 2: connectedMode = GameState.SkinSelect; break;
                            case 3: connectedMode = GameState.Option; break;
                        }
                        ChangeGameState(GameState.ExitMenu);
                    }
                }
                break;
            #endregion



            #region 画面遷移（メニュー画面から遷移）
            case GameState.ExitMenu:
                if (gameStateEnter)
                {
                    gameStateEnter = false;
                    stateEnterTime = Time.time;
                    processFlag = false;
                    if(connectedMode == GameState.Play || connectedMode == GameState.Tutorial)
                        foreach (GameObject obj in obstacles)
                            obj.GetComponent<Rigidbody2D>().velocity = Vector3.left * 20;
                }

                if (connectedMode == GameState.Play || connectedMode == GameState.Tutorial)
                    ScreenCover.GetComponent<SpriteRenderer>().color -= colorChange * Time.deltaTime * 0.5f;


                if (Time.time - stateEnterTime > 0.4 && Time.time - stateEnterTime < 0.6)
                {
                    MenuModel.transform.localScale -= Vector3.up * Time.deltaTime * 2;
                    MenuModel.transform.localScale += Vector3.right * Time.deltaTime;
                }
                else if(Time.time - stateEnterTime > 0.8)
                {
                    if (connectedMode == GameState.SkinSelect)
                    {
                        if (!processFlag)
                        {
                            processFlag = true;
                            MenuModel.transform.localScale = Vector2.one;
                            modelRb.gravityScale = 2;
                            if (modelRb.isKinematic)
                            {
                                modelRb.isKinematic = false;
                                modelRb.velocity = Vector2.up * 7.2f;
                            }
                            else
                            {
                                modelRb.velocity = Vector2.up * 8.6f;
                                modelRb.velocity += Vector2.left * (MenuModel.transform.position.x - 10) / 0.8f;
                            }
                        }
                        MenuModel.transform.localScale *= 1.005f;
                        MenuModelEyeScript.modelScale *= 1.005f;
                    }
                    else if (!processFlag)
                    {
                        processFlag= true;
                        MenuModel.transform.localScale = new Vector2(0.9f, 1.1f);
                        MenuModel.GetComponent<Collider2D>().enabled = false;
                        modelRb.velocity = Vector2.up * 50;
                    }
                }

                HingeLeft.transform.eulerAngles += Vector3.down * Time.deltaTime * 90;
                HingeRight.transform.eulerAngles += Vector3.up * Time.deltaTime * 90;
                if (connectedMode == GameState.SkinSelect)
                {
                    if(HingeUnder.transform.eulerAngles.z > 180 || HingeUnder.transform.eulerAngles.z == 0)
                        HingeUnder.transform.eulerAngles += Vector3.back * Time.deltaTime * ((HingeUnder.transform.eulerAngles.z) % 180 * 3 + 5);
                    else HingeUnder.transform.eulerAngles = Vector3.forward * 180 + Vector3.up * 90;
                    if(HingeUp.transform.eulerAngles.z < 180)
                        HingeUp.transform.eulerAngles += Vector3.forward * Time.deltaTime * 125;
                    else HingeUp.transform.eulerAngles = Vector3.forward * 180 + Vector3.up * 90;
                }

                stageRightEdge = previousObstacle.transform.position.x + previousObstacle.transform.localScale.x;
                if (stageRightEdge < 25)
                {
                    switch (connectedMode)
                    {
                        case GameState.Play:
                        case GameState.Tutorial: 
                            ObstacleCreate(0, stageRightEdge, 3, 1, 20); break;
                        case GameState.SkinSelect:
                        case GameState.Option:
                            RandomObstacleCreate(8); break;
                    }
                    
                }

                if (Time.time - stateEnterTime > 1.6f)
                {
                    HingeRight.transform.eulerAngles = Vector3.up * 150;
                    HingeLeft.transform.eulerAngles = Vector3.down * 150;
                    switch (connectedMode)
                    {
                        case GameState.Play: ChangeGameState(GameState.GameStart); break;
                        case GameState.Tutorial: break;
                        case GameState.SkinSelect: ChangeGameState(GameState.SkinSelect); break;
                    }
                }
                    
                    
                break;
            #endregion



            #region 画面遷移（メニュー画面へ遷移）
            case GameState.EnterMenu:

                switch (connectedMode)
                {

                    #region 直前画面：プレイ（ポーズ）
                    case GameState.Play:
                        if (gameStateEnter)
                        {
                            gameStateEnter = false;
                            processFlag = false;
                            stateEnterTime = Time.time;
                            PauseUI.SetActive(false);
                            ScreenCover.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                            foreach (GameObject obj in obstacles)
                                obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                            Player.GetComponent<Rigidbody2D>().velocity = Vector2.up * 60;
                            Player.GetComponent<Collider2D>().enabled = false;
                            Player.transform.localScale = new Vector3(0.9f, 1.1f, 1);
                        }

                        switch(Time.time - stateEnterTime)
                        {
                            case < 0.2f: break;
                            case < 0.6f:
                                if (!processFlag) 
                                {
                                    processFlag = true;
                                }
                                break;
                            case < 1.2f:
                                if (processFlag)
                                {
                                    processFlag = false;
                                    modelVelocity = Player.GetComponent<Rigidbody2D>().velocity;
                                    PlayerObjectSet.SetActive(false);
                                    foreach (GameObject obj in obstacles)
                                        obj.GetComponent<Rigidbody2D>().velocity = Vector2.left * 8;
                                }
                                break;
                            default:
                                if (!processFlag)
                                {
                                    processFlag = true;
                                    MenuModel.SetActive(true);
                                    MenuModel.transform.position = Vector2.up * 30 + Vector2.right * 10;
                                    modelRb.velocity = Vector2.down * 50;
                                    modelRb.isKinematic = false;
                                }
                                if (MenuModel.GetComponent<RectTransform>().anchoredPosition.y < 200)
                                {
                                    MenuModel.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 200;
                                    modelRb.velocity = Vector2.zero;
                                    modelRb.isKinematic = true;
                                }
                                break;
                        }

                        scoreboardRectTransform.anchoredPosition += scoreboardPosition_Menu * Time.deltaTime * 1.6f;
                        pauseButtonRectTransform.anchoredPosition += pauseButtonPosition_Menu * Time.deltaTime * 3.2f;
                        ScreenCover.GetComponent<SpriteRenderer>().color += colorChange * Time.deltaTime * 0.5f;

                        break;
                    #endregion

                    #region 直前画面：スキン選択
                    case GameState.SkinSelect:
                        if (gameStateEnter)
                        {
                            gameStateEnter = false;
                            stateEnterTime = Time.time;
                            processFlag = false;
                            skinSelectScale = MenuModel.transform.localScale;
                        }
                        if (HingeUnder.transform.eulerAngles.z >= 180)
                            HingeUnder.transform.eulerAngles += Vector3.forward * Time.deltaTime * 125;
                        else HingeUnder.transform.eulerAngles = Vector3.up * 90;
                        if (HingeUp.transform.eulerAngles.z <= 180)
                            HingeUp.transform.eulerAngles += Vector3.back * Time.deltaTime * 125;
                        else HingeUp.transform.eulerAngles = Vector3.up * 90;

                        if (Time.time - stateEnterTime > 0.4 && Time.time - stateEnterTime < 0.6)
                        {
                            MenuModel.transform.localScale -= Vector3.up * Time.deltaTime * 2.6f;
                            MenuModel.transform.localScale += Vector3.right * Time.deltaTime * 1.3f;
                        }
                        else if (Time.time - stateEnterTime > 0.8)
                        {
                            MenuModel.transform.localScale /= 1.005f;
                            MenuModelEyeScript.modelScale /= 1.005f;
                            if (isUseSkin)
                            {
                                if (!processFlag)
                                {
                                    processFlag = true;
                                    modelRb.isKinematic = false;
                                    modelRb.gravityScale = 2;
                                    MenuModel.transform.localScale = skinSelectScale;
                                    modelRb.velocity = Vector2.up * 8.7f;
                                }
                            }
                            else
                            {
                                if(Time.time - stateEnterTime < 1.2)
                                {
                                    if (!processFlag)
                                    {
                                        processFlag = true;
                                        modelRb.isKinematic = false;
                                        modelRb.gravityScale = 2;
                                        MenuModel.transform.localScale = skinSelectScale;
                                        MenuModel.GetComponent<Collider2D>().enabled = false;
                                        MenuModel.transform.localScale = skinSelectScale;
                                        modelRb.velocity = Vector2.up * 50;
                                    }
                                }
                                else
                                {
                                    if (processFlag)
                                    {
                                        processFlag = false;
                                        modelVelocity = modelRb.velocity;
                                        MenuModel_Cube.transform.position = MenuModel.transform.position;
                                        MenuModel_Ball.transform.position = MenuModel.transform.position;
                                        MenuModel_Cube.transform.localScale = MenuModel.transform.localScale;
                                        MenuModel_Ball.transform.localScale = MenuModel.transform.localScale;
                                        if (UseSkin.bodyType != previewSkin.bodyType)
                                        {
                                            MenuModel.GetComponent<Collider2D>().enabled = true;
                                            switch (UseSkin.bodyType)
                                            {
                                                case SkinData.BodyType.Cube: MenuModel = MenuModel_Cube; break;
                                                case SkinData.BodyType.Ball: MenuModel = MenuModel_Ball; break;
                                            }
                                            MenuModel_Cube.SetActive(MenuModel == MenuModel_Cube);
                                            MenuModel_Ball.SetActive(MenuModel == MenuModel_Ball);
                                        }
                                        modelRb = MenuModel.GetComponent<Rigidbody2D>();
                                        modelRb.isKinematic = false;
                                        modelRb.velocity = -modelVelocity;
                                        MenuModel.GetComponent<Collider2D>().enabled = false;
                                        MenuModel.GetComponent<RectTransform>().anchoredPosition += Vector2.up * 32;
                                        MenuModel.transform.Find("Menu Model Skin").GetComponent<SpriteRenderer>().color = UseSkin.color;
                                    }
                                    if (MenuModel.GetComponent<RectTransform>().anchoredPosition.y < 200)
                                        MenuModel.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 200;
                                }
                            }
                        }

                        if (Time.time - stateEnterTime > 1.6f)
                        {
                            if (!isUseSkin)
                            {
                                SkinSelecterWheel.transform.localEulerAngles = selectorEulerAngles;
                                SkinSelecter.GetComponent<SkinSelecterScript>().skinNumber = useSkinNumber;
                            }
                        }
                        break;
                        #endregion
                }

                HingeLeft.transform.eulerAngles += Vector3.up * Time.deltaTime * 90;
                HingeRight.transform.eulerAngles += Vector3.down * Time.deltaTime * 90;
                RandomObstacleCreate(8);

                if (Time.time - stateEnterTime > 1.6f) ChangeGameState(GameState.Menu);
                break;
            #endregion



                    #region 画面遷移（プレイ用オブジェクト生成）
            case GameState.GameStart:
                if (gameStateEnter)
                {
                    gameStateEnter = false;
                    stateEnterTime = Time.time;
                    MenuModel.SetActive(false);
                    ScreenCover.GetComponent<SpriteRenderer>().color = Color.clear;

                    foreach (GameObject obj in UISet_Play)
                    {
                        obj.SetActive(true);
                    }

                    switch (UseSkin.bodyType)
                    {
                        case SkinData.BodyType.Cube: PlayerObjectSet = PlayerObjectSet_Cube; break;
                        case SkinData.BodyType.Ball: PlayerObjectSet = PlayerObjectSet_Ball; break;
                    }
                    PlayerObjectSet.SetActive(true);
                    Player = PlayerObjectSet.transform.Find("Player").gameObject;
                    Player.transform.Find("Skin Default").GetComponent<SpriteRenderer>().color = UseSkin.color;
                    Player.transform.Find("Skin Attack").GetComponent<SpriteRenderer>().color = UseSkin.color;
                    Player.GetComponent<Collider2D>().enabled = true;
                    foreach (Transform obj in PlayerObjectSet.transform)
                    {
                        obj.localPosition = Vector3.up * 15;
                    }
                    ObstacleCreate(0, 0, 30, 1, 0);
                    score = 0;
                    scoreText.text = "" + (StartLevel-1) * 3000;
                    levelText.color = Color.clear;
                }

                scoreboardRectTransform.anchoredPosition -= scoreboardPosition_Menu * Time.deltaTime * 1.6f;
                pauseButtonRectTransform.anchoredPosition -= pauseButtonPosition_Menu * Time.deltaTime * 3.2f;

                if (Time.time - stateEnterTime > 0.6) ChangeGameState(GameState.Play);
                break;
            #endregion



            #region プレイ画面
            case GameState.Play:
                #region プレイ初期設定
                if (gameStateEnter)
                {
                    gameStateEnter = false;

                    foreach (GameObject obj in obstacles)
                    {
                        obj.GetComponent<Rigidbody2D>().velocity = Vector3.left * (5 + (float)Math.Pow(StartLevel, 0.7f) * 3);
                    }
                    scoreboardRectTransform.anchoredPosition = Vector2.zero;
                    pauseButtonRectTransform.anchoredPosition = -pauseButtonPosition_Menu;
                    level = StartLevel - 1;
                    playStartTime = Time.time - (StartLevel -1) * LevelUpScore/100;
                    levelText.text = "Lv." + StartLevel;
                    levelRectTransform.anchoredPosition = centerPosition_top_left;
                    levelText.fontSize = 150;

                    ChangePlayState(PlayState.LevelUp);
                }
                #endregion

                switch (currentPlayState)
                {
                    #region プレイ中処理
                    case PlayState.Play:
                        score = (int)((Time.time - playStartTime) * 100);
                        moveSpeed = 5 + (float)Math.Pow(level, 0.7f) * 3;

                        RandomObstacleCreate(moveSpeed);

                        if (Player.GetComponent<PlayerScript>().currentPlayerState == PlayerScript.PlayerState.GameOver) 
                            ChangePlayState(PlayState.GameOver);
                        else if (UISet_Play[3].GetComponent<ButtonScript_Quick>().isButtonPushed)
                            ChangePlayState(PlayState.Pause);
                        else if (score % LevelUpScore > ((LevelUpScore - 300) - 2400/moveSpeed)) ChangePlayState(PlayState.LevelUp);
                        break;
                    #endregion

                    #region レベル上昇処理
                    case PlayState.LevelUp:
                        if (playStateEnter)
                        {
                            playStateEnter = false;
                            scoreCorrection = level * LevelUpScore;
                        }

                        switch(score - scoreCorrection)
                        {
                            case < -200 :
                                break;
                            case < -150:
                                levelRectTransform.anchoredPosition += (centerPosition_top_left - levelTextPosition_Play) * Time.deltaTime * 2;
                                levelText.fontSize = (score - scoreCorrection) * 2 + 450;
                                break;
                            case < -100:
                                levelRectTransform.anchoredPosition = centerPosition_top_left;
                                levelText.fontSize = 150;
                                break;
                            case < -50:
                                levelText.color -= colorChange * Time.deltaTime * 2;
                                break;
                            case < 0:
                                levelText.color = Color.clear;
                                break;
                            case < 50:
                                if (score > level * LevelUpScore)
                                {
                                    level++;
                                    levelText.text = "Lv." + level;
                                }
                                levelText.color += colorChange * Time.deltaTime * 2;
                                break;
                            case < 150:
                                levelText.color = Color.black;
                                break;
                            case < 200:
                                levelRectTransform.anchoredPosition += (levelTextPosition_Play - centerPosition_top_left) * Time.deltaTime * 2;
                                levelText.fontSize = 450 - (score - scoreCorrection) * 2;
                                break;
                            case < 300:
                                levelRectTransform.anchoredPosition = levelTextPosition_Play;
                                levelText.fontSize = 50;
                                break;
                            default : ChangePlayState(PlayState.Play); break;
                        }
                        

                        score = (int)((Time.time - playStartTime) * 100);

                        stageRightEdge = previousObstacle.transform.position.x + previousObstacle.transform.localScale.x;
                        if (stageRightEdge < 25)
                        {
                            moveSpeed = 5 + (float)Math.Pow(level, 0.7f) * 3;
                            ObstacleCreate(0, stageRightEdge, 3, 1, moveSpeed);
                        }

                        if (Player.GetComponent<PlayerScript>().currentPlayerState == PlayerScript.PlayerState.GameOver)
                            ChangePlayState(PlayState.GameOver);
                        else if (UISet_Play[3].GetComponent<ButtonScript_Quick>().isButtonPushed)
                            ChangePlayState(PlayState.Pause);
                        break;
                    #endregion

                    #region ポーズ画面
                    case PlayState.Pause:
                        if (playStateEnter)
                        {
                            playStateEnter = false;
                            Time.timeScale = 0;
                            PauseUI.SetActive(true);
                            ScreenCover.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                            foreach (GameObject button in PauseButtons)
                                button.GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                        }

                        #region コンティニュー処理
                        if (PauseButtons[0].GetComponent<ButtonScript_Normal>().isButtonPushed)
                        {
                            switch (countinueCountDown)
                            {
                                case > 3:
                                    PauseUI.SetActive(false);
                                    ScreenCover.GetComponent<SpriteRenderer>().color = Color.clear;
                                    CountCircle.SetActive(true);
                                    Application.targetFrameRate = 30;
                                    countinueCountDown = 3;
                                    CountinueCountText.enabled = true;
                                    CountinueCountText.text = "" + countinueCountDown;
                                    break;

                                case > 0:
                                    if (countinueCounterRectNum == 0)
                                    {
                                        countinueCounterRectNum = 30;
                                        countinueCountDown--;
                                        CountinueCountText.text = "" + countinueCountDown;
                                        foreach (GameObject countRect in CounterCircleRects)
                                            countRect.SetActive(true);
                                        if (countinueCountDown == 0)
                                        {
                                            CountCircle.SetActive(false);
                                            CountinueCountText.enabled = false;
                                        }
                                    }
                                    else CounterCircleRects[--countinueCounterRectNum].SetActive(false);
                                    break;

                                case 0:
                                    PauseButtons[0].GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                                    Time.timeScale = 1;
                                    Application.targetFrameRate = 60;
                                    countinueCountDown = 4;
                                    UISet_Play[3].GetComponent<ButtonScript_Quick>().isButtonPushed = false;
                                    ChangePlayState(previousPlayState);
                                    break;
                            }
                        }
                        #endregion

                        #region リタイア処理
                        else if (PauseButtons[1].GetComponent<ButtonScript_Normal>().isButtonPushed)
                        {
                            PauseButtons[1].GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                            Time.timeScale = 1;
                            connectedMode = GameState.Play;
                            UISet_Play[3].GetComponent<ButtonScript_Quick>().isButtonPushed = false;
                            ChangeGameState(GameState.EnterMenu);
                        }
                        #endregion

                        break;
                    #endregion

                    #region ゲームオーバー処理
                    case PlayState.GameOver:
                        if (playStateEnter)
                        {
                            playStateEnter = false;

                            foreach (GameObject obj in obstacles)
                            {
                                obj.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                            }
                            if (Player.GetComponent<PlayerScript>().previousPlayerState != PlayerScript.PlayerState.Squat ||
                                ! Player.GetComponent<PlayerScript>().isInTunnel)
                            {
                                Player.layer = 6;
                                Rigidbody2D playerRb = Player.GetComponent<Rigidbody2D>();
                                playerRb.constraints = RigidbodyConstraints2D.None;
                                playerRb.angularVelocity = moveSpeed * 20;
                                playerRb.velocity = Vector2.up * 15 + Vector2.left * moveSpeed * 0.3f;
                            }
                        }
                        break;
                        #endregion
                }

                DeleteObstacle();

                #region スコア表示処理
                scoreText.text = "" + score;
                #endregion
                break;
            #endregion



            #region スキンセレクト
            case GameState.SkinSelect:
                if (gameStateEnter)
                {
                    gameStateEnter = false;
                    HingeUnder.transform.eulerAngles = Vector3.forward * 180 + Vector3.up * 90;
                    HingeUp.transform.eulerAngles = Vector3.forward * 180 + Vector3.up * 90;
                    MenuModel.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 162;
                    MenuModel_Cube.transform.position = MenuModel.transform.position;
                    MenuModel_Ball.transform.position = MenuModel.transform.position;
                    MenuModel_Cube.transform.localScale = MenuModel.transform.localScale;
                    MenuModel_Ball.transform.localScale = MenuModel.transform.localScale;
                    modelRb.velocity = Vector2.zero;
                    previewSkin = UseSkin;
                    useSkinNumber = SkinSelecter.GetComponent<SkinSelecterScript>().skinNumber;
                    selectorEulerAngles = SkinSelecterWheel.transform.localEulerAngles;
                    foreach(GameObject button in SkinSelectButtons)
                        button.GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                }

                skinNumber = SkinSelecter.GetComponent<SkinSelecterScript>().skinNumber;
                 if (SkinList[skinNumber] != previewSkin)
                {
                    if (SkinList[skinNumber].bodyType != previewSkin.bodyType)
                    {
                        switch (SkinList[skinNumber].bodyType)
                        {
                            case SkinData.BodyType.Cube: MenuModel = MenuModel_Cube; break;
                            case SkinData.BodyType.Ball: MenuModel = MenuModel_Ball; break;
                        }
                        MenuModel_Cube.SetActive(MenuModel == MenuModel_Cube);
                        MenuModel_Ball.SetActive(MenuModel == MenuModel_Ball);
                    }
                    modelRb = MenuModel.GetComponent<Rigidbody2D>();
                    previewSkin = SkinList[skinNumber];
                    SkinName.text = previewSkin.name;
                    MenuModel.transform.Find("Menu Model Skin").GetComponent<SpriteRenderer>().color = previewSkin.color;
                }

                if (SkinSelectButtons[0].GetComponent<ButtonScript_Normal>().isButtonPushed)
                {
                    SkinSelectButtons[0].GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                    isUseSkin = true;
                    UseSkin = previewSkin;
                    connectedMode = GameState.SkinSelect;
                    ChangeGameState(GameState.EnterMenu);
                    break;
                }
                else if (SkinSelectButtons[1].GetComponent<ButtonScript_Normal>().isButtonPushed)
                {
                    SkinSelectButtons[1].GetComponent<ButtonScript_Normal>().isButtonPushed = false;
                    isUseSkin = false;
                    connectedMode = GameState.SkinSelect;
                    ChangeGameState(GameState.EnterMenu);
                    break;
                }

                RandomObstacleCreate(8);
                DeleteObstacle();

                break;
                #endregion
        }

        // リセット（開発用）
        {
            if (Input.GetKeyDown(KeyCode.Space)) ResetGame();
        }
    }


    #region オブジェクト生成関数
    private GameObject ObstacleCreate(int obstacleNum, float positionX, float width, float height, float moveSpeed)
    {
        createObjectNumber = obstacleNum;
         GameObject _previousObstacle = Pool[obstacleNum].Get();
        _previousObstacle.transform.position = Vector3.right * positionX;
        _previousObstacle.transform.localScale = new Vector3(width, height, 1);
        _previousObstacle.GetComponent<Rigidbody2D>().velocity = Vector3.left * moveSpeed;
        obstacles.Add(_previousObstacle); 
        obstacleNumbers.Add(obstacleNum);
        previousObstacle = _previousObstacle;
        stageRightEdge = previousObstacle.transform.position.x + previousObstacle.transform.localScale.x;
        return _previousObstacle;
    }
    #endregion


    #region 障害物ランダム生成関数
    private void RandomObstacleCreate(float moveSpeed)
    {
        stageRightEdge = previousObstacle.transform.position.x + previousObstacle.transform.localScale.x;
        if (stageRightEdge < 25)
        {
            do
            {
                chooseObstacle = UnityEngine.Random.Range(1, ObstacleSet.Length);
            } while (chooseObstacle == previousObstacleNum1 || chooseObstacle == previousObstacleNum2);
            previousObstacleNum2 = previousObstacleNum1;
            previousObstacleNum1 = chooseObstacle;
            switch (chooseObstacle)
            {
                case 1:
                    obstacleWidth = UnityEngine.Random.Range(level * 0.5f + 2.5f, moveSpeed * 0.5f);
                    ObstacleCreate(chooseObstacle, stageRightEdge, obstacleWidth, 1, moveSpeed);
                    break;
                case 2:
                case 3:
                    obstacleWidth = UnityEngine.Random.Range(level * 0.2f + 2, level * 0.2f + 6);
                    ObstacleCreate(chooseObstacle, stageRightEdge, obstacleWidth, 1, moveSpeed);
                    break;
                case 4:
                    obstacleWidth = UnityEngine.Random.Range(level * 0.2f + 4.5f, level * 0.2f + 6.5f);
                    obstacleHeight = UnityEngine.Random.Range(1.0f, 4.0f);
                    ObstacleCreate(chooseObstacle, stageRightEdge, obstacleWidth, obstacleHeight, moveSpeed);
                    obstacleWidth = UnityEngine.Random.Range(level * 0.2f + 3.0f, level * 0.2f + 6.0f);
                    obstacleHeight = UnityEngine.Random.Range(2.0f, 4.0f);
                    obstacleHeight += (obstacleHeight > previousObstacle.transform.localScale.y) ? 1 : -1;
                    ObstacleCreate(chooseObstacle, stageRightEdge, obstacleWidth, obstacleHeight, moveSpeed);
                    break;
                case 5:
                    int number = UnityEngine.Random.Range(1, 6);
                    for (int i = 0; i < number; i++)
                    {
                        ObstacleCreate(chooseObstacle, stageRightEdge, 1, 1, moveSpeed);
                    }
                    break;
            }
            obstacleWidth = UnityEngine.Random.Range(level * 0.2f + 6.5f, level * 0.2f + 7.5f);
            ObstacleCreate(0, stageRightEdge, obstacleWidth, 1, moveSpeed);
        }
    }
    #endregion


    #region オブジェクト削除関数
    private void DeleteObstacle()
    {
        if (obstacles.Count > 0)
        {
            if (obstacles[0].transform.position.x + obstacles[0].transform.localScale.x < -5)
            {
                GameObject deleteObstacle = obstacles[0];
                int deleteNumber = obstacleNumbers[0];
                obstacles.Remove(deleteObstacle);
                obstacleNumbers.Remove(deleteNumber);
                Pool[deleteNumber].Release(deleteObstacle);
            }
        }
    }
    #endregion

    #region リセット関数（開発用）
    private void ResetGame()
    {
        do
        {
            GameObject deleteObject = obstacles[0];
            int deleteNumber = obstacleNumbers[0];
            obstacles.Remove(deleteObject);
            obstacleNumbers.Remove(deleteNumber);
            Pool[deleteNumber].Release(deleteObject);
        } while (obstacles.Count > 0);
        if(PlayerObjectSet != null)
            PlayerObjectSet.SetActive(false);
        foreach (GameObject obj in UISet_Play)
        {
            obj.SetActive(false);
        }
        PauseUI.SetActive(false);
        Time.timeScale = 1;
        UISet_Play[3].GetComponent<ButtonScript_Quick>().isButtonPushed = false;
        MenuModel.GetComponent<ModelScript>().moveStartTiming = Time.time + 5;
        MenuModelEyeScript.closeTiming = Time.time + 5;
        ChangeGameState(GameState.Menu);
    }
    #endregion
}
