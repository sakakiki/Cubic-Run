using UnityEngine;

public class PlayerState_Model_TrainingClear: PlayerStateBase_Model
{
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 20;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_TrainingClear(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_TrainingClear;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        //�o�ߎ��Ԃɕ␳�������ă��Z�b�g
        elapsedTime = -1;

        //�t���O���Z�b�g
        isMoving = false;

        //�X�P�[������
        tf.localScale = Vector3.one;
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //0.5�b�ҋ@
        if (elapsedTime < -0.5) return;

        //���O�v�Z
        float lerpValue = elapsedTime / 1.5f;

        //�X�P�[������
        if (elapsedTime < 0)
            tf.localScale = Vector3.one - Vector3.up * (elapsedTime + 0.5f);
        else tf.localScale = Vector3.one * Mathf.Lerp(1, 1.5f, lerpValue);

        //�W�����v�^�C�~���O�܂őҋ@
        if (elapsedTime < 0) return;

        //�W�����v�J�n����
        if (!isMoving)
        {
            //�������s�ς݂̃t���O�𗧂Ă�
            isMoving = true;

            //�d�̓X�P�[���␳
            rb.gravityScale = 5;

            //�������Z�𖳌���
            rb.isKinematic = true;

            //�ړ��֘A�̒l���L��
            startPos = tf.position;
            posCorrectionY = 0;
            velocityY = 15;

            //�v���C���[�̓����蔻��E�`�惌�C���[�ύX
            playerCon.SetLayer(5);
            playerCon.SetSortingLayer("Model");
        }

        //�ڂ̈ʒu����
        eyeTf.localPosition = Vector2.Lerp(eyePlayPos, eyeStandbyPos, lerpValue);

        //�ړ��֘A���Z
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //���Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 1.5)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
        //���o���X�L�b�v����Ă��X�e�[�g�J��
        else if (gameStateMachine.currentState == gameStateMachine.state_Result)
            stateMachine.ChangeState(stateMachine.state_Model_Kinematic);
    }

    public override void Exit()
    {
        //�v���C���[�̈ʒu���m�肳����
        tf.position = targetPos;

        //�v���C���[�̃X�P�[�����m�肳����
        tf.localScale = Vector3.one * 1.5f;

        //�v���C���[�̖ڂ̈ʒu���m�肳����
        eyeTf.localPosition = eyeStandbyPos;

        //���x��0��
        rb.linearVelocity = Vector2.zero;

        //��]�𖳌���
        rb.freezeRotation = true;

        //�X�L���`�󂪃X�t�B�A�Ȃ�R���C�_�[�ύX
        if (SkinDataBase.Instance.skinData[GameManager.Instance.usingSkinID].bodyType == SkinData.BodyType.Sphere)
        {
            playerCon.boxCol.enabled = false;
            playerCon.capsuleCol.enabled = true;
        }
        
        //GameState���ɒn�`����p�ҋ@�̕s�v�ʒm
        GameStateState_ResultToPlay.isPlayerCheck = true;
    }
}
