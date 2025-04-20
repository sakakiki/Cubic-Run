using UnityEngine;

public class GameStateState_Play : GameStateStateBase
{
    //プレイ用ステートマシン
    private PlayStateStateMachine playStateMachine;

    //ポーズ処理関係変数
    private int countinueCount;
    private int countinueCircleCount;
    public enum PauseState
    {
        Play,
        Pause,
        PauseToPlay
    }
    public static PauseState currentPauseState { get; private set; }



    public GameStateState_Play(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        playStateMachine = new PlayStateStateMachine(stateMachine);

        //ポーズ状態を解除
        InitializePauseState();
    }



    public override void Enter()
    {
        //プレイ画面入力UIの有効化
        IM.InputUISetActive_Play(true);
        IM.InputUISetActive_Player(true);
        IM.isPauseGame = false;

        //プレイ用ステートマシンの初期化
        playStateMachine.Initialize(playStateMachine.state_LevelStart);

        //プレイヤーの全アクションの有効化
        PlayerStateBase_Play.EnableAllAction();

        //地形を減速
        TM.moveSpeed = 5 + Mathf.Pow(GM.level, 0.7f) * 3;
        TM.SetSpeed(5 + Mathf.Pow(GM.level, 0.7f) * 3);

        //プレイ用BGMの再生
        audioSource_BGM.volume = AM.volume_BGM;
    }



    public override void Update(float deltaTime)
    {
        switch (currentPauseState)
        {
            case PauseState.Play:

                //プレイ用ステートマシンのUpdateを実行
                playStateMachine.Update(deltaTime);

                //プレイ画面UIの入力の取得
                IM.GetInput_Play();

                //ポーズ
                if (IM.is_Play_Pause_Tap)
                {
                    //SEの再生
                    AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Panel);

                    //ポーズ状態へ
                    EnterPause();
                }
                break;


            case PauseState.Pause:

                //ポーズ画面入力の取得
                IM.GetInput_Pause();

                //入力に応じた遷移
                if (IM.is_Pause_Continue_Push)
                {
                    //ポーズ画面UIを非表示・無効化
                    IM.InputUISetActive_Pause(false);
                    GM.pauseUI.SetActive(false);

                    //目標フレームレートを30に設定
                    Application.targetFrameRate = 30;

                    //再開までのカウントダウンを3に
                    countinueCount = 3;
                    GM.countinueCountText.SetText("" + countinueCount);

                    //カウントサークルの表示
                    countinueCircleCount = 30;
                    for (int i = 0; i < GM.countinueCircleSquares.Length; i++)
                        GM.countinueCircleSquares[i].SetActive(true);

                    //再開のカウントダウンに移行
                    currentPauseState = PauseState.PauseToPlay;
                }
                else if (IM.is_Pause_Retire_Push)
                {
                    //ポーズ画面UIを非表示・無効化
                    IM.InputUISetActive_Pause(false);
                    GM.pauseUI.SetActive(false);

                    //オブジェクトを動かす
                    Time.timeScale = 1;

                    //メニュー画面へ遷移
                    stateMachine.ChangeState(stateMachine.state_PauseToMenu);
                }
                break;


            case PauseState.PauseToPlay:

                //カウントサークルの数を減らす
                countinueCircleCount--;

                //まだカウントサークルがあるなら非表示に
                if (countinueCircleCount >= 0)
                    GM.countinueCircleSquares[countinueCircleCount].SetActive(false);
                //カウントサークルが無いなら
                else
                {
                    //再開のカウントダウンを減らす
                    countinueCount--;

                    //カウントダウンが続くなら
                    if (countinueCount > 0)
                    {
                        //再開のカウントダウンを更新
                        GM.countinueCountText.SetText("" + countinueCount);

                        //カウントサークルの再表示
                        countinueCircleCount = 30;
                        for (int i = 0; i < GM.countinueCircleSquares.Length; i++)
                            GM.countinueCircleSquares[i].SetActive(true);
                    }
                    //カウントダウンが終わるなら
                    else
                    {
                        //再開のカウントダウンを非表示
                        GM.countinueCountText.SetText("");

                        //目標フレームレートを戻す
                        Application.targetFrameRate = GM.defaultFrameRate;

                        //プレイ画面UIを有効化
                        IM.InputUISetActive_Play(true);
                        IM.isPauseGame = false;

                        //オブジェクトを動かす
                        Time.timeScale = 1;

                        //ポーズ状態を終了
                        currentPauseState = PauseState.Play;
                    }
                }
                break;


            default: break;
        }
    }



    public override void Exit()
    {
        //プレイ画面入力UIの無効化
        IM.InputUISetActive_Play(false);
        IM.InputUISetActive_Player(false);

        //ポーズ状態を解除
        InitializePauseState();
    }



    private static void InitializePauseState()
    {
        //ポーズ状態を解除
        currentPauseState = PauseState.Play;
    }



    public static void EnterPause()
    {
        //プレイ画面UIを無効化
        InputManager.Instance.InputUISetActive_Play(false);
        InputManager.Instance.isPauseGame = true;

        //ポーズ画面UIを表示・有効化
        GameManager.Instance.pauseUI.SetActive(true);
        InputManager.Instance.InputUISetActive_Pause(true);

        //オブジェクトを停止
        Time.timeScale = 0;

        //再開のカウントダウンを非表示
        GameManager.Instance.countinueCountText.SetText("");

        //カウントサークルを非表示
        for (int i = 0; i < GameManager.Instance.countinueCircleSquares.Length; i++)
            GameManager.Instance.countinueCircleSquares[i].SetActive(false);

        //ポーズ状態へ
        currentPauseState = PauseState.Pause;
    }
}
