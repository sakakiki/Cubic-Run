using UnityEngine;

public abstract class PlayerStateBase_Model : PlayerStateBase
{
    protected float elapsedTime;
    protected bool isGrounded;
    private TouchCheck trigerFoot;
    protected Transform eyeTf;
    protected static float eyeCloseTimer;
    protected static float lookDirection;

    public PlayerStateBase_Model(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        trigerFoot = stateMachine.playerController.trigerFoot;
        eyeTf = stateMachine.playerController.eyeTf;
    }

    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //接地判定
        isGrounded = trigerFoot.isTouch;

        //瞬きタイマー減算
        eyeCloseTimer -= Time.deltaTime;

        //目のスケール調整
        if (eyeCloseTimer < 0 )
            eyeTf.localScale = Vector3.one - Vector3.up * (1 - Mathf.Abs(0.1f + eyeCloseTimer));

        //瞬きが終われば
        if (eyeCloseTimer < -0.2)
        {
            //目を開ける
            eyeTf.localScale = Vector3.one;

            //次回瞬きタイミング設定
            eyeCloseTimer = Random.Range(0.5f, 5.5f);
        }

        //ゲームステートが遷移したならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_MenuToPlay ||
            gameStateMachine.currentState == gameStateMachine.state_ResultToPlay)
            stateMachine.ChangeState(stateMachine.state_Model_MenuToPlay);
    }
}
