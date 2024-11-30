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

        //���Ⴊ�ݓ��͉�����Jump�X�e�[�g�ɑJ��
        if (IM.squatButtonRelease)
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
