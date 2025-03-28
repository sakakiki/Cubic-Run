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

        //Updateが無効化中なら何もしない
        if (!isActiveUpdate)
            return;

        //ポーズ状態なら何もしない
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //しゃがみ入力解除時
        if (IM.is_Player_Squat_Release && isActive_Squat)
        {
            //トンネル内ならゲームオーバー
            if (TM.currentTerrainNum == 3)
                stateMachine.ChangeState(stateMachine.state_GameOver);

            //攻撃入力があればAttackステート
            else if (IM.is_Player_Attack_Hold)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //入力なしでRunステート
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //ジャンプ処理
        else if (IM.is_Player_Jump_Push && isActive_Jump)
        {
            rb.velocity = Vector2.up * (isActive_SmallJump ? 20 : 30);

            //SE再生
            AM.PlaySE(AM.SE_Player);
        }

        //攻撃入力でAttackステートに遷移
        else if (IM.is_Player_Attack_Push && isActive_Attack)
            stateMachine.ChangeState(stateMachine.state_Play_Attack);

        //空中に出たらJumpステートへ遷移
        else if (!isGrounded)
            stateMachine.ChangeState(isActive_SmallJump ? stateMachine.state_Play_SmallJump : stateMachine.state_Play_Jump);
    }

    public override void Exit() { }
}
