using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerStateBase
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController playerCon;
    protected Transform tf;
    protected Rigidbody2D rb;

    public PlayerStateBase(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerCon = stateMachine.playerController;
        tf = playerCon.tf;
        rb = playerCon.rb;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
