using UnityEngine;

public class PlayerState_Model_toPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;

    public PlayerState_Model_toPlay(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        //������]�ʋL���E�␳
        startEulerAnglesZ = tf.eulerAngles.z;
        if (tf.eulerAngles.z > 180)
            startEulerAnglesZ -= 360;

        //�܂����������Ă���Ώ������A�����łȂ���Α傫���W�����v
        rb.velocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //��]
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        //��]�̃��Z�b�g
        rb.rotation = 0;

        //��]�𖳌���
        rb.freezeRotation = true;

        //�Q�[���X�e�[�g��Play�Ȃ�X�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_Play)
            stateMachine.ChangeState(stateMachine.state_Play_Run);
    }

    public override void Exit()
    {
        //�v���C���[�̓����蔻��E�`�惌�C���[�ύX
        playerCon.SetLayer(0);
        playerCon.SetSortingLayer("Player");

        //�d�̓X�P�[���␳
        rb.gravityScale = 10;
    }
}
