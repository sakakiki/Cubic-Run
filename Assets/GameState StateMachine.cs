public class GameStateStateMachine
{
    public GameStateStateBase currentState {  get; private set; }

    public GameStateStateBase state_Menu { get; private set; }
    public GameStateStateBase state_MenuToPlay {  get; private set; }
    public GameStateStateBase state_Play { get; private set; }
    public GameStateStateBase state_PlaytoResult { get; private set; }
    public GameStateStateBase state_Result { get; private set; }



    public GameStateStateMachine()
    {
        state_Menu = new GameStateState_Menu(this);
        state_MenuToPlay = new GameStateState_MenuToPlay(this);
        state_Play = new GameStateState_Play(this);
        state_PlaytoResult = new GameStateState_PlaytoResult(this);
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
