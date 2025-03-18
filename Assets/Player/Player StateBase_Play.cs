using UnityEngine;

public abstract class PlayerStateBase_Play : PlayerStateBase
{
    protected InputManager IM;
    protected AudioManager AM;
    protected bool isGrounded;
    private TouchCheck trigerFoot;
    private TouchCheck trigerFront;

    public PlayerStateBase_Play(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        IM = InputManager.Instance;
        AM = AudioManager.Instance;
        trigerFoot = stateMachine.playerController.trigerFoot;
        trigerFront = stateMachine.playerController.trigerFront;
    }

    public override void Update()
    {
        //�|�[�Y��ԂȂ牽�����Ȃ�
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //���͎擾
        IM.GetInput_Player();

        //�ڒn����
        isGrounded = trigerFoot.isTouch;

        //�Q�[���I�[�o�[�ւ̑J��
        if (trigerFront.isTouch || tf.position.y < -5)
            stateMachine.ChangeState(stateMachine.state_GameOver);
        //�g���[�j���O���[�h�N���A��Ԃ֑J��
        else if (gameStateMachine.currentState == gameStateMachine.state_PlayToResult)
            stateMachine.ChangeState(stateMachine.state_Model_TrainingClear);
        //���^�C�A��Ԃ֑J��
        else if (gameStateMachine.currentState == gameStateMachine.state_PauseToMenu)
            stateMachine.ChangeState(stateMachine.state_Model_PauseToMenu);
    }
}
