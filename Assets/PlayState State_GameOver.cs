using TMPro;

public class PlayStateState_GameOver : PlayStateStateBase
{
    private TextMeshProUGUI score;



    public PlayStateState_GameOver(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //ínå`ÇÃí‚é~
        TerrainManager.Instance.StopTerrain();
    }



    public override void Update(float deltaTime)
    {

    }



    public override void Exit()
    {

    }
}
