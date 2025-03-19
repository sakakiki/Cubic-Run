public class GameStateState_Menu : GameStateStateBase
{
    public GameStateState_Menu(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //メニュー画面のUIを有効化
        IM.InputUISetActive_Menu(true);

        //障害物の生成を有効化
        TM.isCreateObstacle = true;

        //BGMがメニュー画面のものでなければ変更して再生
        if (audioSource_BGM.clip != AM.BGM_Menu)
        {
            audioSource_BGM.volume = AM.volume_BGM;
            audioSource_BGM.clip = AM.BGM_Menu;
            audioSource_BGM.Play();
        }
    }



    public override void Update(float deltaTime)
    {
        //入力の取得
        IM.GetInput_Menu();

        //地形を管理
        TM.ManageMovingTerrain();

        //入力に応じたステート遷移
        if (IM.is_Menu_Play_Push)
            stateMachine.ChangeState(stateMachine.state_MenuToPlay);
        else if (IM.is_Menu_Skin_Push)
            stateMachine.ChangeState(stateMachine.state_MenuToSkin);
        else if (IM.is_Menu_Account_Push)
            stateMachine.ChangeState(stateMachine.state_Account);
        else if (IM.is_Menu_Volume_Push)
            stateMachine.ChangeState(stateMachine.state_Volume);
        else if (IM.is_Menu_Button_Push)
            stateMachine.ChangeState(stateMachine.state_Button);
        else if (IM.is_Menu_Credit_Push)
            stateMachine.ChangeState(stateMachine.state_Credit);
    }



    public override void Exit()
    {
        //メニュー画面のUIを無効化
        IM.InputUISetActive_Menu(false);
    }
}
