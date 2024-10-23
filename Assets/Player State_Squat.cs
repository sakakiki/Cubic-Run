using UnityEngine;

public class PlayerState_Squat : PlayerStateBase
{
    private Vector2 squatScale = new Vector2(1.4f, 0.5f);

    public PlayerState_Squat(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        tf.localScale = squatScale;
    }
    public override void Update()
    {
        if (InputManager.Instance.squatButtonRelease)
        {
            if (GameManager.Instance.currentObstacleNum == 3)
                stateMachine.ChangeState(stateMachine.state_GameOver);
            else if (InputManager.Instance.attackButtonHold)
                stateMachine.ChangeState(stateMachine.state_Attack);
            else stateMachine.ChangeState(stateMachine.state_Run);
        }

        else if (InputManager.Instance.jumpButtonPush)
            rb.velocity = Vector2.up * 20;

        else if (InputManager.Instance.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Attack);

        else if (!playerCon.isGrounded)
            stateMachine.ChangeState(stateMachine.state_SmallJump);
    }
    public override void Exit()
    {

    }
}
