using UnityEngine;

public class GameStateState_CheckStamina : GameStateStateBase
{
    private int remainingStamina;



    public GameStateState_CheckStamina(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public async override void Enter()
    {
        //消費後のスタミナ残量を取得
        remainingStamina = await FirestoreManager.Instance.UseStamina();

        //スタミナ不足通知
        if (remainingStamina == -1)
        {
            //広告視聴回数の取得
            int adWatchCount = await FirestoreManager.Instance.GetAdWatchCount();

            //視聴回数が3回未満なら広告視聴を提案
            if (adWatchCount < 3)
            {
                PopupUIManager.Instance.SetupPopup(
                    "スタミナ不足",
                    "スタミナがありません。\n\n広告を見てスタミナを2回復しますか？\n本日の回復可能回数：" + (3 - adWatchCount),
                    AdmobManager.Instance.LoadAndShowRewardedAd,
                    BackToMenu);
            }

            //広告視聴回数が上限なら回復を待つように通知
            else
            {
                PopupUIManager.Instance.SetupPopupMessage(
                    "スタミナ不足",
                    "スタミナがありません。\n\nスタミナは毎日4:00に全回復します。");
                stateMachine.ChangeState(stateMachine.state_Menu);
            }

            return;
        }

        //エラー通知
        else if (remainingStamina == -2)
        {
            PopupUIManager.Instance.SetupMessageBand("スタミナ情報が取得できませんでした。", 2);
            stateMachine.ChangeState(stateMachine.state_Menu);
            return;
        }

        //超過スタミナ消費の場合の処理
        else if (remainingStamina >= 3)
            GM.UpdateOverStamina(remainingStamina);

        //SE再生
        AM.PlaySE(AM.SE_UseStama);

        //問題なければステート遷移
        stateMachine.ChangeState(stateMachine.state_MenuToPlay);
    }



    public override void Update(float deltaTime) 
    {
        //地形を管理
        TM.ManageMovingTerrain();
    }



    public override void Exit()
    {
        //他ステートに変数の受け渡し
        GameStateState_MenuToPlay.remainingStamina = remainingStamina;
    }



    public void BackToMenu()
    {
        stateMachine.ChangeState(stateMachine.state_Menu);
    }
}
