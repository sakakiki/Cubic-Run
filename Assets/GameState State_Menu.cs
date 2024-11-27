public class GameStateState_Menu : GameStateStateBase
{
    public GameStateState_Menu(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        TM.isCreateObstacle = true;
    }



    public override void Update(float deltaTime)
    {
        TM.ManageMovingTerrain();
    }



    public override void Exit()
    {

    }
}
