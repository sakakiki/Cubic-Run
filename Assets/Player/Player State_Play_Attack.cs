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

        //SE�Đ�
        AM.PlaySE(AudioManager.SE.Player_Attack);
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

        //�U�����͉�����
        if (IM.is_Player_Attack_Release && isActive_Attack)
        {
            //���Ⴊ�ݓ��͂�Squat�X�e�[�g
            if (IM.is_Player_Squat_Hold)
            {
                stateMachine.ChangeState(stateMachine.state_Play_Squat);

                //SE�Đ�
                AM.PlaySE(AudioManager.SE.Player_Squat);
            }

            //���͂Ȃ���Run�X�e�[�g
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //�W�����v����
        else if (IM.is_Player_Jump_Push && isActive_Jump)
        {
            rb.linearVelocity = Vector2.up * 30;

            //SE�Đ�
            AM.PlaySE(AudioManager.SE.Player_Jump);
        }

        //���Ⴊ�ݓ��͂�Squat�X�e�[�g�ɑJ��
        else if (IM.is_Player_Squat_Push)
        {
            stateMachine.ChangeState(stateMachine.state_Play_Squat);

            //SE�Đ�
            AM.PlaySE(AudioManager.SE.Player_Squat);
        }

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
