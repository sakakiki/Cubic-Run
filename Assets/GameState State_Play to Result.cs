using UnityEngine;

public class GameStateState_PlaytoResult : GameStateStateBase
{
    private float elapsedTime;
    private Transform scoreSetTf;
    private Vector3 startPos;
    private Vector3 targetPos;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;
    private RectTransform resultHingeRtf_B;

    public GameStateState_PlaytoResult(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        scoreSetTf = GM.scoreSetTf;
        targetPos = GM.scoreMarkerTf.position;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
        resultHingeRtf_B = GM.resultHingeRtf_B;
    }


    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //地形の停止
        TerrainManager.Instance.SetSpeed(0);

        //スコアボードの移動開始位置を記憶
        startPos = scoreSetTf.position;

        //画面タップ検出ボタンの有効化
        IM.InputUISetActive_Screen(true);
    }



    public override void Update(float deltaTime)
    {
        //経過時間の加算
        elapsedTime += deltaTime;

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
            Vector3.up * GM.scorePosY_PlaytoResult.Evaluate(lerpValue) * 3;
        //scoreSetTf.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, lerpValue);

        //スクリーンカバーの色を変更
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //UIを回転
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 180, Vector3.zero, elapsedTime - 2);

        //指定時間経過でステート遷移
        if (elapsedTime >= 4)
            stateMachine.ChangeState(stateMachine.state_Result);
    }



    public override void Exit()
    {
        //画面タップ検出ボタンの無効化
        IM.InputUISetActive_Screen(false);

        //スコアボードの回転量をリセット
        scoreSetTf.eulerAngles = Vector3.zero;
    }
}
