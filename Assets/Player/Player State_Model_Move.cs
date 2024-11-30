using UnityEngine;

public class PlayerState_Model_Move : PlayerStateBase_Model
{
    private float moveTime;
    private Vector2 jumpVector;

    public PlayerState_Model_Move(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //移動する時間を決定
        moveTime = Random.Range(2f, 6f);

        //移動時のジャンプのベクトル
        jumpVector = Vector2.up * 8 + Vector2.right * Mathf.Sign(lookDirection) * 3;

        //初回ジャンプ
        rb.velocity = jumpVector;
    }

    public override void Update()
    {
        base.Update();

        //地面にいればジャンプ
        if (isGrounded) rb.velocity = jumpVector;

        if (elapsedTime > moveTime)
            stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, 2);
    }

    public override void Exit() { }
}
