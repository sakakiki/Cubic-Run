using UnityEngine;

public class PlayerState_Model_LookAround : PlayerStateBase_Model
{
    private float waitTimer;
    private float targetDirection;

    public PlayerState_Model_LookAround(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //タイマー初期化
        waitTimer = Random.Range(1f, 2.5f);

        //最初に向く方向を決定
        if (lookDirection == targetDirection && targetDirection != 0)
            targetDirection *= -1;
        else targetDirection = Random.Range(0, 2) == 0 ? 0.1f : -0.1f;
    }

    public override void Update()
    {
        base.Update();

        //目が稼働域内なら
        if (Mathf.Abs(lookDirection) <= Mathf.Abs(targetDirection))
        {
            float eyeMove = targetDirection * Time.deltaTime * 3;

            //目を動かす
            lookDirection += eyeMove;
            eyeTf.localPosition = Vector2.right * lookDirection + Vector2.up * 0.11f;

            //目が中央なら1/4の確率で回転
            if (lookDirection * targetDirection > 0 &&
                Mathf.Abs(lookDirection) < Mathf.Abs(eyeMove))
            {
                if (Random.Range(0, 4) == 0)
                    stateMachine.ChangeStateDelay(stateMachine.state_Model_Roll, 1);
            }
        }
        //目が稼働域外なら
        else
        {
            //目の位置調整
            lookDirection = targetDirection;
            eyeTf.localPosition = Vector2.right * lookDirection + Vector2.up * 0.11f;

            //壁側を向いていたら振り向く
            if (lookDirection * (tf.position.x - GameManager.Instance.centerPos_World.x) > 0)
                stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, Random.Range(1f, 2.5f));
            else
            {
                switch(Random.Range(0, 3))
                {
                    //1/3の確率で振り返る
                    case 0:
                        stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, Random.Range(1f, 2.5f));
                        break;

                    //1/3の確率で動く
                    case 1:
                        stateMachine.ChangeStateDelay(stateMachine.state_Model_Move, Random.Range(1f, 2.5f));
                        break;

                    //1/3の確率でジャンプ
                    case 2:
                        stateMachine.ChangeStateViaSquat(stateMachine.state_Model_Jump);
                        break;
                }
            }
        }
    }

    public override void Exit() { }
}
