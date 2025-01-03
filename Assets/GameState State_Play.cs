public class GameStateState_Play : GameStateStateBase
{
    //�v���C�p�X�e�[�g�}�V��
    private PlayStateStateMachine playStateMachine;



    public GameStateState_Play(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        playStateMachine = new PlayStateStateMachine(stateMachine);
    }



    public override void Enter()
    {
        //�v���C��ʓ���UI�̗L����
        IM.InputUISetActive_Play(true);

        //�v���C�p�X�e�[�g�}�V���̏�����
        playStateMachine.Initialize(playStateMachine.state_LevelStart);

        //�n�`������
        TM.moveSpeed = 8;
        TM.SetSpeed(8);

        //�|�[�Y��Ԃ�����
        PlayStateStateBase.InitializePauseState();
    }



    public override void Update(float deltaTime)
    {
        //�v���C�p�X�e�[�g�}�V����Update�����s
        playStateMachine.Update(deltaTime);
    }



    public override void Exit()
    {
        //�v���C��ʓ���UI�̖�����
        IM.InputUISetActive_Play(false);
    }
}
