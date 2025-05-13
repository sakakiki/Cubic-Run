using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

//GameManagerは処理を優先実行
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    //ゲームのバージョン
    public const int gameVersion = 5;


    //自身のインスタンス
    public static GameManager Instance;

    //Manager系
    private TerrainManager TM;
    private FirestoreManager FSM;
    private SkinDataBase SDB;

    [Header("情報のキャッシュ（Firebase保存データ）")]
    public string playerName = "Noname";
    public int totalExp = 0;
    public int playerScore = 0;
    public List<int> trainingClearCounts = new List<int>();
    public int usingSkinID = 0;
    public int totalRunDistance = 0;
    [Space(30)]

    [Header("情報のキャッシュ（Firebase上とローカルの両方に保存）")]
    public int highScore = 0;
    [Space(30)]

    [Header("情報のキャッシュ（ローカル保存データ）")]
    public bool isUnsavedHighScore;
    public Queue<int> rankingScoreQueue = new Queue<int>();   //playerScoreに未反映のランキングモードのスコア
    public bool isTraining { get; private set; }
    public int trainingLevel;
    [Space(30)]

    [Header("プレイ中のデータを保持")]
    public int score;
    public int level;
    public int levelUpSpan {  get; private set; }
    public int playerRank = 0;  //totalExpから算出
    public int requiredExp;
    public int highestTrainingLevel { get; private set; } = 1;    // = trainingClearCounts.Count;
    public bool[] isSkinUnlocked { get; private set; } = new bool[16];    //totalExpとhighestTrainingLevelから算出
    public int previousSkinID;
    public Color panelSelectedColor;
    public Vector2 centerPos_PlayerArea;
    public Queue<int> newSkinQueue = new Queue<int>();
    public Color staminaColor { private set; get; }
    public int adScore = 0;
    public int adCount = 0;
    private bool isSetupUI = false;
    [Space(30)]

    [Header("インスペクターから設定")]
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
    public Transform scoreMarkerTf_Result_Ranking;
    public Transform scoreMarkerTf_Result_Trainig;
    public SpriteRenderer screenCover;
    public SpriteRenderer scoreGageSprite;
    public GameObject pauseUI;
    public TextMeshProUGUI countinueCountText;
    public GameObject[] countinueCircleSquares;
    public TextMeshProUGUI playButtonText;
    public TextMeshProUGUI retryButtonText;
    public GameObject resultRankingUI;
    public GameObject resultTrainingUI;
    public GameObject resultTutorialUI;
    public RectTransform resultButtonRtf_Title;
    public GameObject resultButton_Retry;
    public TextMeshProUGUI clearRateText;
    public TextMeshProUGUI clearTimesNumText;
    public BoxCollider2D resultWallCol;
    [SerializeField] private Transform content_LevelSelecter;
    [SerializeField] private GameObject levelPanelPrefab;
    [HideInInspector] public List<Button_LevelSelector> button_LevelSelecters = new List<Button_LevelSelector>();
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
    public GameObject unlockPlay;
    public RectTransform[] unlockPlayModeRtf;
    public TextMeshProUGUI unlockPlayMessage;
    public Sprite squarSprite;
    public Sprite cicleSprite;
    public SpriteRenderer frontScreenCover;
    public GameObject optionUIBase;
    public TextMeshProUGUI optionTitle;
    public GameObject optionUI_Account;
    public TextMeshProUGUI optionUI_Account_CurrentAccount;
    public TextMeshProUGUI optionUI_Account_Status;
    public GameObject optionUI_Account_Anonymous;
    public GameObject optionUI_Account_Email;
    public GameObject optionUI_Account_SendEmail;
    public GameObject optionUI_Account_DeleteData;
    public GameObject optionUI_Account_Relogin;
    public GameObject optionUI_Volume;
    public GameObject optionUI_Button;
    public GameObject optionUI_Credit;
    public RankingBoard highScoreRankingBoard;
    public RankingBoard playerScoreRankingBoard;
    public ResultRankingBoard resultRankingBoard;
    public TextMeshProUGUI scoreFluctuation_highScore;
    public TextMeshProUGUI scoreFluctuation_playerScore;
    public TextMeshProUGUI resultHighScoreText;
    public TextMeshProUGUI resultPlayerScoreText;
    private Queue<GameObject> trainingPanels = new Queue<GameObject>();
    [SerializeField] private Image volumeFillArea_BGM;
    [SerializeField] private Image volumeFillArea_SE;
    [SerializeField] private TextMeshProUGUI playerInfo_PlayerName;
    [SerializeField] private TextMeshProUGUI playerInfo_PlayerRank;
    [SerializeField] private RectTransform playerInfo_ExpScaleRtf;
    [SerializeField] private SpriteRenderer playerInfo_ExpScaleSprite;
    [SerializeField] private TextMeshProUGUI playerInfo_TotalExp;
    [SerializeField] private TextMeshProUGUI playerInfo_RequiredExp;
    [SerializeField] private TextMeshProUGUI playerInfo_TotalRunDistance;
    [SerializeField] private SpriteRenderer buttonPatternSelectSquare;
    [SerializeField] private GameObject playCost;
    public SpriteRenderer[] staminaSprite;
    [SerializeField] private TextMeshProUGUI overStamina;
    public RectTransform[] staminaRtf;
    public GameObject loadingCube;
    public Transform loadingCubeTf;

    //ステートマシン
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //定数登録・記憶
    public int defaultFrameRate { get; private set; } = 120;
    public Vector2 centerPos_World { get; private set; } = new Vector2(5, 3);
    public Vector2 centerPos_PlayerArea_Result { get; private set; } = new Vector2(-1, 3);
    public Color screenCoverColor_Menu { get; private set; } = Color.white - Color.black * 0.2f;
    public Color screenCoverColor_Play { get; private set; } = Color.clear;
    public Vector2 resultButtonAPos_Title_Single { get; private set; } = new Vector2(0, 200);
    public Vector2 resultButtonAPos_Title_Double { get; private set; } = new Vector2(-400, 200);



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        //目標フレームレート設定
        Application.targetFrameRate = defaultFrameRate;
    }



    private void Start()
    {
        //Managerのインスタンス格納
        TM = TerrainManager.Instance;
        FSM = FirestoreManager.Instance;
        SDB = SkinDataBase.Instance;

        //ステートマシンの作成・格納
        gameStateMachine = new GameStateStateMachine();
        gameStateMachine.Initialize(gameStateMachine.state_Login);

        //プレイヤー移動可能エリアの中心の変更
        centerPos_PlayerArea = centerPos_World;
    }



    private void Update()
    {
        //ステートマシンのUpdateを実行
        gameStateMachine.Update(Time.deltaTime);
    }



    //データのロードとゲームへの反映
    public async Task GameInitialize()
    {
        if (!isSetupUI) 
        {
            #region レターボックス
            // 基準（縦方向）サイズと基準アスペクト比から基準横方向サイズを算出
            var baseHorizontalSize = 5.622821f * 2560 / 1440;
            // 基準横方向サイズと対象アスペクト比で対象縦方向サイズを算出
            var verticalSize = baseHorizontalSize / Camera.main.aspect;
            Camera.main.orthographicSize = verticalSize;
            #endregion

            //UIをHingeに接続
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
            for (int i = 0; i < resultUIs_B.Length; i++)
                resultUIs_B[i].SetParent(resultHingeRtf_B);
        }

        //変数・オブジェクト・設定の初期化
        SetTrainingMode(false);
        trainingClearCounts.Clear();
        button_LevelSelecters.Clear();
        while (trainingPanels.Count > 0)
            Destroy(trainingPanels.Dequeue());


        //ローカルデータのロード
        highScore = HighScoreManager.Load();
        rankingScoreQueue = RankingScoreManager.Load();
        isUnsavedHighScore = UnsavedHighScoreFlagManager.Load();
        AudioManager.Instance.SetVolumeBGM(VolumeBGMManager.Load());
        AudioManager.Instance.SetVolumeSE(VolumeSEManager.Load());
        InputManager.Instance.playButtonPatternNum = PlayerPrefs.GetInt("ButtonPattern", 0);
        for (int i = 0; i < InputManager.Instance.actionAllocation.Length; i++)
            InputManager.Instance.actionAllocation[i] = PlayerPrefs.GetInt("ActionAllocation_" + i, (4-i)%3);
        InputManager.Instance.BindEvent();

        //クラウドに未反映のデータがあれば反映
        if (isUnsavedHighScore)
            await FSM.SaveHighScore(highScore);
        if (rankingScoreQueue.Count > 0)
            await FSM.SavePlayerScore();


        bool onlineSuccess = true;

        //クラウドデータのロード
        onlineSuccess &= await FSM.LoadAll();


        //プレイヤーランク算出
        playerRank = CalculatePlayerRank(totalExp);

        //次のランクまでに必要な経験値量を算出
        requiredExp = (playerRank + 1) * (playerRank + 2) / 2 * 100 - totalExp;


        //未クリアのトレーニングモードのレベルのCountを確保
        if (trainingClearCounts[trainingClearCounts.Count - 1] > 0)
            trainingClearCounts.Add(0);

        //トレーニングモード到達レベルを算出
        highestTrainingLevel = trainingClearCounts.Count;
        trainingLevel = highestTrainingLevel;

        //到達レベルまでのトレーニングモードボタンを配置
        for (int i = 1; i <= highestTrainingLevel; i++)
            AddLevelPanel(i);

        //クリア回数表示を更新
        UpdatePanelCount();


        //スキンを全てロック
        for (int i = 0; i < isSkinUnlocked.Length; i++)
            isSkinUnlocked[i] = false;

        //スキンパネルの生成
        skinSelecter.CreateSkinPanel();

        //スキンのロック解除
        CheckSkinUnlock();

        //表示スキンとスキンセレクター回転量の変更
        ChangePlayerSkin(usingSkinID);
        skinSelecter.SetWheelAngle(usingSkinID);


        //プレイヤー情報の更新
        UpdatePlayerInfo();


        //チュートリアル未クリアならプレイボタンをロック
        if (playerRank == 0)
            InputManager.Instance.SetPlayButtonLock(true);
        //チュートリアルクリア済みならプレイボタンをアンロック
        else
            InputManager.Instance.SetPlayButtonLock(false);


        //ランキング更新
        onlineSuccess &= await RankingManager.UpdateRanking(RankingManager.RankingType.HighScore);
        onlineSuccess &= await RankingManager.UpdateRanking(RankingManager.RankingType.PlayerScore);
        highScoreRankingBoard.UpdateRankingDisplay();


        //スタミナの反映
        onlineSuccess &= UpdateStamina(await FSM.CheckResetAndGetStamina());


        //オンラインデータ取得が失敗すれば通知
        if (!onlineSuccess)
            PopupUIManager.Instance.SetupMessageBand("クラウドデータを取得できませんでした。", 2);
    }



    //LevelSelecterのパネルを追加
    public void AddLevelPanel(int level)
    {
        //パネルのインスタンスを生成
        RectTransform newPanelRtf = Instantiate(levelPanelPrefab).GetComponent<RectTransform>();

        //スクロールビューの要素に
        newPanelRtf.SetParent(content_LevelSelecter);
        newPanelRtf.localScale = Vector3.one;
        newPanelRtf.anchoredPosition3D = newPanelRtf.anchoredPosition3D - Vector3.forward * newPanelRtf.anchoredPosition3D.z;
        newPanelRtf.localEulerAngles = Vector3.zero;

        //入力スクリプトを管理可能に
        Button_LevelSelector button_LevelSelecter = newPanelRtf.GetComponent<Button_LevelSelector>();
        button_LevelSelecter.SetLevel(level);
        button_LevelSelecters.Add(button_LevelSelecter);

        //パネルオブジェクトを管理可能に
        trainingPanels.Enqueue(newPanelRtf.gameObject);
    }



    //ランキングモード - トレーニングモード切り替え
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
            button_LevelSelecters[trainingLevel - 1].PushButton();

            //レベル上昇間隔を変更
            levelUpSpan = 5000;

            //背景の速度を変更
            TM.SetSpeed(5 + Mathf.Pow(trainingLevel, 0.7f) * 3);
            TM.moveSpeed = 5 + Mathf.Pow(trainingLevel, 0.7f) * 3;

            //スタミナ消費表示を非表示
            playCost.SetActive(false);
        }

        //ランキングモードへ遷移時
        else
        {
            //ボタンのテキストを変更
            playButtonText.SetText("プレイ");

            //レベル上昇間隔を変更
            levelUpSpan = 2000;

            //背景の速度を変更
            TM.SetSpeed(8);
            TM.moveSpeed = 8;

            //スタミナ消費表示を表示
            playCost.SetActive(true);
        }
    }



    //トレーニングモードのプレイレベルを設定
    public void SetTrainingLevel(int level)
    {
        //現在の選択を解除
        button_LevelSelecters[trainingLevel - 1].Initialize();

        //トレーニングのレベルを更新
        trainingLevel = level;
        playButtonText.SetText("プレイ - Lv." + level);

        //背景の速度を変更
        TM.SetSpeed(5 + Mathf.Pow(level, 0.7f) * 3);
        TM.moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;
    }



    //トレーニングモードのレベルを追加
    public void AddTrainingLevel()
    {
        //クリア回数保存変数の追加
        trainingClearCounts.Add(0);

        //最高到達レベルを更新
        highestTrainingLevel++;

        //トレーニングモードレベル選択パネルを追加
        AddLevelPanel(highestTrainingLevel);

        //トレーニングモードの選択レベルを更新
        SetTrainingLevel(highestTrainingLevel);
        button_LevelSelecters[highestTrainingLevel - 1].PushButton();
    }



    //クリア回数の加算
    public async void AddClearCount(int level)
    {
        //クリア回数の加算
        trainingClearCounts[level - 1]++;

        //パネルに表示されている回数の加算
        button_LevelSelecters[level - 1].clearTimesNumTMP.SetText(trainingClearCounts[level - 1] + "回");

        // Firestoreに即時反映
        await FSM.SaveTrainingClearCount(level);
    }



    //スキンを変更
    public void ChangePlayerSkin(int skinID)
    {
        //GameManager側のデータを更新
        usingSkinID = skinID;

        //プレビューのモデルを隠さない
        skinModelCover.SetActive(false);

        //スキンの変更
        playerCon.ChangeSkin(skinID);

        //表示スキン名の変更
        skinNameText.SetText(SDB.skinData[skinID].name);

        //UIの色を変更
        ChangeUIColor(skinID);

        //Crystalスキンロック時の例外処理
        if (!isSkinUnlocked[skinID])
            if (skinID % 8 == 7)
            {
                //プレビューのモデルを隠す
                skinModelCover.SetActive(true);

                //名前を表示しない
                skinNameText.SetText("？？？");
            }
    }



    //UIの色のみを変更
    public void ChangeUIColor(int skinID)
    {
        //パネル選択時の色を変更
        panelSelectedColor = SDB.skinData[skinID].UIColor;

        //スコアゲージの色を変更
        scoreGageSprite.color = SDB.skinData[skinID].UIColor - Color.black * 0.4f;

        //経験値バー色の変更
        expSprite.color = Color.Lerp(SDB.skinData[skinID].UIColor, Color.gray + Color.white * 0.3f, 0.3f);
        playerInfo_ExpScaleSprite.color =
            Color.Lerp(SDB.skinData[skinID].UIColor, Color.gray + Color.white * 0.3f, 0.3f);

        //ボタンの色の更新
        button_LevelSelecters[trainingLevel - 1].PushButton();

        //音量バーの色の変更
        volumeFillArea_BGM.color = Color.Lerp(SDB.skinData[skinID].UIColor, Color.gray + Color.white * 0.5f, 0.3f);
        volumeFillArea_SE.color = Color.Lerp(SDB.skinData[skinID].UIColor, Color.gray + Color.white * 0.5f, 0.3f);

        //ボタン配置UIの色変更
        buttonPatternSelectSquare.color = SDB.skinData[skinID].UIColor;

        //スタミナの色を変更
        staminaColor = Color.Lerp(SDB.skinData[skinID].UIColor, Color.gray, 0.2f);
        for (int i = 0; i < staminaSprite.Length; i++)
            if(staminaSprite[i].color != Color.clear)
                staminaSprite[i].color = staminaColor;
    }



    //スキンのアンロック
    public void UnlockSkin(int skinID)
    {
        //既にロック解除済みなら何もしない
        if (isSkinUnlocked[skinID]) return;

        //アンロックを保存
        isSkinUnlocked[skinID] = true;

        //スキンパネルのアンロック
        skinSelecter.UnlockPanel(skinID);
    }



    //スキンのロック解除条件を生成
    public string GetUnlockSkinCondition(int skinID)
    {
        string condition = "アンロック条件：\n";

        if (skinID < 8)
        {
            condition += "プレイヤーランク";
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
            condition += "　到達";
        }
        else
        {
            condition += "トレーニングモードLv.";
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
            condition += "　クリア";
        }

        return condition;
    }



    //総獲得経験値からプレイヤーランクを算出
    public int CalculatePlayerRank(int totalExp)
    {
        int playerRank = 0;

        while (totalExp >= (playerRank + 1) * 100)
        {
            playerRank++;
            totalExp -= playerRank * 100;
        }

        return playerRank;
    }



    //スキンアンロック状態のチェック
    public void CheckSkinUnlock()
    {
        //Cubeスキンのチェック
        if (playerRank >= 0) UnlockSkin(0);
        if (playerRank >= 1) UnlockSkin(1);
        if (playerRank >= 10) UnlockSkin(2);
        if (playerRank >= 20) UnlockSkin(3);
        if (playerRank >= 30) UnlockSkin(4);
        if (playerRank >= 40) UnlockSkin(5);
        if (playerRank >= 50) UnlockSkin(6);
        if (playerRank >= 100) UnlockSkin(7);

        //Sphereスキンのチェック
        if (highestTrainingLevel > 1) UnlockSkin(8);
        if (highestTrainingLevel > 2) UnlockSkin(9);
        if (highestTrainingLevel > 3) UnlockSkin(10);
        if (highestTrainingLevel > 5) UnlockSkin(11);
        if (highestTrainingLevel > 7) UnlockSkin(12);
        if (highestTrainingLevel > 10) UnlockSkin(13);
        if (highestTrainingLevel > 15) UnlockSkin(14);
        if (highestTrainingLevel > 20) UnlockSkin(15);
    }



    //トレーニングモードパネルのクリア回数の更新
    public void UpdatePanelCount()
    {
        for (int levelIndex = 0; levelIndex < highestTrainingLevel; levelIndex++)
            button_LevelSelecters[levelIndex].clearTimesNumTMP.SetText(trainingClearCounts[levelIndex] + "回");
    }



    //プレイ結果のセーブ
    public async Task SaveResult()
    {
        // 広告再生に関する数値を保存
        adScore += score;
        adCount++;

        #region 走行距離の加算
        int addDistance = 0;
        //ランキングモードの場合はLV.1からの距離も加算
        if (!isTraining)
            for (int calcLevel = 1; calcLevel < level; calcLevel++)
                addDistance += (int)((5 + Mathf.Pow(calcLevel, 0.7f) * 3) * 20);

        //最終レベルの走行距離を加算
        addDistance += (int)((5 + Mathf.Pow(level, 0.7f) * 3) * (isTraining ? score / 100 : (score - (level - 1) * 2000) / 100));

        //ローカルに反映
        totalRunDistance += addDistance;

        //クラウドには経験値と一緒に保存
        #endregion

        #region 経験値の加算
        //ゲームモードに応じた加算量算出
        int addExp = isTraining ? addDistance / 10 : addDistance;

        //トレーニングモード完走で完走ボーナス加算
        if (score == 5000 && isTraining)
            addExp += level * 50;

        //ローカルに反映
        totalExp += addExp;

        //プレイヤーランクを更新
        requiredExp -= addExp;
        while (requiredExp <= 0)
        {
            playerRank++;
            requiredExp += (playerRank + 1) * 100;
        }

        //クラウドに保存
        FSM.SaveExperienceAndRunDistance(addExp, addDistance);
        #endregion

        //プレイヤー情報の更新
        UpdatePlayerInfo();

        //トレーニングモードなら処理終了
        if (isTraining) return;

        #region ハイスコアの保存
        //ハイスコアを更新していれば
        if (score > highScore)
        {
            //ハイスコアの更新
            highScore = score;

            //ローカルに保存
            HighScoreManager.Save(highScore);

            //クラウドに保存（失敗すればフラグが立つ）
            await FSM.SaveHighScore(highScore);
        }
        #endregion

        #region プレイヤースコアの保存
        //Queueにスコアを格納
        rankingScoreQueue.Enqueue(score);

        //クラウドに保存
        //ローカルへの保存はメソッド内
        await FSM.SavePlayerScore();
        #endregion
    }
    public void SaveResult(int getExp)
    {
        #region 経験値の加算
        //ローカルに反映
        totalExp += getExp;

        //プレイヤーランクを更新
        requiredExp -= getExp;
        while (requiredExp <= 0)
        {
            playerRank++;
            requiredExp += (playerRank + 1) * 100;
        }

        //クラウドに保存
        FSM.SaveExperienceAndRunDistance(getExp, 0);
        #endregion

        //プレイヤー情報の更新
        UpdatePlayerInfo();
    }



    //ローカルデータを反映させたプレイヤースコアを取得
    public int GetPlayerScore()
    {
        int playerScore = this.playerScore;

        // 未反映のデータを反映
        foreach (int resultScore in rankingScoreQueue)
        {
            playerScore += (resultScore - playerScore) / 10;
        }

        return playerScore;
    }



    //プレイヤー情報の更新
    public void UpdatePlayerInfo()
    {
        playerInfo_PlayerName.SetText(playerName);
        playerInfo_PlayerRank.SetText("プレイヤーランク　" + playerRank);
        playerInfo_ExpScaleRtf.localScale = Vector3.one - Vector3.right * (requiredExp / (float)((playerRank + 1) * 100));
        playerInfo_TotalExp.SetText(totalExp + "Exp");
        playerInfo_RequiredExp.SetText(requiredExp.ToString());
        playerInfo_TotalRunDistance.SetText("走った距離：" + totalRunDistance.ToString("N0") + "m");
    }



    //アプリ中断時の処理
    public void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            if (gameStateMachine.currentState == gameStateMachine.state_Play)
                GameStateState_Play.EnterPause();
        }
    }



    //スタミナ表示の更新
    public bool UpdateStamina(int stamina)
    {
        for (int i = 1; i < staminaSprite.Length; i++)
        {
            if (i <= stamina)
                staminaSprite[i].color = staminaColor;
            else
                staminaSprite[i].color = Color.clear;
        }

        UpdateOverStamina(stamina);

        return stamina >= 0;
    }



    //最大値超過スタミナ表示の更新
    public void UpdateOverStamina(int stamina)
    {
        if (stamina > 3)
            overStamina.SetText("＋" + (stamina - 3));
        else
            overStamina.SetText("");
    }
}
