using UnityEngine;

public class PlayerState_Run : PlayerStateBase
{
    public PlayerState_Run(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        tf.localScale = Vector2.one;
    }
    public override void Update()
    {
        if (InputManager.Instance.jumpButtonPush)
            rb.velocity = Vector2.up * 30;

        else if (InputManager.Instance.squatButtonPush)
            stateMachine.ChangeState(stateMachine.state_Squat);

        else if (InputManager.Instance.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Attack);

        else if (!playerCon.isGrounded)
            stateMachine.ChangeState(stateMachine.state_Jump);
    }
    public override void Exit()
    {

    }
}
