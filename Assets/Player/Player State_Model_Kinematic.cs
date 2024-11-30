using UnityEngine;

public class PlayerState_Model_Kinematic : PlayerStateBase_Model
{
    private float waitTimer;
    private float targetDirection;

    public PlayerState_Model_Kinematic(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //�d�̓X�P�[���␳
        rb.gravityScale = 5;

        //�J�n5�b��ɏu��
        eyeCloseTimer = 5;

        //�^�C�}�[������
        waitTimer = Random.Range(1f, 2.5f);

        //�ڂ͐��ʂɂ���
        lookDirection = 0;

        //�ŏ��Ɍ�������������
        targetDirection = Random.Range(0, 2) == 0 ? 0.1f : -0.1f;
    }

    public override void Update()
    {
        base.Update();

        //�X�e�[�g�J�n��7�b�ҋ@
        if (elapsedTime < 7) return;

        //�ڂ��ғ�����Ȃ�
        if (Mathf.Abs(lookDirection) <= Mathf.Abs(targetDirection))
        {
            //�ڂ𓮂���
            lookDirection += targetDirection * Time.deltaTime * 3;
            eyeTf.localPosition = Vector2.right * lookDirection + Vector2.up * 0.11f;
        }
        //�ڂ��ғ���O�Ȃ�
        else
        {
            //�ڂ̈ʒu����
            eyeTf.localPosition = Vector2.right * targetDirection + Vector2.up * 0.11f;

            //�ҋ@�p�^�C�}�[���Z
            waitTimer -= Time.deltaTime;

            //�ҋ@�p�^�C�}�[�̔���
            if (waitTimer > 0) return;

            //������̖ڂ̈ʒu���C��
            lookDirection = targetDirection;

            //1/2�̊m���ŐU�����
            if (Random.Range(0, 2) == 0)
            {
                targetDirection *= -1;
                waitTimer = Random.Range(1f, 2.5f);
            }
            //1/2�̊m���œ����n�߂�
            else stateMachine.ChangeState(stateMachine.state_Model_Move);
        }
    }

    public override void Exit()
    {
        //�������Z�̗L����
        rb.isKinematic = false;
    }
}
