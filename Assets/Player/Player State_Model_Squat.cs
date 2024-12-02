using UnityEngine;

public class PlayerState_Model_Squat : PlayerStateBase_Model
{
    public PlayerStateBase nextState;

    public PlayerState_Model_Squat(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //スケール調整
        tf.localScale = (Vector3.one - Vector3.up * elapsedTime) * 1.5f;

        //0.5秒待機後にステート遷移
        if (elapsedTime > 0.5)
            stateMachine.ChangeState(nextState);
    }

    public override void Exit()
    {
        //スケール調整
        tf.localScale = Vector3.one * 1.5f;
    }
}
