using UnityEngine;

public class PlayerState_Model_Jump : PlayerStateBase_Model
{
    private Vector2 jumpVector;

    public PlayerState_Model_Jump(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //スケール調整
        tf.localScale = Vector3.one * 1.5f;

        //移動時のジャンプのベクトル
        jumpVector = 
            Vector2.up * Random.Range(15f, 25f) + 
            Vector2.right * Mathf.Sign(lookDirection) * Random.Range(10f, 25f);

        //初回ジャンプ
        rb.velocity = jumpVector;
    }

    public override void Update()
    {
        base.Update();

        //時間経過でステート遷移
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Model_LookAround);
    }

    public override void Exit() { }
}
