using UnityEngine;

public abstract class PlayerStateBase
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController playerCon;
    protected GameStateStateMachine gameStateMachine;
    protected Transform tf;
    protected Rigidbody2D rb;

    public PlayerStateBase(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerCon = stateMachine.playerController;
        gameStateMachine = GameManager.Instance.gameStateMachine;
        tf = playerCon.tf;
        rb = playerCon.rb;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
