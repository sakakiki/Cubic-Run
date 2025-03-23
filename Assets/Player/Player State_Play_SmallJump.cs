using UnityEngine;

public class PlayerState_Play_SmallJump : PlayerStateBase_Play
{
    private Vector2 smallJumpScale = new Vector2(1.2f, 0.8f);

    public PlayerState_Play_SmallJump(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //�X�P�[���C��
        tf.localScale = smallJumpScale;

        //�d�͂�����
        rb.gravityScale = 13;
    }

    public override void Update()
    {
        base.Update();

        //Update�����������Ȃ牽�����Ȃ�
        if (!isActiveUpdate)
            return;

        //�|�[�Y��ԂȂ牽�����Ȃ�
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //���Ⴊ�ݓ��͉�����Jump�X�e�[�g�ɑJ��
        if (IM.is_Player_Squat_Release)
            stateMachine.ChangeState(stateMachine.state_Play_Jump);

        //���n��Squat�X�e�[�g�ɑJ��
        else if (isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_Squat);
    }

    public override void Exit()
    {
        //�d�͂�߂�
        rb.gravityScale = 10;
    }
}
