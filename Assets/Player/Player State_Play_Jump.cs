using UnityEngine;

public class PlayerState_Play_Jump : PlayerStateBase_Play
{
    private Vector2 jumpScale = new Vector2(0.9f, 1.1f);

    public PlayerState_Play_Jump(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //洞窟内で天井に干渉するなら位置を調整
        if (TerrainManager.Instance.currentTerrainNum == 5 && GameManager.Instance.playerTf.position.y > 1.6)
            GameManager.Instance.playerTf.position = Vector2.up * 1.6f;

        //ジャンプ用スケールに変更
        tf.localScale = jumpScale;
    }

    public override void Update()
    {
        base.Update();

        //しゃがみボタンが押されたら急降下
        if (IM.is_Player_Squat_Push)
        {
            if (rb.velocity.y > -25) rb.velocity = Vector2.down * 25;

            //SE再生
            AM.PlaySE(AM.SE_Player);

            stateMachine.ChangeState(stateMachine.state_Play_SmallJump);
        }

        //着地したらステート遷移
        else if (isGrounded)
        {
            //攻撃ボタンが押されていたらAttackステートへ
            if (IM.is_Player_Attack_Hold)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //入力が無ければRunステートへ
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }
    }

    public override void Exit() { }
}
