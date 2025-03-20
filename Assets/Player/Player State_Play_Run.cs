using UnityEngine;

public class PlayerState_Play_Run : PlayerStateBase_Play
{
    public PlayerState_Play_Run(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        //�X�P�[���C��
        tf.localScale = Vector2.one;
    }

    public override void Update()
    {
        base.Update();

        //�|�[�Y��ԂȂ牽�����Ȃ�
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //�W�����v����
        if (IM.is_Player_Jump_Push && isActive_Jump)
        {
            rb.velocity = Vector2.up * 30;

            //SE�Đ�
            AM.PlaySE(AM.SE_Player);
        }

        //���Ⴊ�ݓ��͂�Squat�X�e�[�g�ɑJ��
        else if (IM.is_Player_Squat_Push && isActive_Squat)
        {
            stateMachine.ChangeState(stateMachine.state_Play_Squat);

            //SE�Đ�
            AM.PlaySE(AM.SE_Player);
        }

        //�U�����͂�Attack�X�e�[�g�ɑJ��
        else if (IM.is_Player_Attack_Push && isActive_Attack)
            stateMachine.ChangeState(stateMachine.state_Play_Attack);

        //�󒆂ɏo����Jump�X�e�[�g�֑J��
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_Jump);
    }

    public override void Exit() { }
}
