using UnityEngine;

public class PlayerState_Model_toPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;

    public PlayerState_Model_toPlay(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_GameStart;
    }

    public override void Enter()
    {
        base.Enter();

        //�������Z�̗L����
        rb.isKinematic = false;

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

        if (elapsedTime < 0.5) return;

        //�X�P�[������
        if (elapsedTime < 1)
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * 1.5f;
        else tf.localScale = Vector3.one * Mathf.Lerp(1.5f, 1, (elapsedTime - 1) * 2);

        if (elapsedTime < 1) return;

        //�W�����v�J�n�ʒu�L��
        if (!isMoving)
        {
            isMoving = true;
            startPos = tf.position;
        }

        //�ړ�
        tf.position = 
            Vector2.Lerp(startPos, targetPos, elapsedTime - 1) + 
            Vector2.up * Mathf.Sqrt(Mathf.Sin(Mathf.PI * (elapsedTime - 1))) * 5;

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

        //��]�𖳌���
        rb.freezeRotation = true;
    }
}
