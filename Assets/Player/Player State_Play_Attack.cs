using UnityEngine;

public class PlayerState_Play_Attack : PlayerStateBase_Play
{
    public PlayerState_Play_Attack(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //�X�P�[���C��
        tf.localScale = Vector2.one;

        //�X�L���̐؂�ւ�
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinAttack.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

        //�U�����͉�����
        if (IM.is_Play_Attack_Release)
        {
            //���Ⴊ�ݓ��͂�Squat�X�e�[�g
            if (IM.is_Play_Squat_Hold) 
                stateMachine.ChangeState(stateMachine.state_Play_Squat);

            //���͂Ȃ���Run�X�e�[�g
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //�W�����v����
        else if (IM.is_Play_Jump_Push)
            rb.velocity = Vector2.up * 30;

        //���Ⴊ�ݓ��͂�Squat�X�e�[�g�ɑJ��
        else if (IM.is_Play_Squat_Push)
            stateMachine.ChangeState(stateMachine.state_Play_Squat);

        //�󒆂ɏo����Jump�X�e�[�g�֑J��
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_Jump);
    }

    public override void Exit()
    {
        //�X�L���̐؂�ւ�
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinAttack.SetActive(false);
    }
}
