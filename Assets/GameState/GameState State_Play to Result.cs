using System.Globalization;
using TMPro;
using UnityEngine;

public class GameStateState_PlayToResult : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 targetScale;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;
    private AnimationCurve curveScorePosY;
    private RectTransform playerRankScaleRtf;
    private TextMeshProUGUI playerRankText;
    private TextMeshProUGUI addExpText;
    private TextMeshProUGUI requiredExpText;
    private int beforePlayRequiredExp;
    private int addExp;
    private int beforePlayRank;
    private TextMeshProUGUI highScoreFluctuationText;
    private TextMeshProUGUI playerScoreFluctuationText;
    private bool isSetText;
    private int beforeHighScore;
    private int beforePlayerScore;
    private int newPlayerScore;
    private TextMeshProUGUI highScoreText;
    private TextMeshProUGUI playerScoreText;
    private bool isAudioPlay;
    private int levelUpCount_state;

    public GameStateState_PlayToResult(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
        curveScorePosY = GM.scorePosY_PlaytoResult;
        playerRankScaleRtf = GM.playerRankScaleRtf;
        playerRankText = GM.playerRankText;
        addExpText = GM.addExpText;
        requiredExpText = GM.requiredExpText;
        highScoreFluctuationText = GM.scoreFluctuation_highScore;
        playerScoreFluctuationText = GM.scoreFluctuation_playerScore;
        highScoreText = GM.resultHighScoreText;
        playerScoreText = GM.resultPlayerScoreText;
    }


    public override async void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //フラグリセット
        isSetText = false;
        isAudioPlay = false;

        //変数リセット
        levelUpCount_state = 0;

        //ゲームモードに応じてUIの切り替え
        GM.resultRankingUI.SetActive(!GM.isTraining);
        GM.resultTrainingUI.SetActive(GM.isTraining);

        //ゲームモードに応じた移動先・スケールの変更
        targetPos = GM.isTraining ? GM.scoreMarkerTf_Result_Trainig.position : GM.scoreMarkerTf_Result_Ranking.position;
        targetScale = GM.isTraining ? Vector3.one * 1.5f : Vector3.one * 2 / Camera.main.aspect;

        //UIの内容変更
        if (GM.isTraining)
        {
            GM.clearRateText.SetText((GM.score / 5000f * 100).ToString("F1", CultureInfo.CurrentCulture) + "%");
            GM.clearTimesNumText.SetText("走破回数：" + GM.trainingClearCounts[GM.level - 1] + "回");
        }

        //トレーニングモードクリア時を除き
        if (GM.score != 5000 || !GM.isTraining)
        {
            //地形の停止
            TerrainManager.Instance.SetSpeed(0);

            //リトライボタンのテキストを変更
            GM.retryButtonText.SetText("リトライ");
        }

        //画面タップ検出ボタンの有効化
        IM.InputUISetActive_Screen(true);

        //スコアボードの移動開始位置を記憶
        startPos = GM.scoreMarkerTf_Play.position;

        //プレイ前のランクアップまでに必要な経験値量を記憶
        beforePlayRequiredExp = GM.requiredExp;

        //プレイ前の経験値量を記憶
        int beforePlaytotalExp = GM.totalExp;

        //プレイ前のプレイヤーランクを記憶・表示
        beforePlayRank = GM.playerRank; 
        playerRankText.SetText("プレイヤーランク　" + beforePlayRank);

        //経験値に関する表示の更新
        requiredExpText.SetText("" + GM.requiredExp);
        playerRankScaleRtf.localScale = Vector3.one - Vector3.right * (GM.requiredExp / (float)((GM.playerRank+1) * 100));

        //ランキングモードならプレイ前のスコアを記憶・記憶
        if (!GM.isTraining)
        {
            beforeHighScore = GM.highScore;
            beforePlayerScore = GM.GetPlayerScore();
            highScoreText.SetText(beforeHighScore.ToString());
            playerScoreText.SetText(beforePlayerScore.ToString());
        }

        //プレイ結果のセーブ
        await GM.SaveResult();

        //獲得経験値量の算出
        addExp = GM.totalExp - beforePlaytotalExp;

        //更新後のプレイヤースコアを記憶
        newPlayerScore = GM.GetPlayerScore();

        //スキン開放演出の必要性をチェック
        if (beforePlayRank < 10 && 10 <= GM.playerRank) GM.newSkinQueue.Enqueue(2);
        if (beforePlayRank < 20 && 20 <= GM.playerRank) GM.newSkinQueue.Enqueue(3);
        if (beforePlayRank < 30 && 30 <= GM.playerRank) GM.newSkinQueue.Enqueue(4);
        if (beforePlayRank < 40 && 40 <= GM.playerRank) GM.newSkinQueue.Enqueue(5);
        if (beforePlayRank < 50 && 50 <= GM.playerRank) GM.newSkinQueue.Enqueue(6);
        if (beforePlayRank < 100 && 100 <= GM.playerRank) GM.newSkinQueue.Enqueue(7);
        if (GM.score == 5000 && GM.isTraining && GM.trainingClearCounts[GM.level-1] == 1)
        {
            switch (GM.level)
            {
                case 1: GM.newSkinQueue.Enqueue(8); break;
                case 2: GM.newSkinQueue.Enqueue(9); break;
                case 3: GM.newSkinQueue.Enqueue(10); break;
                case 5: GM.newSkinQueue.Enqueue(11); break;
                case 7: GM.newSkinQueue.Enqueue(12); break;
                case 10: GM.newSkinQueue.Enqueue(13); break;
                case 15: GM.newSkinQueue.Enqueue(14); break;
                case 20: GM.newSkinQueue.Enqueue(15); break;
                default: break;
            }
        }

        //ランキング更新
        GM.resultRankingBoard.UpdateRanking();

        //経験値・スコア変動表示リセット
        addExpText.SetText("");
        highScoreFluctuationText.SetText("");
        playerScoreFluctuationText.SetText("");
    }



    public override void Update(float deltaTime)
    {
        //経過時間の加算
        elapsedTime += deltaTime;

        //トレーニングモードクリア時は地形の管理
        if (GM.score == 5000 && GM.isTraining)
            TM.ManageMovingTerrain();

        //開始後1秒間
        if (elapsedTime < 1)
        {
            //BGMのボリューム変更
            audioSource_BGM.volume = (1 - elapsedTime) * AM.volume_BGM;

            //残りの処理を飛ばす
            return;
        }

        //リザルト用BGMの再生
        else if (!isAudioPlay)
        {
            //実行は1回のみ
            isAudioPlay = true;

            //BGM再生
            audioSource_BGM.volume = AM.volume_BGM;
            audioSource_BGM.clip = AM.BGM_Result;
            audioSource_BGM.Play();
        }

        //画面がタップされれば演出を飛ばす
        IM.GetInput_Screen();
        if (IM.is_Screen_Tap) elapsedTime = 5;

        //事前計算
        float lerpValue = (elapsedTime - 1) / 2;

        //スコアボードの移動，回転，スケール変更
        scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(0, 3600, Mathf.Pow((elapsedTime - 1)/3, 0.3f));
        scoreSetTf.position = 
            Vector3.Lerp(startPos, targetPos, lerpValue) + 
            Vector3.up * curveScorePosY.Evaluate(lerpValue) * 3;
        scoreSetTf.localScale = Vector3.Lerp(Vector3.one, targetScale, lerpValue);

        //スクリーンカバーの色を変更
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UIを回転
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, elapsedTime - 1);
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * 120, Vector3.zero, elapsedTime - 2);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 120, Vector3.zero, elapsedTime - 2);

        if (elapsedTime < 3) return;

        //テキスト表示
        if (!isSetText)
        {
            //実行は1回のみ
            isSetText = true;

            //獲得経験値量の表示
            //トレーニングモード完走時のみ例外処理
            if (GM.score == 5000 && GM.isTraining)
                addExpText.SetText("+" + addExp + "Exp（完走ボーナス+" + GM.level * 50 + "Exp）");
            else
                addExpText.SetText("+" + addExp + "Exp");

            //SEの再生
            AM.audioSource_SE.PlayOneShot(AM.SE_GetExp);

            if (!GM.isTraining)
            {
                //スコア変動表示
                if (GM.highScore > beforeHighScore)
                    highScoreFluctuationText.SetText("<color=#00951F>+" + (GM.highScore - beforeHighScore) + "</color>");
                else highScoreFluctuationText.SetText("");
                if (GM.GetPlayerScore() > beforePlayerScore)
                    playerScoreFluctuationText.SetText("<color=#00951F>▲" + (GM.GetPlayerScore() - beforePlayerScore) + "</color>");
                else if (GM.GetPlayerScore() < beforePlayerScore)
                    playerScoreFluctuationText.SetText("<color=#BA3C46>▼" + (beforePlayerScore - GM.GetPlayerScore()) + "</color>");
                else playerScoreFluctuationText.SetText("<color=\"black\">±0</color>");
            }
        }

        //表示するランクアップまでに必要な経験値量を算出
        float displayAddExpF = Mathf.Lerp(0, addExp, Mathf.Pow(elapsedTime - 3, 0.35f));
        float displayRequiredExpF = beforePlayRequiredExp - (int)displayAddExpF;

        //表示プレイヤーランクの更新
        int displayRank = beforePlayRank;
        int levelUpCount_local = 0;
        while (displayRequiredExpF < 0)
        {
            displayRank++;
            displayRequiredExpF += (displayRank + 1) * 100;
            playerRankText.SetText("プレイヤーランク　" + beforePlayRank + " >> <b>" + displayRank + "</b>");
            levelUpCount_local++;
            if (levelUpCount_local > levelUpCount_state)
            {
                //SEの再生
                AM.audioSource_SE.PlayOneShot(AM.SE_LevelUp);
                levelUpCount_state++;
            }
        }

        //経験値に関する表示の更新
        requiredExpText.SetText("" + (int)displayRequiredExpF);
        playerRankScaleRtf.localScale = Vector3.one - Vector3.right * (displayRequiredExpF / (float)((displayRank+1) * 100));

        //ランキングモードならスコア表示の変動
        if (!GM.isTraining)
        {
            highScoreText.SetText("" + (int)Mathf.Lerp(beforeHighScore, GM.highScore, elapsedTime - 3));
            playerScoreText.SetText("" + (int)Mathf.Lerp(beforePlayerScore, newPlayerScore, elapsedTime - 3));
        }

        //指定時間経過でステート遷移
        if (elapsedTime >= 5)
        {
            if (GM.newSkinQueue.Count > 0) stateMachine.ChangeState(stateMachine.state_UnlockSkin);
            else stateMachine.ChangeState(stateMachine.state_Result);
        }
    }



    public override void Exit()
    {
        //地形の停止
        TerrainManager.Instance.SetSpeed(0);

        //画面タップ検出ボタンの無効化
        IM.InputUISetActive_Screen(false);

        //スコアボードの回転量をリセット
        scoreSetTf.eulerAngles = Vector3.zero;

        //プレイヤーランクに関する表示を確定
        if (beforePlayRank != GM.playerRank)
            playerRankText.SetText("プレイヤーランク　" + beforePlayRank + " >> <b>" + GM.playerRank + "</b>");
        requiredExpText.SetText("" + GM.requiredExp);
        playerRankScaleRtf.localScale = Vector3.one - Vector3.right * (GM.requiredExp / (float)((GM.playerRank + 1) * 100));

        //SEの停止
        AM.audioSource_SE.Stop();
    }
}
