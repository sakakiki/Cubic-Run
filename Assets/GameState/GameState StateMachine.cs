using UnityEngine;

public class GameStateStateMachine
{
    public GameStateStateBase currentState {  get; private set; }

    public GameStateStateBase state_Login { get; private set; }
    public GameStateStateBase state_Menu { get; private set; }
    public GameStateStateBase state_MenuToPlay { get; private set; }
    public GameStateStateBase state_MenuToSkin { get; private set; }
    public GameStateStateBase state_MenuToTutorial { get; private set; }
    public GameStateStateBase state_Skin { get; private set; }
    public GameStateStateBase state_SkinToMenu { get; private set; }
    public GameStateStateBase state_Tutorial { get; private set; }
    public GameStateStateBase state_TutorialRetry { get; private set; }
    public GameStateStateBase state_Play { get; private set; }
    public GameStateStateBase state_PlayToResult { get; private set; }
    public GameStateStateBase state_UnlockSkin { get; private set; }
    public GameStateStateBase state_Result { get; private set; }
    public GameStateStateBase state_ResultToMenu { get; private set; }
    public GameStateStateBase state_ResultToPlay { get; private set; }
    public GameStateStateBase state_PauseToMenu { get; private set; }
    public GameStateStateBase state_Account { get; private set; }
    public GameStateStateBase state_Volume { get; private set; }
    public GameStateStateBase state_Button { get; private set; }
    public GameStateStateBase state_Credit { get; private set; }



    public GameStateStateMachine()
    {
        state_Login = new GameStateState_Login(this);
        state_Menu = new GameStateState_Menu(this);
        state_MenuToPlay = new GameStateState_MenuToPlay(this);
        state_MenuToSkin = new GameStateState_MenuToSkin(this);
        state_MenuToTutorial = new GameStateState_MenuToTutorial(this);
        state_Skin = new GameStateState_Skin(this);
        state_SkinToMenu = new GameStateState_SkinToMenu(this);
        state_Tutorial = new GameStateState_Tutorial(this);
        state_TutorialRetry = new GameStateState_TutorialRetry(this);
        state_Play = new GameStateState_Play(this);
        state_PlayToResult = new GameStateState_PlayToResult(this);
        state_UnlockSkin = new GameStateState_UnlockSkin(this);
        state_Result = new GameStateState_Result(this);
        state_ResultToMenu = new GameStateState_ResultToMenu(this);
        state_ResultToPlay = new GameStateState_ResultToPlay(this);
        state_PauseToMenu = new GameStateState_PauseToMenu(this);
        state_Account = new GameStateState_Option_Account(this);
        state_Volume = new GameStateState_Option_Volume(this);
        state_Button = new GameStateState_Option_Button(this);
        state_Credit = new GameStateState_Option_Credit(this);
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
