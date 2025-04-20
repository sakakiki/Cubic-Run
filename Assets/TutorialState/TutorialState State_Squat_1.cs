public class TutorialStateState_Squat_1 : TutorialStateStateBase
{
    public TutorialStateState_Squat_1(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //障害物の生成
        TM.CreateTerrain(3, TM.stageRightEdge, 6, 1, TM.moveSpeed);

        //再挑戦ステートの更新
        continueState = this;

        //ジャンプアクションの無効化
        PlayerStateBase_Play.isActive_Jump = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (TM.nextTerrainNum == 3)
            stateMachine.ChangeState(stateMachine.state_Squat_2);
    }



    public override void Exit() { }
}
