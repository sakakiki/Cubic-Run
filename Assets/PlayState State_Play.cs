using TMPro;

public class PlayStateState_Play : PlayStateStateBase
{
    private TextMeshProUGUI score;
    private PlayerStateMachine playerStateMachine;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        score = GameManager.Instance.score;
        playerStateMachine = playerCon.stateMachine;
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

        //スコアテキスト更新
        score.SetText("" + (int)(playTime * 100));

        //プレイヤーがGameOverステートならGameOverステートに遷移
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
