public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //’nŒ`‚Ì’â~
        TerrainManager.Instance.SetSpeed(0);
    }



    public override void Update(float deltaTime)
    {

    }



    public override void Exit()
    {

    }
}
