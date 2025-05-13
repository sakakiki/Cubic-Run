using UnityEngine;
using System.Collections;

public class LoginStateState_Exit : LoginStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer screenCover;
    private bool isPlayable;

    public LoginStateState_Exit(LoginStateStateMachine stateMachine) : base(stateMachine)
    {
        screenCover = GM.frontScreenCover;
    }

    public override async void Enter()
    {
        //データのロード完了まではゲームを開始しない
        isPlayable = false;

        //経過時間のリセット
        elapsedTime = 0;

        //バージョンチェック
        if (!await FirestoreManager.Instance.CheckVersion())
        {
            MissingVersion();
            return;
        }

        //ログイン後初期処理実行
        await GM.GameInitialize();

        //BGMを流し始める
        AudioManager.Instance.SetBGMPlayVolume(1);
        AudioManager.Instance.PlayBGM(AudioManager.BGM.Menu);

        //ローディングキューブを非表示
        GM.loadingCube.SetActive(false);

        //ゲームプレイ可能
        isPlayable = true;
    }

    public override void Update()
    {
        //ゲームプレイ可能でないなら何もしない
        if (!isPlayable) return;

        //経過時間の加算
        elapsedTime += Time.deltaTime;

        //画面カバーの透明度変更
        screenCover.color = Color.white - Color.black * elapsedTime/2;

        //2秒経過すればステート遷移
        if (elapsedTime > 2)
            gameStateMachine.ChangeState(gameStateMachine.state_Menu);
    }

    public override void Exit() { }

    private void MissingVersion()
    {
        //ローディングキューブを非表示
        GM.loadingCube.SetActive(false);

        //現在のバージョンが最小バージョンに満たない場合は通知
        PopupUIManager.Instance.SetupPopupMessage(
            "バージョン不足",
            "アプリのアップデートが必要です。\n\nストアから最新のバージョンに\nアップデートし、ゲームを\n再起動してください。",
            Enter
        );
    }
}
