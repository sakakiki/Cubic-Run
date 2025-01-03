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
        //プレイ画面入力UIの有効化
        IM.InputUISetActive_Play(true);

        //プレイ用ステートマシンの初期化
        playStateMachine.Initialize(playStateMachine.state_LevelStart);

        //地形を減速
        TM.moveSpeed = 8;
        TM.SetSpeed(8);

        //ポーズ状態を解除
        PlayStateStateBase.InitializePauseState();
    }



    public override void Update(float deltaTime)
    {
        //プレイ用ステートマシンのUpdateを実行
        playStateMachine.Update(deltaTime);
    }



    public override void Exit()
    {
        //プレイ画面入力UIの無効化
        IM.InputUISetActive_Play(false);
    }
}
