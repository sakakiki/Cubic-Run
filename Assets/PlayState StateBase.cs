using TMPro;

public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected GameStateStateMachine gameStateMachine;
    protected GameManager GM;
    protected TerrainManager TM;
    protected PlayerController playerCon;
    protected PlayerStateMachine playerStateMachine;
    public static float playTime;
    private TextMeshProUGUI scoreText;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameStateMachine = stateMachine.gameStateMachine;
        GM = GameManager.Instance;
        TM = TerrainManager.Instance;
        playerCon = GM.playerCon;
        playerStateMachine = playerCon.stateMachine;
        scoreText = GM.scoreText;
    }

    public abstract void Enter();

    public virtual void Update(float deltaTime)
    {
        //プレイ時間加算
        playTime += deltaTime;

        //移動中の地形を管理
        TM.ManageMovingTerrain();

        //スコア更新
        GM.score = (int)(playTime * 100);
        scoreText.SetText("" + GM.score);

        //プレイヤーがGameOverステートならGameStateをResultに遷移
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            gameStateMachine.ChangeState(gameStateMachine.state_Result);
    }

    public abstract void Exit();
}
