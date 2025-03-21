public class TutorialStateState_SmallJump_1 : TutorialStateStateBase
{
    public TutorialStateState_SmallJump_1(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //��Q���̐���
        TM.CreateTerrain(5, TM.stageRightEdge, 6, 1, TM.moveSpeed);

        //�Ē���X�e�[�g�̍X�V
        continueState = this;

        //�W�����v�A�N�V�����̖�����
        PlayerStateBase_Play.isActive_Jump = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (TM.nextTerrainNum == 5)
            stateMachine.ChangeState(stateMachine.state_SmallJump_2);
    }



    public override void Exit() { }
}
