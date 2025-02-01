using UnityEngine;

public class GameStateStateMachine
{
    public GameStateStateBase currentState {  get; private set; }

    public GameStateStateBase state_Menu { get; private set; }
    public GameStateStateBase state_MenuToPlay { get; private set; }
    public GameStateStateBase state_MenuToSkin { get; private set; }
    public GameStateStateBase state_Skin { get; private set; }
    public GameStateStateBase state_SkinToMenu { get; private set; }
    public GameStateStateBase state_Play { get; private set; }
    public GameStateStateBase state_PlayToResult { get; private set; }
    public GameStateStateBase state_Result { get; private set; }
    public GameStateStateBase state_ResultToMenu { get; private set; }
    public GameStateStateBase state_ResultToPlay { get; private set; }
    public GameStateStateBase state_PauseToMenu { get; private set; }



    public GameStateStateMachine()
    {
        state_Menu = new GameStateState_Menu(this);
        state_MenuToPlay = new GameStateState_MenuToPlay(this);
        state_MenuToSkin = new GameStateState_MenuToSkin(this);
        state_Skin = new GameStateState_Skin(this);
        state_SkinToMenu = new GameStateState_SkinToMenu(this);
        state_Play = new GameStateState_Play(this);
        state_PlayToResult = new GameStateState_PlayToResult(this);
        state_Result = new GameStateState_Result(this);
        state_ResultToMenu = new GameStateState_ResultToMenu(this);
        state_ResultToPlay = new GameStateState_ResultToPlay(this);
        state_PauseToMenu = new GameStateState_PauseToMenu(this);
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
