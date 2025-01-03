using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameStateState_PauseToMenu : GameStateStateBase
{
    private float elapsedTime;
    private bool isMoveStart;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_L;
    public RectTransform playHingeRtf_R;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;

    public GameStateState_PauseToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_R;
        playHingeRtf_R = GM.playHingeRtf_L;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
    }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //動作フラグリセット
        isMoveStart = false;

        //プレイヤーがトンネル内なら演出を早める
        if (TM.currentTerrainNum == 3)
            elapsedTime = 1;

        //背景の地形を停止
        TM.moveSpeed = 0;
        TM.SetSpeed(0);
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //1.5秒待機
        if (elapsedTime < 1) return;

        //UI動作開始処理
        if (!isMoveStart)
        {
            //処理実行済みのフラグを立てる
            isMoveStart = true;

            //背景の地形を動かす
            TM.moveSpeed = 5;
            TM.SetSpeed(5);

            //レベルのテキストのZ座標を修正
            Vector3 levelPos = GM.levelTf.position;
            levelPos.z = GM.scoreSetTf.position.z;
            GM.levelTf.position = levelPos;
        }

        //事前計算
        float lerpValue = (elapsedTime - 1) / 1.5f;

        //地形を管理
        TM.ManageMovingTerrain();

        //UIを回転
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);

        //スクリーンカバーの色を変更
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //指定時間経過でステート遷移
        if (elapsedTime > 2.5)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit() { }
}
