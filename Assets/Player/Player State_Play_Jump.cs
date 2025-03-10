using UnityEngine;

public class PlayerState_Play_Jump : PlayerStateBase_Play
{
    private Vector2 jumpScale = new Vector2(0.9f, 1.1f);

    public PlayerState_Play_Jump(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //���A���œV��Ɋ�����Ȃ�ʒu�𒲐�
        if (TerrainManager.Instance.currentTerrainNum == 5 && GameManager.Instance.playerTf.position.y > 1.6)
            GameManager.Instance.playerTf.position = Vector2.up * 1.6f;

        //�W�����v�p�X�P�[���ɕύX
        tf.localScale = jumpScale;
    }

    public override void Update()
    {
        base.Update();

        //���Ⴊ�݃{�^���������ꂽ��}�~��
        if (IM.is_Player_Squat_Push)
        {
            if (rb.velocity.y > -25) rb.velocity = Vector2.down * 25;

            //SE�Đ�
            AM.PlaySE(AM.SE_Player);

            stateMachine.ChangeState(stateMachine.state_Play_SmallJump);
        }

        //���n������X�e�[�g�J��
        else if (isGrounded)
        {
            //�U���{�^����������Ă�����Attack�X�e�[�g��
            if (IM.is_Player_Attack_Hold)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //���͂��������Run�X�e�[�g��
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }
    }

    public override void Exit() { }
}
