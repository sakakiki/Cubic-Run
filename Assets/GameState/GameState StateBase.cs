using UnityEngine;

public abstract class GameStateStateBase
{
    protected GameStateStateMachine stateMachine;
    protected GameManager GM;
    protected InputManager IM;
    protected TerrainManager TM;
    protected PlayerController playerCon;
    protected PlayerStateMachine playerStateMachine;
    protected AudioSource audioSource_BGM;

    public GameStateStateBase(GameStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        GM = GameManager.Instance;
        IM = InputManager.Instance;
        TM = TerrainManager.Instance;
        playerCon = GM.playerCon;
        playerStateMachine = playerCon.stateMachine;
        audioSource_BGM = AudioManager.Instance.audioSource_BGM;
    }

    public abstract void Enter();

    public abstract void Update(float deltaTime);

    public abstract void Exit();
}
