public class GameStateState_TutorialRetry : GameStateStateBase
{
    private float elapsedTime;
    private bool isMoveTerrein;

    public GameStateState_TutorialRetry(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = - 1;

        //動作フラグリセット
        isMoveTerrein = false;

        //地形の停止
        TM.SetSpeed(0);

        //プレイヤーがトンネル内なら演出を早める
        if (TM.currentTerrainNum == 3)
            elapsedTime = 0;

        //プレイヤーが画面外なら演出を早める
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 0.5f;
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
        //地形動作開始後0.1秒間はPlayer側の判定のために実行しない
        if (elapsedTime > 1.6)
            TM.ManageMovingTerrain();

        //指定時間経過でステート遷移
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Tutorial);
    }



    public override void Exit() { }
}
