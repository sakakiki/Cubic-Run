using UnityEngine;

public class PlayerState_Play_Squat : PlayerStateBase_Play
{
    private TerrainManager TM;
    private Vector2 squatScale = new Vector2(1.4f, 0.5f);

    public PlayerState_Play_Squat(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        TM = TerrainManager.Instance;
    }

    public override void Enter()
    {
        //�X�P�[���C��
        tf.localScale = squatScale;
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

        //���Ⴊ�ݓ��͉�����
        if (IM.is_Player_Squat_Release && isActive_Squat)
        {
            //�g���l�����Ȃ�Q�[���I�[�o�[
            if (TM.currentTerrainNum == 3)
                stateMachine.ChangeState(stateMachine.state_GameOver);

            //�U�����͂������Attack�X�e�[�g
            else if (IM.is_Player_Attack_Hold)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //���͂Ȃ���Run�X�e�[�g
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //�W�����v����
        else if (IM.is_Player_Jump_Push && isActive_Jump)
        {
            rb.linearVelocity = Vector2.up * (isActive_SmallJump ? 20 : 30);

            //SE�Đ�
            AM.PlaySE(AudioManager.SE.Player_Jump);
        }

        //�U�����͂�Attack�X�e�[�g�ɑJ��
        else if (IM.is_Player_Attack_Push && isActive_Attack)
            stateMachine.ChangeState(stateMachine.state_Play_Attack);

        //�󒆂ɏo����Jump�X�e�[�g�֑J��
        else if (!isGrounded)
            stateMachine.ChangeState(isActive_SmallJump ? stateMachine.state_Play_SmallJump : stateMachine.state_Play_Jump);
    }

    public override void Exit() { }
}
