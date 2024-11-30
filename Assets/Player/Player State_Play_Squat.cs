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

        //���Ⴊ�ݓ��͉�����
        if (IM.squatButtonRelease)
        {
            //�g���l�����Ȃ�Q�[���I�[�o�[
            if (TM.currentTerrainNum == 3)
                stateMachine.ChangeState(stateMachine.state_GameOver);

            //�U�����͂������Attack�X�e�[�g
            else if (IM.attackButtonHold)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //���͂Ȃ���Run�X�e�[�g
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }

        //�W�����v����
        else if (IM.jumpButtonPush)
            rb.velocity = Vector2.up * 20;

        //�U�����͂�Attack�X�e�[�g�ɑJ��
        else if (IM.attackButtonPush)
            stateMachine.ChangeState(stateMachine.state_Play_Attack);

        //�󒆂ɏo����Jump�X�e�[�g�֑J��
        else if (!isGrounded)
            stateMachine.ChangeState(stateMachine.state_Play_SmallJump);
    }

    public override void Exit() { }
}
