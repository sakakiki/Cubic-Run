public abstract class LoginStateStateBase
{
    protected LoginStateStateMachine stateMachine;
    protected GameStateStateMachine gameStateMachine;
    protected AuthManager AM;
    protected GameManager GM;
    protected InputManager IM;

    public LoginStateStateBase(LoginStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameStateMachine = stateMachine.gameStateMachine;
        AM = AuthManager.Instance;
        GM = GameManager.Instance;
        IM = InputManager.Instance;
    }

    public abstract void Enter();

    public abstract void Update();

    public abstract void Exit();
}
