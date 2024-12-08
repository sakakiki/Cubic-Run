public class GameStateState_Play : GameStateStateBase
{
    //プレイ用ステートマシン
    private PlayStateStateMachine playStateMachine;



    public GameStateState_Play(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        playStateMachine = new PlayStateStateMachine(stateMachine);
    }



    public override void Enter()
    {
        //プレイ用ステートマシンの初期化
        playStateMachine.Initialize(playStateMachine.state_LevelStart);
    }



    public override void Update(float deltaTime)
    {
        //プレイ用ステートマシンのUpdateを実行
        playStateMachine.Update(deltaTime);
    }



    public override void Exit() { }
}
