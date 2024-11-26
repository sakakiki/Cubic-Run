using UnityEngine;

public class PlayerState_Run : PlayerStateBase_Play
{
    public PlayerState_Run(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        //スケール修正
        tf.localScale = Vector2.one;
    }

    public override void Update()
    {
        base.Update();

        //ジャンプ処理
        if (IM.jumpButtonPush)
            rb.velocity = Vector2.up * 30;

        //しゃがみ入力でSquatステートに遷移
        else if (IM.squatButtonPush)
            stateMachine.ChangeState(stateMachine.state_Squat);

        //攻撃入力でAttackステートに遷移
        else if (IM.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Attack);

        //空中に出たらJumpステートへ遷移
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Jump);
    }

    public override void Exit() { }
}
