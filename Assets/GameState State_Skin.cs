using UnityEngine;

public class GameStateState_Skin : GameStateStateBase
{
    private SkinSelecter skinSelecter;

    public GameStateState_Skin(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        skinSelecter = GM.skinSelecter;
    }



    public override void Enter()
    {
        //スキン選択画面のUIを有効化
        IM.InputUISetActive_Skin(true);

        //スキンセレクターの有効化
        GameManager.Instance.skinSelecter.isActive = true;
    }



    public override void Update(float deltaTime)
    {
        //入力の取得
        IM.GetInput_Skin();

        //地形を管理
        TM.ManageMovingTerrain();

        //入力がありセレクターのホイールが停止していればステート遷移
        if (IM.is_Skin_OK_Push || IM.is_Skin_Cancel_Push)
            if (skinSelecter.isStop)
                stateMachine.ChangeState(stateMachine.state_SkinToMenu);
    }



    public override void Exit()
    {
        //スキン選択画面のUIを無効化
        IM.InputUISetActive_Skin(false);

        //スキンセレクタースクリプトを一時的に無効化（SkinToMenuステートにて再有効化）
        skinSelecter.enabled = false;
    }
}
