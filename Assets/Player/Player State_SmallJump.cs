using UnityEngine;

public class PlayerState_SmallJump : PlayerStateBase_Play
{
    private Vector2 smallJumpScale = new Vector2(1.2f, 0.8f);

    public PlayerState_SmallJump(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //スケール修正
        tf.localScale = smallJumpScale;

        //重力を強化
        rb.gravityScale = 13;
    }

    public override void Update()
    {
        base.Update();

        //しゃがみ入力解除でJumpステートに遷移
        if (IM.squatButtonRelease)
            stateMachine.ChangeState(stateMachine.state_Jump);

        //着地でSquatステートに遷移
        else if (isGrounded)
            stateMachine.ChangeState(stateMachine.state_Squat);
    }

    public override void Exit()
    {
        //重力を戻す
        rb.gravityScale = 10;
    }
}
