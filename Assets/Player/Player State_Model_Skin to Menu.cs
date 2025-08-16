using UnityEngine;

public class PlayerState_Model_SkinToMenu : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_SkinToMenu(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_Menu;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        //�o�ߎ��Ԃ����Z�b�g
        elapsedTime = 0;

        //�t���O���Z�b�g
        isMoving = false;

        //�ڂ��J����
        eyeTf.localScale = Vector3.one;

        //�������Z�̗L����
        rb.isKinematic = false;

        //�������W�����v
        rb.linearVelocity = Vector2.up * 7;

        //�[���������Z�p�����[�^�ݒ�
        if (InputManager.Instance.isSkinSelect)
        {
            gravity = 30;
            velocityY = 15;
        }
        else
        {
            gravity = 80;
            velocityY = 40;
        }
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //���O�v�Z
        float lerpValue = elapsedTime - 1;

        //0.5�b�ҋ@
        if ( elapsedTime < 0.5) return;

        //�X�P�[������
        if (elapsedTime < 1)
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * 2;
        else tf.localScale = Vector3.one * Mathf.Lerp(2, 1.5f, lerpValue);

        if (elapsedTime < 1) return;

        //�W�����v�J�n����
        if (!isMoving)
        {
            //�������s�ς݂̃t���O�𗧂Ă�
            isMoving = true;

            //�������Z�𖳌���
            rb.isKinematic = true;

            //�ړ��֘A�̒l���L��
            startPos = tf.position;
            posCorrectionY = 0;
        }

        //�ړ��֘A���Z
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //�Q�[���X�e�[�g��Menu�Ȃ�X�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
    }

    public override void Exit()
    {
        //�v���C���[�̈ʒu���m�肳����
        tf.position = targetPos;

        //���x��0��
        rb.linearVelocity = Vector2.zero;

        //��]�𖳌���
        rb.freezeRotation = true;
    }
}
