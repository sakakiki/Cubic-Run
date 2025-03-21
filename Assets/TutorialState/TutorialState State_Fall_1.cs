public class TutorialStateState_Fall_1 : TutorialStateStateBase
{
    public TutorialStateState_Fall_1(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //��Q���̐���
        TM.CreateTerrain(6, TM.stageRightEdge, 5, 1, TM.moveSpeed);
        TM.CreateTerrain(0, TM.stageRightEdge + 5, 1.5f, 1, TM.moveSpeed);
        TM.CreateTerrain(1, TM.stageRightEdge + 6.5f, 2, 1, TM.moveSpeed);
        TM.CreateTerrain(0, TM.stageRightEdge + 8.5f, 2.5f, 1, TM.moveSpeed);
        TM.CreateTerrain(6, TM.stageRightEdge + 11, 5, 1, TM.moveSpeed);

        //�Ē���X�e�[�g�̍X�V
        continueState = this;

        //�W�����v�A�N�V�����̖�����
        PlayerStateBase_Play.isActive_Jump = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (TM.nextTerrainNum == 1)
            stateMachine.ChangeState(stateMachine.state_Fall_2);
    }



    public override void Exit() { }
}
