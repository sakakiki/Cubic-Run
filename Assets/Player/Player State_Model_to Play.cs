using UnityEngine;

public class PlayerState_Model_toPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;

    public PlayerState_Model_toPlay(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_GameStart;
    }

    public override void Enter()
    {
        base.Enter();

        //物理演算の有効化
        rb.isKinematic = false;

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

        if (elapsedTime < 0.5) return;

        //スケール調整
        if (elapsedTime < 1)
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * 1.5f;
        else tf.localScale = Vector3.one * Mathf.Lerp(1.5f, 1, (elapsedTime - 1) * 2);

        if (elapsedTime < 1) return;

        //ジャンプ開始位置記憶
        if (!isMoving)
        {
            isMoving = true;
            startPos = tf.position;
        }

        //移動
        tf.position = 
            Vector2.Lerp(startPos, targetPos, elapsedTime - 1) + 
            Vector2.up * Mathf.Sqrt(Mathf.Sin(Mathf.PI * (elapsedTime - 1))) * 5;

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

        //回転を無効化
        rb.freezeRotation = true;
    }
}
