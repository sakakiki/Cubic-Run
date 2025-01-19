using UnityEngine;

public abstract class PlayerStateBase_Model : PlayerStateBase
{
    protected float elapsedTime;
    protected bool isGrounded;
    private TouchCheck trigerFoot;
    protected Transform eyeTf;
    protected static float eyeCloseTimer;
    protected static float lookDirection;

    public PlayerStateBase_Model(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        trigerFoot = stateMachine.playerController.trigerFoot;
        eyeTf = stateMachine.playerController.eyeTf;
    }

    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //�ڒn����
        isGrounded = trigerFoot.isTouch;

        //�u���^�C�}�[���Z
        eyeCloseTimer -= Time.deltaTime;

        //�ڂ̃X�P�[������
        if (eyeCloseTimer < 0 )
            eyeTf.localScale = Vector3.one - Vector3.up * (1 - Mathf.Abs(0.1f + eyeCloseTimer));

        //�u�����I����
        if (eyeCloseTimer < -0.2)
        {
            //�ڂ��J����
            eyeTf.localScale = Vector3.one;

            //����u���^�C�~���O�ݒ�
            eyeCloseTimer = Random.Range(0.5f, 5.5f);
        }

        //�Q�[���X�e�[�g���J�ڂ����Ȃ�X�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_MenuToPlay ||
            gameStateMachine.currentState == gameStateMachine.state_ResultToPlay)
            stateMachine.ChangeState(stateMachine.state_Model_MenuToPlay);
    }
}
