public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //�n�`�̒�~
        TerrainManager.Instance.SetSpeed(0);
    }



    public override void Update(float deltaTime)
    {

    }



    public override void Exit()
    {

    }
}
