using UnityEngine;

public class PlayerState_Model_TrainingClear: PlayerStateBase_Model
{
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 20;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_TrainingClear(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_TrainingClear;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        //経過時間に補正をかけてリセット
        elapsedTime = -1;

        //フラグリセット
        isMoving = false;

        //スケール調整
        tf.localScale = Vector3.one;
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //0.5秒待機
        if (elapsedTime < -0.5) return;

        //事前計算
        float lerpValue = elapsedTime / 1.5f;

        //スケール調整
        if (elapsedTime < 0)
            tf.localScale = Vector3.one - Vector3.up * (elapsedTime + 0.5f);
        else tf.localScale = Vector3.one * Mathf.Lerp(1, 1.5f, lerpValue);

        //ジャンプタイミングまで待機
        if (elapsedTime < 0) return;

        //ジャンプ開始処理
        if (!isMoving)
        {
            //処理実行済みのフラグを立てる
            isMoving = true;

            //重力スケール補正
            rb.gravityScale = 5;

            //物理演算を無効化
            rb.isKinematic = true;

            //移動関連の値を記憶
            startPos = tf.position;
            posCorrectionY = 0;
            velocityY = 15;

            //プレイヤーの当たり判定・描画レイヤー変更
            playerCon.SetLayer(5);
            playerCon.SetSortingLayer("Model");
        }

        //目の位置調整
        eyeTf.localPosition = Vector2.Lerp(eyePlayPos, eyeStandbyPos, lerpValue);

        //移動関連演算
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //時間経過でステート遷移
        if (elapsedTime > 1.5)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
        //演出がスキップされてもステート遷移
        else if (gameStateMachine.currentState == gameStateMachine.state_Result)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
    }

    public override void Exit()
    {
        //プレイヤーの位置を確定させる
        tf.position = targetPos;

        //プレイヤーのスケールを確定させる
        tf.localScale = Vector3.one * 1.5f;

        //プレイヤーの目の位置を確定させる
        eyeTf.localPosition = eyeStandbyPos;

        //速度を0に
        rb.velocity = Vector2.zero;

        //回転を無効化
        rb.freezeRotation = true;
    }
}
