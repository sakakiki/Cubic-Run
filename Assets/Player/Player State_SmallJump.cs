using UnityEngine;

public class PlayerState_SmallJump : PlayerStateBase
{
    private Vector2 smallJumpScale = new Vector2(1.2f, 0.8f);

    public PlayerState_SmallJump(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        tf.localScale = smallJumpScale;
        rb.gravityScale = 13;
    }
    public override void Update()
    {
        if (InputManager.Instance.squatButtonRelease)
            stateMachine.ChangeState(stateMachine.state_Jump);

        else if (playerCon.isGrounded)
            stateMachine.ChangeState(stateMachine.state_Squat);
    }
    public override void Exit()
    {
        rb.gravityScale = 10;
    }
}
