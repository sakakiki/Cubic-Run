using UnityEngine;

public class PlayerState_Play_Run : PlayerStateBase_Play
{
    public PlayerState_Play_Run(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        //スケール修正
        tf.localScale = Vector2.one;
    }

    public override void Update()
    {
        base.Update();

        //ポーズ状態なら何もしない
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //ジャンプ処理
        if (IM.is_Player_Jump_Push && isActive_Jump)
        {
            rb.velocity = Vector2.up * 30;

            //SE再生
            AM.PlaySE(AM.SE_Player);
        }

        //しゃがみ入力でSquatステートに遷移
        else if (IM.is_Player_Squat_Push && isActive_Squat)
        {
            stateMachine.ChangeState(stateMachine.state_Play_Squat);

            //SE再生
            AM.PlaySE(AM.SE_Player);
        }

        //攻撃入力でAttackステートに遷移
        else if (IM.is_Player_Attack_Push && isActive_Attack)
            stateMachine.ChangeState(stateMachine.state_Play_Attack);

        //空中に出たらJumpステートへ遷移
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_Jump);
    }

    public override void Exit() { }
}
