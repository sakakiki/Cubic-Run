using UnityEngine;

public class PlayerState_Model_Roll : PlayerStateBase_Model
{
    private float rollTime;
    private float rollAngularVelocity;
    private float centerX;

    public PlayerState_Model_Roll(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        centerX = GameManager.Instance.centerPos_PlayerArea.x;
    }

    public override void Enter()
    {
        base.Enter();

        //移動する時間を決定
        rollTime = Random.Range(2f, 5f);

        //回転を有効化
        rb.freezeRotation = false;

        //回転速度設定
        rollAngularVelocity = Mathf.Sign(tf.position.x - centerX) * 300;

        //反対方向に勢いをつける
        rb.angularVelocity = -rollAngularVelocity * 3;
    }

    public override void Update()
    {
        base.Update();

        //ステート開始後0.5秒待機
        if (elapsedTime < 0.5) return;

        //回転
        rb.angularVelocity = rollAngularVelocity;

        //指定時間経過で回転終了
        if (elapsedTime > rollTime)
            stateMachine.ChangeStateDelay(stateMachine.state_Model_ResetRotation, 2);
    }

    public override void Exit() { }
}
