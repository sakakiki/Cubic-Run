public class PlayStateState_TrainingEnd: PlayStateStateBase
{
    public PlayStateState_TrainingEnd(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //��Q�������𖳌���
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //��X�R�A�ɓ��B�����
        if (GM.score > 5000)
        {
            //�X�R�A��␳
            GM.score = 5000;
            GM.scoreText.SetText("5000");

            //�X�e�[�g�J��
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        }
    }



    public override void Exit()
    {

    }
}
