using UnityEngine;
using System;

public class GameStateState_Result : GameStateStateBase
{
    private float elapsedTime;
    public static bool isAdNeeded; // 広告表示の必要性を示すフラグ

    public GameStateState_Result(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //フラグリセット
        isAdNeeded = false;

        //プレイヤーとUIの干渉防止壁の有効化
        GM.resultWallCol.enabled = true;

        //プレイヤー移動可能エリアの中心の変更
        GM.centerPos_PlayerArea = GM.centerPos_PlayerArea_Result;

        //広告表示の必要性を確認
        if ((GM.adScore > 10000 || GM.adCount >= 5) && DateTime.Now > AdEnableTimeManager.Load())
        {
            //広告表示が必要な場合、フラグを設定
            isAdNeeded = true;

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
        //経過時間加算
        elapsedTime += deltaTime;

        if (isAdNeeded)
        {
            if (0.5f < elapsedTime && elapsedTime < 1.0f)
            {
                //ローディングキューブを表示
                GM.loadingCube.SetActive(true);
            }

            //ローディングキューブの回転
            GM.loadingCube.transform.Rotate(Vector3.up, 180 * deltaTime);

            //5秒経過で広告表示を諦める
            if (elapsedTime > 5)
            {
                //広告再生リクエストを取り下げ
                isAdNeeded = false;

                //ローディングキューブを非表示
                GM.loadingCube.SetActive(false);

                //リザルト画面のUIを有効化
                IM.InputUISetActive_Result(true);
            }
        }

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

        //ローディングキューブを非表示（本来は不要だが、バグ対策）
        if (GM.loadingCube.activeSelf)
            GM.loadingCube.SetActive(false);
    }
}
