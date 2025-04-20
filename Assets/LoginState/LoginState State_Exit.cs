using UnityEngine;

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

        //ログイン後初期処理実行
        await GM.GameInitialize();

        //BGMを流し始める
        AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Menu;
        AudioManager.Instance.audioSource_BGM.Play();

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
}
