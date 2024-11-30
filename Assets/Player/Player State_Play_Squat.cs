using UnityEngine;

public class PlayerState_Play_Squat : PlayerStateBase_Play
{
    private TerrainManager TM;
    private Vector2 squatScale = new Vector2(1.4f, 0.5f);

    public PlayerState_Play_Squat(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        TM = TerrainManager.Instance;
    }

    public override void Enter()
    {
        //スケール修正
        tf.localScale = squatScale;
    }

    public override void Update()
    {
        base.Update();

        //しゃがみ入力解除時
        if (IM.squatButtonRelease)
        {
            //トンネル内ならゲームオーバー
            if (TM.currentTerrainNum == 3)
                stateMachine.ChangeState(stateMachine.state_GameOver);

            //攻撃入力があればAttackステート
            else if (IM.attackButtonHold)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //入力なしでRunステート
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //ジャンプ処理
        else if (IM.jumpButtonPush)
            rb.velocity = Vector2.up * 20;

        //攻撃入力でAttackステートに遷移
        else if (IM.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Play_Attack);

        //空中に出たらJumpステートへ遷移
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_SmallJump);
    }

    public override void Exit() { }
}
