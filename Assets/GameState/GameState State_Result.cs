using System;

public class GameStateState_Result : GameStateStateBase
{

    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //プレイヤーとUIの干渉防止壁の有効化
        GM.resultWallCol.enabled = true;

        //プレイヤー移動可能エリアの中心の変更
        GM.centerPos_PlayerArea = GM.centerPos_PlayerArea_Result;

        //広告表示の必要性を確認
        if ((GM.adScore > 10000 || GM.adCount >= 5) && DateTime.Now > AdEnableTimeManager.Load())
        {
            //広告表示
            AdmobManager.Instance.LoadAndShowInterstitial();
        }
        //広告表示が不要ならリザルト画面のUIを有効化
        else
        {
            IM.InputUISetActive_Result(true);
        }
    }



    public override void Update(float deltaTime)
    {
        //入力の取得
        IM.GetInput_Result();

        //入力に応じたステート遷移
        if (IM.is_Result_Title_Push)
            stateMachine.ChangeState(stateMachine.state_ResultToMenu);
        else if (IM.is_Result_Retry_Push)
            stateMachine.ChangeState(stateMachine.state_ResultToPlay);
    }



    public override void Exit()
    {
        //リザルト画面のUIを無効化
        IM.InputUISetActive_Result(false);

        //プレイヤーとUIの干渉防止壁の無効化
        GM.resultWallCol.enabled = false;

        //プレイヤー移動可能エリアの中心の変更
        GM.centerPos_PlayerArea = GM.centerPos_World;
    }
}
