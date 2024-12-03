using UnityEngine;

public class PlayerState_Model_LookAround : PlayerStateBase_Model
{
    private float waitTimer;
    private float targetDirection;

    public PlayerState_Model_LookAround(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //�^�C�}�[������
        waitTimer = Random.Range(1f, 2.5f);

        //�ŏ��Ɍ�������������
        if (lookDirection == targetDirection && targetDirection != 0)
            targetDirection *= -1;
        else targetDirection = Random.Range(0, 2) == 0 ? 0.1f : -0.1f;
    }

    public override void Update()
    {
        base.Update();

        //�ڂ��ғ�����Ȃ�
        if (Mathf.Abs(lookDirection) <= Mathf.Abs(targetDirection))
        {
            float eyeMove = targetDirection * Time.deltaTime * 3;

            //�ڂ𓮂���
            lookDirection += eyeMove;
            eyeTf.localPosition = Vector2.right * lookDirection + Vector2.up * 0.11f;

            //�ڂ������Ȃ�1/4�̊m���ŉ�]
            if (lookDirection * targetDirection > 0 &&
                Mathf.Abs(lookDirection) < Mathf.Abs(eyeMove))
            {
                if (Random.Range(0, 4) == 0)
                    stateMachine.ChangeStateDelay(stateMachine.state_Model_Roll, 1);
            }
        }
        //�ڂ��ғ���O�Ȃ�
        else
        {
            //�ڂ̈ʒu����
            lookDirection = targetDirection;
            eyeTf.localPosition = Vector2.right * lookDirection + Vector2.up * 0.11f;

            //�Ǒ��������Ă�����U�����
            if (lookDirection * (tf.position.x - GameManager.Instance.centerPos_World.x) > 0)
                stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, Random.Range(1f, 2.5f));
            else
            {
                switch(Random.Range(0, 3))
                {
                    //1/3�̊m���ŐU��Ԃ�
                    case 0:
                        stateMachine.ChangeStateDelay(stateMachine.state_Model_LookAround, Random.Range(1f, 2.5f));
                        break;

                    //1/3�̊m���œ���
                    case 1:
                        stateMachine.ChangeStateDelay(stateMachine.state_Model_Move, Random.Range(1f, 2.5f));
                        break;

                    //1/3�̊m���ŃW�����v
                    case 2:
                        stateMachine.ChangeStateViaSquat(stateMachine.state_Model_Jump);
                        break;
                }
            }
        }
    }

    public override void Exit() { }
}
