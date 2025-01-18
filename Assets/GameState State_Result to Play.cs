using UnityEngine;

public class GameStateState_ResultToPlay : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    private bool isMoveTerrein;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;
    private bool isResetScore;

    public GameStateState_ResultToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        startPos = GM.scoreMarkerTf_Result.position;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Menu;
        targetCoverColor = GM.screenCoverColor_Play;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
    }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //動作フラグリセット
        isMoveTerrein = false;
        isResetScore = false;

        //スコアボードの目標位置を記憶
        targetPos = GM.scoreMarkerTf_Play.position;

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

        //地形を管理
        TM.ManageMovingTerrain();

        //事前計算
        float lerpValue = (elapsedTime - 1.5f) / 1.5f;

        //スコアボードの移動
        scoreSetTf.position = Vector3.Lerp(startPos, targetPos, lerpValue);

        //スコアボードの回転
        if (elapsedTime < 2.25)
            scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(360, 180, lerpValue);
        else
        {
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
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, elapsedTime - 2);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, elapsedTime - 2);

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
