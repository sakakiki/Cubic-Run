using UnityEngine;

public class PlayerState_Model_ResetRotation : PlayerStateBase_Model
{
    private float startEulerAnglesZ;

    public PlayerState_Model_ResetRotation(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        //初期回転量記憶・補正
        startEulerAnglesZ = tf.eulerAngles.z;
        if (tf.eulerAngles.z > 180) 
            startEulerAnglesZ -= 360;

        //まっすぐ立っていればLookAroundへ
        if (Mathf.Abs(startEulerAnglesZ) < 1)
            stateMachine.ChangeState(stateMachine.state_Model_LookAround);
        //そうでなければ上にジャンプ
        else rb.velocity = Vector2.up * 12;
    }

    public override void Update()
    {
        base.Update();

        //回転
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        //一定時間経過でステート遷移
        if (elapsedTime > 2) 
            stateMachine.ChangeState(stateMachine.state_Model_LookAround);
    }

    public override void Exit()
    {
        //回転を無効化
        rb.freezeRotation = true;
    }
}
