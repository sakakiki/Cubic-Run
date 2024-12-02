using UnityEngine;

public class PlayerState_Play_Attack : PlayerStateBase_Play
{
    public PlayerState_Play_Attack(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //スケール修正
        tf.localScale = Vector2.one;

        //スキンの切り替え
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinAttack.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

        //攻撃入力解除時
        if (IM.is_Play_Attack_Release)
        {
            //しゃがみ入力でSquatステート
            if (IM.is_Play_Squat_Hold) 
                stateMachine.ChangeState(stateMachine.state_Play_Squat);

            //入力なしでRunステート
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //ジャンプ処理
        else if (IM.is_Play_Jump_Push)
            rb.velocity = Vector2.up * 30;

        //しゃがみ入力でSquatステートに遷移
        else if (IM.is_Play_Squat_Push)
            stateMachine.ChangeState(stateMachine.state_Play_Squat);

        //空中に出たらJumpステートへ遷移
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_Jump);
    }

    public override void Exit()
    {
        //スキンの切り替え
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinAttack.SetActive(false);
    }
}
