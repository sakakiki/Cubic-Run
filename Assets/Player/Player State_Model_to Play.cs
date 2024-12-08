using UnityEngine;

public class PlayerState_Model_toPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;

    public PlayerState_Model_toPlay(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        //初期回転量記憶・補正
        startEulerAnglesZ = tf.eulerAngles.z;
        if (tf.eulerAngles.z > 180)
            startEulerAnglesZ -= 360;

        //まっすぐ立っていれば小さく、そうでなければ大きくジャンプ
        rb.velocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //回転
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        //回転のリセット
        rb.rotation = 0;

        //回転を無効化
        rb.freezeRotation = true;

        //ゲームステートがPlayならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_Play)
            stateMachine.ChangeState(stateMachine.state_Play_Run);
    }

    public override void Exit()
    {
        //プレイヤーの当たり判定・描画レイヤー変更
        playerCon.SetLayer(0);
        playerCon.SetSortingLayer("Player");

        //重力スケール補正
        rb.gravityScale = 10;
    }
}
