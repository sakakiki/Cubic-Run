using UnityEngine;

public class PlayerState_Model_ResultToMenu : PlayerStateBase_Model
{
    private float startEulerAnglesZ;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isResetRotation;
    private bool isMoving;
    private float posCorrectionY;
    private float velocityY;
    private float gravity = 20;
    private Vector2 eyeStandbyPos;
    private Vector2 eyePlayPos;

    public PlayerState_Model_ResultToMenu(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        targetPos = playerCon.playerPos_Menu;
        eyeStandbyPos = playerCon.eyePos_Model;
        eyePlayPos = playerCon.eyePos_Play;
    }

    public override void Enter()
    {
        //�o�ߎ��Ԃɕ␳�������ă��Z�b�g
        elapsedTime = -0.5f;

        //�t���O���Z�b�g
        isResetRotation = false;
        isMoving = false;

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

            //GameState���ɒn�`���芮���̒ʒm
            GameStateState_ResultToMenu.isPlayerCheck = true;

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

            //�܂����������Ă���Ώ������A�����łȂ���Α傫���W�����v
            rb.linearVelocity = Vector2.up * (Mathf.Abs(startEulerAnglesZ) < 1 ? 7 : 12);
        }

        //���O�v�Z
        float lerpValue = (elapsedTime - 1)/1.5f;

        //��]
        rb.rotation = Mathf.Lerp(startEulerAnglesZ, 0, Mathf.Sqrt(elapsedTime * 2));

        if (elapsedTime < 0.5) return;

        //�X�P�[������
        if (elapsedTime < 1)
            tf.localScale = Vector3.one - Vector3.up * (elapsedTime - 0.5f);
        else tf.localScale = Vector3.one * Mathf.Lerp(1, 1.5f, lerpValue);

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
