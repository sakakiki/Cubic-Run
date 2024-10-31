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
        stateMachine.playTime += deltaTime;

        if (TM.stageRightEdge < 30)
            TM.CreateTerrainRandom();

        score.SetText("" + (int)(stateMachine.playTime * 100));

        if (playerStateMachine.currentState == 
            playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
