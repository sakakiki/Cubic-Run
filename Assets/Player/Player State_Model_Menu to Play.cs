using UnityEditor.Tilemaps;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class PlayerState_Model_MenuToPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 40;
    private Vector2 eyeStartPos;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_MenuToPlay(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_GameStart;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        base.Enter();

        //フラグリセット
        isMoving = false;

        //物理演算の有効化
        rb.isKinematic = false;

        //初期回転量記憶・補正
        startEulerAnglesZ = tf.eulerAngles.z;
        if (tf.eulerAngles.z > 180)
            startEulerAnglesZ -= 360;

        //目の位置を記憶
        eyeStartPos = eyeTf.localPosition;
        
        //目を開ける
        eyeTf.localScale = Vector3.one;

        //まっすぐ立っていれば小さく、そうでなければ大きくジャンプ
        rb.velocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //事前計算
        float lerpValue = elapsedTime - 1;

        //回転
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        //目の位置調整
        if (elapsedTime < 0.5)
            eyeTf.localPosition = Vector2.Lerp(eyeStartPos, eyeStandbyPos, elapsedTime * 3);
        else eyeTf.localPosition = Vector2.Lerp(eyeStandbyPos, eyePlayPos, lerpValue);

        if (elapsedTime < 0.5) return;

        //スケール調整
        if (elapsedTime < 1)
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * 1.5f;
        else tf.localScale = Vector3.one * Mathf.Lerp(1.5f, 1, lerpValue);

        if (elapsedTime < 1) return;

        //ジャンプ開始位置記憶
        if (!isMoving)
        {
            //処理実行済みのフラグを立てる
            isMoving = true;

            //物理演算を無効化
            rb.isKinematic = true;

            //移動関連の値を記憶
            startPos = tf.position;
            posCorrectionY = 0;
            velocityY = 20;
        }

        //移動関連演算
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        if (posCorrectionY < 0) posCorrectionY = 0;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //ゲームステートがPlayならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_Play)
            stateMachine.ChangeState(stateMachine.state_Play_Run);
    }

    public override void Exit()
    {
        //プレイヤーの当たり判定・描画レイヤー変更
        playerCon.SetLayer(0);
        playerCon.SetSortingLayer("Player");

        //プレイヤーの位置を確定させる
        tf.position = targetPos;

        //物理演算の有効化
        rb.isKinematic = false;

        //重力スケール補正
        rb.gravityScale = 10;

        //回転を無効化
        rb.freezeRotation = true;

        //正面のトリガーの接触判定をリセット
        playerCon.trigerFront.Initialize();

        //スキン形状がスフィアならコライダー変更
        if (SkinDataBase.Instance.skinData[GameManager.Instance.usingSkinID].bodyType == SkinData.BodyType.Sphere)
        {
            playerCon.boxCol.enabled = true;
            playerCon.capsuleCol.enabled = false;
        }
    }
}
