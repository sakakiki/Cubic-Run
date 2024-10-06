using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerStateBase
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController playerCon;
    protected Rigidbody2D rb;

    public PlayerStateBase(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerCon = stateMachine.playerController;
        rb = playerCon.rb;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
