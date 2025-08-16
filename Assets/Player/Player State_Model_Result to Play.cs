using UnityEngine;

public class PlayerState_Model_ResultToPlay : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isResetRotation;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 20;
    private Vector2 eyeStartPos;
    private Vector2 eyePlayPos;
    private float startScale;
    private float targetScale = 1;

    public PlayerState_Model_ResultToPlay(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_GameStart;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        //�o�ߎ��Ԃɕ␳�������ă��Z�b�g
        elapsedTime = -0.5f;

        //�t���O���Z�b�g
        isResetRotation = false;
        isMoving = false;

        //�ڂ̈ʒu���L��
        eyeStartPos = eyeTf.localPosition;

        //�J�n���̑傫�����L��
        startScale = tf.localScale.x;

        //�v���C���[����ʊO�Ȃ牉�o�𑁂߂�
        if (playerCon.tf.position.y < -5 || playerCon.tf.position.x < -6)
            elapsedTime = 1;
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //0.5�b�ҋ@
        if (elapsedTime < 0) return;

        //�p���␳�J�n����
        if (!isResetRotation && elapsedTime < 1)
        {
            isResetRotation = true;

            //�v���C���[���g���l�����Ȃ牉�o�𑁂߂�
            if (TerrainManager.Instance.currentTerrainNum == 3)
                elapsedTime = 1;
            //�g���l���O�Ȃ珉����]�ʋL���E�␳
            else
            {
                startEulerAnglesZ = tf.eulerAngles.z;
                if (tf.eulerAngles.z > 180)
                    startEulerAnglesZ -= 360;
            }

            //�d�̓X�P�[���␳
            rb.gravityScale = 5;

            //�������Z�̗L����
            rb.isKinematic = false;

            //�܂����������Ă���Ώ������A�����łȂ���Α傫���W�����v
            rb.linearVelocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
        }

        //���O�v�Z
        float lerpValue = (elapsedTime - 1)/1.5f;

        //��]
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        if (elapsedTime < 0.5) return;

        //1.5�b�o�߂܂ł�
        if (elapsedTime < 1)
        {
            //�X�P�[������
            tf.localScale = (Vector3.one - Vector3.up * (elapsedTime - 0.5f)) * startScale;

            //�c��̏������΂�
            return;
        }

        //�W�����v�J�n����
        if (!isMoving)
        {
            //�������s�ς݂̃t���O�𗧂Ă�
            isMoving = true;

            //GameState���ɒn�`���芮���̒ʒm
            GameStateState_ResultToPlay.isPlayerCheck = true;

            //�������Z�𖳌���
            rb.isKinematic = true;

            //�X�P�[������
            tf.localScale = Vector3.one;

            //�ړ��֘A�̒l���L��
            startPos = tf.position;
            posCorrectionY = 0;
            velocityY = 15;
        }

        //�ڂ̈ʒu����
        eyeTf.localPosition = Vector2.Lerp(eyeStartPos, eyePlayPos, lerpValue);

        //�X�P�[������
        tf.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, lerpValue);

        //�ړ��֘A���Z
        velocityY -= gravity * Time.deltaTime;
        posCorrectionY += velocityY * Time.deltaTime;
        tf.position = Vector2.Lerp(startPos, targetPos, lerpValue) + Vector2.up * posCorrectionY;

        //�Q�[���X�e�[�g��Play�Ȃ�X�e�[�g�J��
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

        //�v���C���[�̃X�P�[�����m�肳����
        tf.localScale = Vector3.one;

        //���x��0��
        rb.linearVelocity = Vector2.zero;

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
