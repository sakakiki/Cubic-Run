public abstract class PlayerStateBase_Play : PlayerStateBase
{
    protected InputManager IM;
    protected bool isGrounded;
    private TouchCheck trigerFoot;
    private TouchCheck trigerFront;

    public PlayerStateBase_Play(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        IM = InputManager.Instance;
        trigerFoot = stateMachine.playerController.trigerFoot;
        trigerFront = stateMachine.playerController.trigerFront;
    }

    public override void Update()
    {
        //入力取得
        IM.GetInput_Play();

        //接地判定
        isGrounded = trigerFoot.isTouch;

        //ゲームオーバーへの遷移
        if (trigerFront.isTouch || tf.position.y < -5)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }
}
