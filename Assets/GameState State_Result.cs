public class GameStateState_Result : GameStateStateBase
{
    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //リザルト画面のUIを有効化
        IM.InputUISetActive_Result(true);
    }



    public override void Update(float deltaTime)
    {
        //入力の取得
        IM.GetInput_Result();

        //入力に応じたステート遷移
        //if (IM.is_Menu_Play_Push)
        //    stateMachine.ChangeState(stateMachine.state_MenuToPlay);
    }



    public override void Exit()
    {
        //リザルト画面のUIを無効化
        IM.InputUISetActive_Result(false);
    }
}
