using UnityEngine;

public class PlayerState_Model_ResultToPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isResetRotation;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 20;

    public PlayerState_Model_ResultToPlay(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_GameStart;
    }

    public override void Enter()
    {
        //経過時間に補正をかけてリセット
        elapsedTime = -0.5f;

        //フラグリセット
        isResetRotation = false;
        isMoving = false;

        //プレイヤーが画面外なら演出を早める
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 1;
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //0.5秒待機
        if (elapsedTime < 0) return;

        //姿勢補正開始処理
        if (!isResetRotation && elapsedTime < 1)
        {
            isResetRotation = true;

            //プレイヤーがトンネル内なら演出を早める
            if (TerrainManager.Instance.currentTerrainNum == 3 || TerrainManager.Instance.currentTerrainNum == 4)
                elapsedTime = 1;
            //トンネル外なら初期回転量記憶・補正
            else
            {
                startEulerAnglesZ = tf.eulerAngles.z;
                if (tf.eulerAngles.z > 180)
                    startEulerAnglesZ -= 360;
            }

            //重力スケール補正
            rb.gravityScale = 5;

            //まっすぐ立っていれば小さく、そうでなければ大きくジャンプ
            rb.velocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
        }

        //事前計算
        float lerpValue = (elapsedTime - 1)/1.5f;

        //回転
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        if (elapsedTime < 0.5) return;

        //1.5秒経過までは
        if (elapsedTime < 1)
        {
            //スケール調整
            tf.localScale = Vector3.one - Vector3.up * (elapsedTime - 0.5f);

            //残りの処理を飛ばす
            return;
        }

        //ジャンプ開始処理
        if (!isMoving)
        {
            //処理実行済みのフラグを立てる
            isMoving = true;

            //物理演算を無効化
            rb.isKinematic = true;

            //スケール調整
            tf.localScale = Vector3.one;

            //移動関連の値を記憶
            startPos = tf.position;
            posCorrectionY = 0;
            velocityY = 15;
        }

        //移動関連演算
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //ゲームステートがPlayならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_Play)
            stateMachine.ChangeState(stateMachine.state_Play_Run);
    }

    public override void Exit()
    {
        //プレイヤーの位置を確定させる
        tf.position = targetPos;

        //速度を0に
        rb.velocity = Vector2.zero;

        //物理演算の有効化
        rb.isKinematic = false;

        //重力スケール補正
        rb.gravityScale = 10;

        //回転を無効化
        rb.freezeRotation = true;

        //正面のトリガーの接触判定をリセット
        playerCon.trigerFront.Initialize();
    }
}
