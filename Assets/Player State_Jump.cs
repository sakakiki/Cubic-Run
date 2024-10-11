using UnityEngine;

public class PlayerState_Jump : PlayerStateBase
{
    private Vector2 jumpScale = new Vector2(0.9f, 1.1f);

    public PlayerState_Jump(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        tf.localScale = jumpScale;
    }
    public override void Update()
    {
        if (InputManager.Instance.squatButtonPush)
        {
            if (rb.velocity.y > -25) rb.velocity = Vector2.down * 25;
            stateMachine.ChangeState(stateMachine.state_SmallJump);
        }

        else if (playerCon.isGrounded)
        {
            if (InputManager.Instance.attackButtonHold)
                stateMachine.ChangeState(stateMachine.state_Attack);
            else stateMachine.ChangeState(stateMachine.state_Run);
        }
    }
    public override void Exit()
    {

    }
}
