using UnityEngine;

public class PlayerState_Model_SkinToMenu : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_SkinToMenu(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_Menu;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        //経過時間をリセット
        elapsedTime = 0;

        //フラグリセット
        isMoving = false;

        //物理演算の有効化
        rb.isKinematic = false;

        //小さくジャンプ
        rb.velocity = Vector2.up * 7;

        //擬似物理演算パラメータ設定
        if (InputManager.Instance.isSkinSelect)
        {
            gravity = 30;
            velocityY = 15;
        }
        else
        {
            gravity = 80;
            velocityY = 40;
        }
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //事前計算
        float lerpValue = elapsedTime - 1;

        //0.5秒待機
        if ( elapsedTime < 0.5) return;

        //スケール調整
        if (elapsedTime < 1)
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * 2;
        else tf.localScale = Vector3.one * Mathf.Lerp(2, 1.5f, lerpValue);

        if (elapsedTime < 1) return;

        //ジャンプ開始処理
        if (!isMoving)
        {
            //処理実行済みのフラグを立てる
            isMoving = true;

            //物理演算を無効化
            rb.isKinematic = true;

            //移動関連の値を記憶
            startPos = tf.position;
            posCorrectionY = 0;
        }

        //移動関連演算
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //ゲームステートがMenuならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
    }

    public override void Exit()
    {
        //プレイヤーの位置を確定させる
        tf.position = targetPos;

        //速度を0に
        rb.velocity = Vector2.zero;

        //回転を無効化
        rb.freezeRotation = true;
    }
}
