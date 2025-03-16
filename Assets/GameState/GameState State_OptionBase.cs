using UnityEngine;

public abstract class GameStateState_OptionBase : GameStateStateBase
{
    public GameStateState_OptionBase(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //オプション画面の入力を有効化
        IM.InputUISetActive_Option(true);

        //オプション画面のUIを有効化
        GameManager.Instance.optionUIBase.SetActive(true);

        //無効部分にカバー
        GameManager.Instance.frontScreenCover.color = Color.white - Color.black * 0.2f;
    }



    public override void Update(float deltaTime)
    {
        //入力の取得
        IM.GetInput_Option();

        //地形を管理
        TM.ManageMovingTerrain();

        //入力に応じたステート遷移
        if (IM.is_Option_Close_Tap)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit()
    {
        //オプション画面の入力を無効化
        IM.InputUISetActive_Option(false);

        //オプション画面のUIを無効化
        GameManager.Instance.optionUIBase.SetActive(false);

        //カバーを透明に
        GameManager.Instance.frontScreenCover.color = Color.clear;

        //SEの再生
        AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Close);
    }
}
