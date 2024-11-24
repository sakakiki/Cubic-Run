public class GameStateState_Play : GameStateStateBase
{
    //ステートマシン
    private PlayStateStateMachine playStateMachine;



    public GameStateState_Play(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        playStateMachine = new PlayStateStateMachine(stateMachine);
        playStateMachine.Initialize(playStateMachine.state_LevelStart);
    }



    public override void Enter()
    {

    }



    public override void Update(float deltaTime)
    {
        playStateMachine.Update(deltaTime);
    }



    public override void Exit()
    {

    }
}
