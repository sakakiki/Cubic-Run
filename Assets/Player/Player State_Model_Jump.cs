using UnityEngine;

public class PlayerState_Model_Jump : PlayerStateBase_Model
{
    private Vector2 jumpVector;

    public PlayerState_Model_Jump(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //�ړ����̃W�����v�̃x�N�g��
        jumpVector = 
            Vector2.up * Random.Range(15f, 25f) + 
            Vector2.right * Mathf.Sign(lookDirection) * Random.Range(10f, 25f);

        //����W�����v
        rb.linearVelocity = jumpVector;
    }

    public override void Update()
    {
        base.Update();

        //���Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 3)
            stateMachine.ChangeState(stateMachine.state_Model_LookAround);
    }

    public override void Exit() { }
}
