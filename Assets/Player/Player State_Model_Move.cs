using UnityEngine;

public class PlayerState_Model_Move : PlayerStateBase_Model
{
    private float moveTime;
    private Vector2 jumpVector;

    public PlayerState_Model_Move(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //�ړ����鎞�Ԃ�����
        moveTime = Random.Range(2f, 6f);

        //�ړ����̃W�����v�̃x�N�g��
        jumpVector = Vector2.up * 8 + Vector2.right * Mathf.Sign(lookDirection) * 3;

        //����W�����v
        rb.linearVelocity = jumpVector;
    }

    public override void Update()
    {
        base.Update();

        //�n�ʂɂ���΃W�����v
        if (isGrounded) rb.linearVelocity = jumpVector;

        if (elapsedTime > moveTime)
            stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, 2);
    }

    public override void Exit() { }
}
