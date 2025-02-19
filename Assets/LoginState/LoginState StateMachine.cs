using UnityEngine;

public class LoginStateStateMachine
{
    public GameStateStateMachine gameStateMachine;

    public LoginStateStateBase currentState {  get; private set; }

    public LoginStateState_Select state_Select { get; private set; }
    public LoginStateState_Exit state_Exit { get; private set; }




    public LoginStateStateMachine(GameStateStateMachine gameStateMachine)
    {
        this.gameStateMachine = gameStateMachine;
        state_Select = new LoginStateState_Select(this);
        state_Exit = new LoginStateState_Exit(this);
    }

    public void Initialize(LoginStateStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
        PlayStateStateBase.playTime = 0;
    }

    public void ChangeState(LoginStateStateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }

    public void Update()
    {
        //ステートに応じたUpdateの実行
        if (currentState != null) currentState.Update();
    }
}
