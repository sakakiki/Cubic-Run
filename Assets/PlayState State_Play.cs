using TMPro;

public class PlayStateState_Play : PlayStateStateBase
{
    private TextMeshProUGUI score;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        score = GameManager.Instance.score;
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
    }



    public override void Exit()
    {

    }
}
