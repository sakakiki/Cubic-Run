using UnityEngine;

public class PlayerState_Jump : PlayerStateBase
{
    private Vector2 jumpScale = new Vector2(0.9f, 1.1f);

    public PlayerState_Jump(PlayerStateMachine stateMachine) : base(stateMachine) { }

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
        //���Ⴊ�݃{�^���������ꂽ��}�~��
        if (InputManager.Instance.squatButtonPush)
        {
            if (rb.velocity.y > -25) rb.velocity = Vector2.down * 25;
            stateMachine.ChangeState(stateMachine.state_SmallJump);
        }

        //���n������X�e�[�g�J��
        else if (playerCon.isGrounded)
        {
            //�U���{�^����������Ă�����Attack�X�e�[�g��
            if (InputManager.Instance.attackButtonHold)
                stateMachine.ChangeState(stateMachine.state_Attack);

            //���͂��������Run�X�e�[�g��
            else stateMachine.ChangeState(stateMachine.state_Run);
        }
    }

    public override void Exit()
    {

    }
}
