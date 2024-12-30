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
        //���͎擾
        IM.GetInput_Play();

        //�ڒn����
        isGrounded = trigerFoot.isTouch;

        //�Q�[���I�[�o�[�ւ̑J��
        if (trigerFront.isTouch || tf.position.y < -5)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }
}
