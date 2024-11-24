using UnityEngine;

public class GameStateStateMachine
{
    public GameStateStateBase currentState {  get; private set; }

    public GameStateStateBase state_Play;
    public GameStateStateBase state_Result;



    public GameStateStateMachine()
    {
        state_Play = new GameStateState_Play(this);
        state_Result = new GameStateState_Result(this);
    }

    public void Initialize(GameStateStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
    }

    public void ChangeState(GameStateStateBase newState)
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
