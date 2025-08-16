using UnityEngine;

public class PlayerState_Model_ResetRotation : PlayerStateBase_Model
{
    private float startEulerAnglesZ;

    public PlayerState_Model_ResetRotation(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        //������]�ʋL���E�␳
        startEulerAnglesZ = tf.eulerAngles.z;
        if (tf.eulerAngles.z > 180) 
            startEulerAnglesZ -= 360;

        //�܂����������Ă����LookAround��
        if (Mathf.Abs(startEulerAnglesZ) < 1)
            stateMachine.ChangeState(stateMachine.state_Model_LookAround);
        //�����łȂ���Ώ�ɃW�����v
        else rb.linearVelocity = Vector2.up * 12;
    }

    public override void Update()
    {
        base.Update();

        //��]
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        //��莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 0.5) 
            stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, 1.5f);
    }

    public override void Exit()
    {
        //��]�̃��Z�b�g
        rb.rotation = 0;

        //��]�𖳌���
        rb.freezeRotation = true;
    }
}
