public class GameStateState_Menu : GameStateStateBase
{
    public GameStateState_Menu(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //メニュー画面のUIを有効化
        IM.InputUISetActive_Menu(true);

        //障害物の生成を有効化
        TM.isCreateObstacle = true;
    }



    public override void Update(float deltaTime)
    {
        //地形を管理
        TM.ManageMovingTerrain();
    }



    public override void Exit()
    {
        //メニュー画面のUIを無効化
        IM.InputUISetActive_Menu(false);
    }
}
