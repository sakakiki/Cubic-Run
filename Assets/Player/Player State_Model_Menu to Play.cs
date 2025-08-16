using UnityEngine;

public class PlayerState_Model_MenuToPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 40;
    private Vector2 eyeStartPos;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_MenuToPlay(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_GameStart;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        base.Enter();

        //�t���O���Z�b�g
        isMoving = false;

        //�������Z�̗L����
        rb.isKinematic = false;

        //������]�ʋL���E�␳
        startEulerAnglesZ = tf.eulerAngles.z;
        if (tf.eulerAngles.z > 180)
            startEulerAnglesZ -= 360;

        //�ڂ̈ʒu���L��
        eyeStartPos = eyeTf.localPosition;
        
        //�ڂ��J����
        eyeTf.localScale = Vector3.one;

        //�܂����������Ă���Ώ������A�����łȂ���Α傫���W�����v
        rb.linearVelocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //���O�v�Z
        float lerpValue = elapsedTime - 1;

        //��]
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        //�ڂ̈ʒu����
        if (elapsedTime < 0.5)
            eyeTf.localPosition = Vector2.Lerp(eyeStartPos, eyeStandbyPos, elapsedTime * 3);
        else eyeTf.localPosition = Vector2.Lerp(eyeStandbyPos, eyePlayPos, lerpValue);

        if (elapsedTime < 0.5) return;

        //�X�P�[������
        if (elapsedTime < 1)
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * 1.5f;
        else tf.localScale = Vector3.one * Mathf.Lerp(1.5f, 1, lerpValue);

        if (elapsedTime < 1) return;

        //�W�����v�J�n�ʒu�L��
        if (!isMoving)
        {
            //�������s�ς݂̃t���O�𗧂Ă�
            isMoving = true;

            //�������Z�𖳌���
            rb.isKinematic = true;

            //�ړ��֘A�̒l���L��
            startPos = tf.position;
            posCorrectionY = 0;
            velocityY = 20;
        }

        //�ړ��֘A���Z
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        if (posCorrectionY < 0) posCorrectionY = 0;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //�Q�[���X�e�[�g��Play��Tutorial�Ȃ�X�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_Play ||
            gameStateMachine.currentState == gameStateMachine.state_Tutorial)
            stateMachine.ChangeState(stateMachine.state_Play_Run);
    }

    public override void Exit()
    {
        //�v���C���[�̓����蔻��E�`�惌�C���[�ύX
        playerCon.SetLayer(0);
        playerCon.SetSortingLayer("Player");

        //�v���C���[�̈ʒu���m�肳����
        tf.position = targetPos;

        //�������Z�̗L����
        rb.isKinematic = false;

        //�d�̓X�P�[���␳
        rb.gravityScale = 10;

        //��]�𖳌���
        rb.freezeRotation = true;

        //���ʂ̃g���K�[�̐ڐG��������Z�b�g
        playerCon.trigerFront.Initialize();

        //�X�L���`�󂪃X�t�B�A�Ȃ�R���C�_�[�ύX
        if (SkinDataBase.Instance.skinData[GameManager.Instance.usingSkinID].bodyType == SkinData.BodyType.Sphere)
        {
            playerCon.boxCol.enabled = true;
            playerCon.capsuleCol.enabled = false;
        }
    }
}
