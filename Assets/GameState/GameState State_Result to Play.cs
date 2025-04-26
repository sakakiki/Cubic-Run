using UnityEngine;

public class GameStateState_ResultToPlay : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startScoreBoardScale;
    private Vector3 startPos;
    private Vector3 targetPos;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    private bool isMoveTerrein;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_B;
    private bool isResetScore;
    private Transform scoreGageTf;
    private float startGageScale;
    public static bool isPlayerCheck;

    public GameStateState_ResultToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Menu;
        targetCoverColor = GM.screenCoverColor_Play;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_B = GM.resultHingeRtf_B;
        scoreGageTf = GM.scoreGageTf;
    }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //動作フラグリセット
        isMoveTerrein = false;
        isResetScore = false;

        //スコアボードの開始スケール・開始位置・目標位置を記憶
        startScoreBoardScale = GM.isTraining ? Vector3.one * 1.5f : Vector3.one * 2 / Camera.main.aspect;
        startPos = GM.isTraining ? GM.scoreMarkerTf_Result_Trainig.position : GM.scoreMarkerTf_Result_Ranking.position;
        targetPos = GM.scoreMarkerTf_Play.position;

        //スコアゲージの初期スケールを記憶
        startGageScale = scoreGageTf.localScale.y;

        //トレーニングモードクリア状態なら演出を早める
        if (GM.score == 5000 && GM.isTraining)
            elapsedTime = 1;

        //プレイヤーがトンネル内なら演出を早める
        if (TM.currentTerrainNum == 3)
            elapsedTime = 1;

        //プレイヤーが画面外なら演出を早める
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 1.5f;

        //障害物の生成を停止
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //1.5秒待機
        if (elapsedTime < 1.5) return;

        //地形加速処理
        if (!isMoveTerrein)
        {
            isMoveTerrein = true;
            TM.moveSpeed = 30;
            TM.SetSpeed(30);
        }

        //BGMのボリューム変更
        AM.SetBGMPlayVolume(2.5f - elapsedTime);

        //地形を管理
        //Player側の判定完了までは実行しない
        if (isPlayerCheck)
            TM.ManageMovingTerrain();

        //事前計算
        float lerpValue = (elapsedTime - 1.5f) / 1.5f;

        //スコアボードの移動，スケール変更
        scoreSetTf.position = Vector3.Lerp(startPos, targetPos, lerpValue);
        scoreSetTf.localScale = Vector3.Lerp(startScoreBoardScale, Vector3.one, lerpValue);

        if (elapsedTime < 2.25)
        {
            //スコアボードの回転
            scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(360, 180, lerpValue);

            //スコア減少
            GM.scoreText.SetText("" + (int)Mathf.Lerp(GM.score, 0, lerpValue * 2));

            //スコアゲージスケール変更
            scoreGageTf.localScale = Vector3.right + Vector3.forward + Vector3.up * Mathf.Lerp(startGageScale, 0, lerpValue * 2);
        }
        else
        {
            //スコアボードの回転
            scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(180, 0, lerpValue);

            //スコアボードのリセット
            if (!isResetScore)
            {
                isResetScore = true;

                //レベルのリセット
                if (GM.isTraining)
                    GM.level = GM.trainingLevel - 1;
                else GM.level = 0;
                GM.levelText.SetText("");

                //スコアのリセット
                GM.score = 0;
                PlayStateStateBase.scoreCorrection = GM.level * 5000;
                GM.scoreText.SetText("0");

                //スコアゲージリセット
                scoreGageTf.localScale = Vector3.right + Vector3.forward;

                //レベルテキストの位置・大きさ・色調整
                GM.levelTf.position = GM.centerPos_World;
                GM.levelText.fontSize = 300;
                GM.levelText.color = Color.clear;
            }
        }

        //スクリーンカバーの色を変更
        screenCover.color =
            startCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UIを回転
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, elapsedTime - 2);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, lerpValue);

        //指定時間経過でステート遷移
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //スコアボードの位置を修正
        GM.scoreSetTf.position = GM.scoreMarkerTf_Play.position;
    }
}
