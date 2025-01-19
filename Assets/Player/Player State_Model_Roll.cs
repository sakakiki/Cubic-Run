using UnityEngine;

public class PlayerState_Model_Roll : PlayerStateBase_Model
{
    private float rollTime;
    private float rollAngularVelocity;
    private float centerX;

    public PlayerState_Model_Roll(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        centerX = GameManager.Instance.centerPos_PlayerArea.x;
    }

    public override void Enter()
    {
        base.Enter();

        //�ړ����鎞�Ԃ�����
        rollTime = Random.Range(2f, 5f);

        //��]��L����
        rb.freezeRotation = false;

        //��]���x�ݒ�
        rollAngularVelocity = Mathf.Sign(tf.position.x - centerX) * 300;

        //���Ε����ɐ���������
        rb.angularVelocity = -rollAngularVelocity * 3;
    }

    public override void Update()
    {
        base.Update();

        //�X�e�[�g�J�n��0.5�b�ҋ@
        if (elapsedTime < 0.5) return;

        //��]
        rb.angularVelocity = rollAngularVelocity;

        //�w�莞�Ԍo�߂ŉ�]�I��
        if (elapsedTime > rollTime)
            stateMachine.ChangeStateDelay(stateMachine.state_Model_ResetRotation, 2);
    }

    public override void Exit() { }
}
