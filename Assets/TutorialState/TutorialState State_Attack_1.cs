public class TutorialStateState_Attack_1 : TutorialStateStateBase
{
    public TutorialStateState_Attack_1(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //障害物の生成
        for (int i = 0; i < 5; i++) 
            TM.CreateTerrain(4, TM.stageRightEdge + i, 1, 1, TM.moveSpeed);

        //再挑戦ステートの更新
        continueState = this;

        //ジャンプアクションの無効化
        PlayerStateBase_Play.isActive_Jump = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (TM.nextTerrainNum == 4)
            stateMachine.ChangeState(stateMachine.state_Attack_2);
    }



    public override void Exit() { }
}
