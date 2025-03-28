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

        //SE再生
        AM.PlaySE(AM.SE_Player);
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

        //攻撃入力解除時
        if (IM.is_Player_Attack_Release && isActive_Attack)
        {
            //しゃがみ入力でSquatステート
            if (IM.is_Player_Squat_Hold)
            {
                stateMachine.ChangeState(stateMachine.state_Play_Squat);

                //SE再生
                AM.PlaySE(AM.SE_Player);
            }

            //入力なしでRunステート
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //ジャンプ処理
        else if (IM.is_Player_Jump_Push && isActive_Jump)
        {
            rb.velocity = Vector2.up * 30;

            //SE再生
            AM.PlaySE(AM.SE_Player);
        }

        //しゃがみ入力でSquatステートに遷移
        else if (IM.is_Player_Squat_Push)
        {
            stateMachine.ChangeState(stateMachine.state_Play_Squat);

            //SE再生
            AM.PlaySE(AM.SE_Player);
        }

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
