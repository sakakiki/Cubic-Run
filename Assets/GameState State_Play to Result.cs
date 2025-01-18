using UnityEngine;

public class GameStateState_PlayToResult : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    public RectTransform playHingeRtf_R;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;
    private AnimationCurve curveScorePosY;

    public GameStateState_PlayToResult(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        targetPos = GM.scoreMarkerTf_Result.position;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
        playHingeRtf_R = GM.playHingeRtf_R;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
        curveScorePosY = GM.scorePosY_PlaytoResult;
    }


    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

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
    }



    public override void Update(float deltaTime)
    {
        //経過時間の加算
        elapsedTime += deltaTime;

        //トレーニングモードクリア時は地形の管理
        if (GM.score == 5000 && GM.isTraining)
            TM.ManageMovingTerrain();

        //1秒間待機
        if (elapsedTime < 1) return;

        //画面がタップされれば演出を飛ばす
        IM.GetInput_Screen();
        if (IM.is_Screen_Tap) elapsedTime = 4;

        //事前計算
        float lerpValue = (elapsedTime - 1) / 2;

        //スコアボードの移動
        scoreSetTf.eulerAngles = Vector3.up * Mathf.Lerp(0, 3600, Mathf.Pow((elapsedTime - 1)/3, 0.3f));
        scoreSetTf.position = 
            Vector3.Lerp(startPos, targetPos, lerpValue) + 
            Vector3.up * curveScorePosY.Evaluate(lerpValue) * 3;

        //スクリーンカバーの色を変更
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UIを回転
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, elapsedTime - 1);
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * 120, Vector3.zero, elapsedTime - 2);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 180, Vector3.zero, elapsedTime - 2);

        //指定時間経過でステート遷移
        if (elapsedTime >= 4)
            stateMachine.ChangeState(stateMachine.state_Result);
    }



    public override void Exit()
    {
        //地形の停止
        TerrainManager.Instance.SetSpeed(0);

        //画面タップ検出ボタンの無効化
        IM.InputUISetActive_Screen(false);

        //スコアボードの回転量をリセット
        scoreSetTf.eulerAngles = Vector3.zero;
    }
}
