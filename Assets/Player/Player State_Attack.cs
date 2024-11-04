using UnityEngine;

public class PlayerState_Attack : PlayerStateBase
{
    public PlayerState_Attack(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        tf.localScale = Vector2.one;
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinAttack.SetActive(true);
    }
    public override void Update()
    {
        if (InputManager.Instance.attackButtonRelease)
        {
            if (InputManager.Instance.squatButtonHold) 
                stateMachine.ChangeState(stateMachine.state_Squat);
            else stateMachine.ChangeState(stateMachine.state_Run);
        }

        else if (InputManager.Instance.jumpButtonPush)
            rb.velocity = Vector2.up * 30;

        else if (InputManager.Instance.squatButtonPush)
            stateMachine.ChangeState(stateMachine.state_Squat);

        else if (!playerCon.isGrounded)
            stateMachine.ChangeState(stateMachine.state_Jump);
    }
    public override void Exit()
    {
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinAttack.SetActive(false);
    }
}
