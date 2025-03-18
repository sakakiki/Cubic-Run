using UnityEngine;

public class GameStateState_ResultToMenu : GameStateStateBase
{
    private float elapsedTime;
    private bool isMoveStart;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_L;
    private RectTransform resultHingeRtf_L;
    private RectTransform resultHingeRtf_B;

    public GameStateState_ResultToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_L;
        resultHingeRtf_L = GM.resultHingeRtf_L;
        resultHingeRtf_B = GM.resultHingeRtf_B;
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

        //プレイヤーが画面外なら演出を早める
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 1.5f;

        //トレーニングモードクリア時は演出を早める
        if (GM.score == 5000 && GM.isTraining)
            elapsedTime = 1.5f;

        //ランキングを更新
        GM.highScoreRankingBoard.UpdateRanking();
        GM.playerScoreRankingBoard.UpdateRanking();
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //1.5秒待機
        if (elapsedTime < 1.5) return;

        //BGMのボリューム変更
        audioSource_BGM.volume = (2.5f - elapsedTime) * AM.volume_BGM;

        //UI動作開始処理
        if (!isMoveStart)
        {
            //処理実行済みのフラグを立てる
            isMoveStart = true;

            //背景の地形を動かす
            if (GM.isTraining)
            {
                TM.moveSpeed = 5 + Mathf.Pow(GM.trainingLevel, 0.7f) * 3;
                TM.SetSpeed(5 + Mathf.Pow(GM.trainingLevel, 0.7f) * 3);
            }
            else
            {
                TM.moveSpeed = 8;
                TM.SetSpeed(8);
            }
        }

        //地形を管理
        //地形動作開始後0.1秒間はPlayer側の判定のために実行しない
        if (elapsedTime > 1.6)
            TM.ManageMovingTerrain();

        //事前計算
        float lerpValue = (elapsedTime - 1.5f) / 1.5f;

        //UIを回転
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        resultHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        resultHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, lerpValue);

        //指定時間経過でステート遷移
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit()
    {
        //スコアボードの位置を修正
        GM.scoreSetTf.position = GM.scoreMarkerTf_Play.position;
        GM.scoreSetTf.localScale = Vector3.one;
    }
}
