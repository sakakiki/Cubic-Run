using UnityEngine;

public class PlayerState_Model_Kinematic : PlayerStateBase_Model
{
    private float waitTimer;
    private float targetDirection;

    public PlayerState_Model_Kinematic(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //重力スケール補正
        rb.gravityScale = 5;

        //開始5秒後に瞬き
        eyeCloseTimer = 5;

        //タイマー初期化
        waitTimer = Random.Range(1f, 2.5f);

        //目は正面にある
        lookDirection = 0;

        //最初に向く方向を決定
        targetDirection = Random.Range(0, 2) == 0 ? 0.1f : -0.1f;
    }

    public override void Update()
    {
        base.Update();

        //ステート開始後7秒待機
        if (elapsedTime < 7) return;

        //目が稼働域内なら
        if (Mathf.Abs(lookDirection) <= Mathf.Abs(targetDirection))
        {
            //目を動かす
            lookDirection += targetDirection * Time.deltaTime * 3;
            eyeTf.localPosition = Vector2.right * lookDirection + Vector2.up * 0.11f;
        }
        //目が稼働域外なら
        else
        {
            //目の位置調整
            eyeTf.localPosition = Vector2.right * targetDirection + Vector2.up * 0.11f;

            //待機用タイマー減算
            waitTimer -= Time.deltaTime;

            //待機用タイマーの判定
            if (waitTimer > 0) return;

            //処理上の目の位置を修正
            lookDirection = targetDirection;

            //1/2の確率で振り向く
            if (Random.Range(0, 2) == 0)
            {
                targetDirection *= -1;
                waitTimer = Random.Range(1f, 2.5f);
            }
            //1/2の確率で動き始める
            else stateMachine.ChangeState(stateMachine.state_Model_Move);
        }
    }

    public override void Exit()
    {
        //物理演算の有効化
        rb.isKinematic = false;
    }
}
