using UnityEngine;

public class PlayStateStateMachine
{
    public PlayStateStateBase currentState {  get; private set; }

    public PlayStateStateBase state_Play;
    public PlayStateStateBase state_GameOver;
    public PlayStateStateBase state_LevelStart;
    public PlayStateStateBase state_LevelEnd;



    public PlayStateStateMachine()
    {
        state_Play = new PlayStateState_Play(this);
        state_GameOver = new PlayStateState_GameOver(this);
        state_LevelStart = new PlayStateState_LevelStart(this);
        state_LevelEnd = new PlayStateState_LevelEnd(this);
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

    public void Update()
    {
        //ステートに応じたUpdateの実行
        if (currentState != null) currentState.Update(Time.deltaTime);
    }
}
