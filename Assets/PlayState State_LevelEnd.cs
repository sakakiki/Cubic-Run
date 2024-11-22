using TMPro;

public class PlayStateState_LevelEnd : PlayStateStateBase
{
    private PlayerStateMachine playerStateMachine;
    private int levelUpScore;
    private TextMeshProUGUI scoreText;



    public PlayStateState_LevelEnd(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        playerStateMachine = playerCon.stateMachine;
        scoreText = GameManager.Instance.scoreText;
    }



    public override void Enter()
    {
        //レベルが上昇するスコアを算出
        levelUpScore = GM.level * GM.levelUpSpan;

        //障害物生成を無効化
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        //プレイ時間加算
        playTime += deltaTime;

        //移動中の地形を管理
        TM.ManageMovingTerrain();

        //スコア更新
        GM.score = (int)(playTime * 100);
        scoreText.SetText("" + GM.score);

        //スコアがレベル上昇基準を満たせばステート遷移
        if (GM.score > levelUpScore)
            stateMachine.ChangeState(stateMachine.state_LevelStart);

        //プレイヤーがGameOverステートならGameOverステートに遷移
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
