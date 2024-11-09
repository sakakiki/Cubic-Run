using TMPro;

public class PlayStateState_Play : PlayStateStateBase
{
    private PlayerStateMachine playerStateMachine;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI levelText;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        playerStateMachine = playerCon.stateMachine;
        scoreText = GameManager.Instance.scoreText;
        levelText = GameManager.Instance.levelText;
    }



    public override void Enter()
    {

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

        //レベル更新
        GM.level = GM.score / 1000 + 1;
        levelText.SetText("Lv." + GM.level);

        //プレイヤーがGameOverステートならGameOverステートに遷移
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
