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
    private bool isTutorial;
    public static bool isPlayerCheck;

    public GameStateState_PauseToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_L = GM.playHingeRtf_L;
        playHingeRtf_R = GM.playHingeRtf_R;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Play;
        targetCoverColor = GM.screenCoverColor_Menu;
    }



    public async override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //動作フラグリセット
        isMoveStart = false;
        //一部の処理はチュートリアルからのリタイアでは実行しない
        isTutorial =
            TutorialStateStateBase.continueState !=
            ((GameStateState_Tutorial)stateMachine.state_Tutorial).tutorialStateMachine.state_Start;

        //チュートリアル中ならチュートリアル進度をリセット
        if (isTutorial)
            TutorialStateStateBase.continueState = 
                ((GameStateState_Tutorial)stateMachine.state_Tutorial).tutorialStateMachine.state_Start;

        //プレイヤーがトンネル内なら演出を早める
        if (TM.currentTerrainNum == 3)
            elapsedTime = 1;

        //背景の地形を停止
        TM.moveSpeed = 0;
        TM.SetSpeed(0);

        //必要ならばランキングを更新
        await RankingManager.CheckUpdateNecessity(RankingManager.RankingType.HighScore);
        GM.highScoreRankingBoard.UpdateRankingDisplay();
        await RankingManager.CheckUpdateNecessity(RankingManager.RankingType.PlayerScore);
        GM.playerScoreRankingBoard.UpdateRankingDisplay();
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //1.5秒待機
        if (elapsedTime < 1) return;

        //BGMのボリューム変更
        AM.SetBGMPlayVolume(2 - elapsedTime);

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

            //レベルのテキストのZ座標を修正
            if (!isTutorial)
            {
                Vector3 levelPos = GM.levelTf.position;
                levelPos.z = GM.scoreSetTf.position.z;
                GM.levelTf.position = levelPos;
            }
        }

        //地形を管理
        //Player側の判定完了までは実行しない
        if (isPlayerCheck)
            TM.ManageMovingTerrain();

        //事前計算
        float lerpValue = (elapsedTime - 1) / 1.5f;

        //UIを回転
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        if (!isTutorial)
            playHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);

        //スクリーンカバーの色を変更
        screenCover.color =
            targetCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //指定時間経過でステート遷移
        if (elapsedTime > 2.5)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit() { }
}
