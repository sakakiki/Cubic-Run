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

        //Update�����������Ȃ牽�����Ȃ�
        if (!isActiveUpdate)
            return;

        //�|�[�Y��ԂȂ牽�����Ȃ�
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //���Ⴊ�݃{�^���������ꂽ��}�~��
        if (IM.is_Player_Squat_Push && isActive_Fall)
        {
            if (rb.linearVelocity.y > -25) rb.linearVelocity = Vector2.down * 25;

            //SE�Đ�
            AM.PlaySE(AudioManager.SE.Player_Squat);

            stateMachine.ChangeState(stateMachine.state_Play_SmallJump);
        }

        //���n������X�e�[�g�J��
        else if (isGrounded)
        {
            //�U���{�^����������Ă�����Attack�X�e�[�g��
            if (IM.is_Player_Attack_Hold && isActive_Attack)
                stateMachine.ChangeState(stateMachine.state_Play_Attack);

            //���͂��������Run�X�e�[�g��
            else stateMachine.ChangeState(stateMachine.state_Play_Run);
        }
    }

    public override void Exit() { }
}
