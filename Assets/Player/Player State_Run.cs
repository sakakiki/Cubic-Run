using UnityEngine;

public class PlayerState_Run : PlayerStateBase_Play
{
    public PlayerState_Run(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        //�X�P�[���C��
        tf.localScale = Vector2.one;
    }

    public override void Update()
    {
        base.Update();

        //�W�����v����
        if (IM.jumpButtonPush)
            rb.velocity = Vector2.up * 30;

        //���Ⴊ�ݓ��͂�Squat�X�e�[�g�ɑJ��
        else if (IM.squatButtonPush)
            stateMachine.ChangeState(stateMachine.state_Squat);

        //�U�����͂�Attack�X�e�[�g�ɑJ��
        else if (IM.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Attack);

        //�󒆂ɏo����Jump�X�e�[�g�֑J��
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Jump);
    }

    public override void Exit() { }
}
