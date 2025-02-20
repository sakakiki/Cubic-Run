public class LoginStateStateMachine
{
    public GameStateStateMachine gameStateMachine;

    public LoginStateStateBase currentState {  get; private set; }

    public LoginStateStateBase state_Login { get; private set; }
    public LoginStateStateBase state_Register { get; private set; }
    public LoginStateStateBase state_Exit { get; private set; }




    public LoginStateStateMachine(GameStateStateMachine gameStateMachine)
    {
        this.gameStateMachine = gameStateMachine;
        state_Login = new LoginStateState_Login(this);
        state_Register = new LoginStateState_Register(this);
        state_Exit = new LoginStateState_Exit(this);
    }

    public void Initialize(LoginStateStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
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
