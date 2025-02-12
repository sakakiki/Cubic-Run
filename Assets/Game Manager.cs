using TMPro;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.InputManagerEntry;


public class GameManager : MonoBehaviour
{
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
    public int playerRank { get; private set; } = 0;  //totalExpから算出
    public int highestTrainingLevel { get; private set; } = 1;    // = trainingClearCounts.Count;
    public bool[] isSkinUnlocked { get; private set; } = new bool[16];    //totalExpとhighestTrainingLevelから算出
    public int previousSkinID;
    public Color panelSelectedColor;
    public Vector2 centerPos_PlayerArea;
    [Space(30)]

    [Header("インスペクターから設定")]
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

    //ステートマシン
    public GameStateStateMachine gameStateMachine {  get; private set; }

    //定数登録・記憶
    public int defaultFrameRate { get; private set; } = 120;
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

        #region レターボックス
        // 基準（縦方向）サイズと基準アスペクト比から基準横方向サイズを算出
        var baseHorizontalSize = 5.622821f * 2560 / 1440;
        // 基準横方向サイズと対象アスペクト比で対象縦方向サイズを算出
        var verticalSize = baseHorizontalSize / Camera.main.aspect;
        Camera.main.orthographicSize = verticalSize;
        #endregion

        //目標フレームレート設定
        Application.targetFrameRate = defaultFrameRate;
    }



    private async void Start()
    {
        //Managerのインスタンス格納
        TM = TerrainManager.Instance;
        FSM = FirestoreManager.Instance;
        SDB = SkinDataBase.Instance;

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
        for (int i = 0; i < resultUIs_L.Length; i++)
            resultUIs_L[i].SetParent(resultHingeRtf_L);
        for (int i = 0; i < resultUIs_B.Length; i++)
            resultUIs_B[i].SetParent(resultHingeRtf_B);

        //ランキングモードで開始
        SetTrainingMode(false);

        //スキンパネルの生成
        skinSelecter.CreateSkinPanel();

        //プレイヤー移動可能エリアの中心の変更
        centerPos_PlayerArea = centerPos_World;



        /* 以下LoginステートのExit等に移動 */

        //データのロード
        //await FSM.SaveNewPlayerData(playerName);  //新規アカウント作成
        await FSM.LoadAll();


        //プレイヤーランク算出
        playerRank = CalculatePlayerRank(totalExp);


        //トレーニングモード到達レベルを算出
        highestTrainingLevel = trainingClearCounts.Count;
        trainingLevel = highestTrainingLevel;

        //到達レベルまでのトレーニングモードボタンを配置
        for (int i = 1; i <= highestTrainingLevel; i++)
            AddLevelPanel(i);

        //クリア回数表示を更新
        UpdatePanelCount();


        //表示スキンとスキンセレクター回転量の変更
        ChangePlayerSkin(usingSkinID);
        skinSelecter.SetWheelAngle(usingSkinID);

        //スキンを全てロック
        for (int i = 0; i < isSkinUnlocked.Length; i++)
            isSkinUnlocked[i] = false;

        //初期スキンのロック解除
        UnlockSkin(0);

        //条件を満たしているスキンのロック解除
        CheckSkinUnlock();
    }



    private void Update()
    {
        //ステートマシンのUpdateを実行
        gameStateMachine.Update(Time.deltaTime);
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
        Button_LevelSelecter button_LevelSelecter = newPanelRtf.GetComponent<Button_LevelSelecter>();
        button_LevelSelecter.SetLevel(level);
        button_LevelSelecters.Add(button_LevelSelecter);
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

        //パネル選択時の色を変更
        panelSelectedColor = SDB.skinData[skinID].UIColor;

        //スコアゲージの色を変更
        scoreGageSprite.color = SDB.skinData[skinID].UIColor - Color.black * 0.4f;

        //ボタンの色の更新
        button_LevelSelecters[trainingLevel - 1].PushButton();

        //表示スキン名の変更
        skinNameText.SetText(SDB.skinData[skinID].name);

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

        while (totalExp > 0)
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
}
