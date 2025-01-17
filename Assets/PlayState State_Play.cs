public class PlayStateState_Play : PlayStateStateBase
{
    public int levelEndScore;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //��Q��������L����
        TM.isCreateObstacle = true;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //��X�R�A�ɓ��B����΃X�e�[�g�J��
        if (GM.score > levelEndScore - scoreCorrection)
        {
            if (GM.isTraining)
                stateMachine.ChangeState(stateMachine.state_TrainingEnd);
            else 
                stateMachine.ChangeState(stateMachine.state_LevelEnd);
        }
    }



    public override void Exit()
    {

    }
}
