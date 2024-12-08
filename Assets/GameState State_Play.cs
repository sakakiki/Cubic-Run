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
        //�v���C�p�X�e�[�g�}�V���̏�����
        playStateMachine.Initialize(playStateMachine.state_LevelStart);
    }



    public override void Update(float deltaTime)
    {
        //�v���C�p�X�e�[�g�}�V����Update�����s
        playStateMachine.Update(deltaTime);
    }



    public override void Exit() { }
}
