using UnityEngine;

public class PlayStateStateMachine
{
    public GameStateStateMachine gameStateMachine;

    public PlayStateStateBase currentState {  get; private set; }

    public PlayStateStateBase state_Play;
    public PlayStateStateBase state_LevelStart;
    public PlayStateStateBase state_LevelEnd;
    public PlayStateStateBase state_TrainingEnd;



    public PlayStateStateMachine(GameStateStateMachine gameStateMachine)
    {
        this.gameStateMachine = gameStateMachine;
        state_Play = new PlayStateState_Play(this);
        state_LevelStart = new PlayStateState_LevelStart(this);
        state_LevelEnd = new PlayStateState_LevelEnd(this);
        state_TrainingEnd = new PlayStateState_TrainingEnd(this);
    }

    public void Initialize(PlayStateStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
        PlayStateStateBase.playTime = 0;
    }

    public void ChangeState(PlayStateStateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }

    public void Update(float deltaTime)
    {
        //ステートに応じたUpdateの実行
        if (currentState != null) currentState.Update(deltaTime);
    }
}
