using UnityEditor;
using UnityEngine;

public class GameStateState_Play : GameStateStateBase
{
    //プレイ用ステートマシン
    private PlayStateStateMachine playStateMachine;

    //ポーズ処理関係変数
    private int countinueCount;
    private int countinueCircleCount;
    private enum PauseState
    {
        Play,
        Pause,
        PauseToPlay
    }
    private static PauseState currentPauseState;



    public GameStateState_Play(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        playStateMachine = new PlayStateStateMachine(stateMachine);
    }



    public override void Enter()
    {
        //プレイ画面入力UIの有効化
        IM.InputUISetActive_Play(true);
        IM.InputUISetActive_Player(true);

        //プレイ用ステートマシンの初期化
        playStateMachine.Initialize(playStateMachine.state_LevelStart);

        //地形を減速
        TM.moveSpeed = 8;
        TM.SetSpeed(8);

        //ポーズ状態を解除
        InitializePauseState();
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
                    //プレイ画面UIを無効化
                    IM.InputUISetActive_Play(false);
                    IM.InputUISetActive_Player(false);

                    //ポーズ画面UIを表示・有効化
                    GM.PauseUI.SetActive(true);
                    IM.InputUISetActive_Pause(true);

                    //オブジェクトを停止
                    Time.timeScale = 0;

                    //ポーズ状態へ
                    currentPauseState = PauseState.Pause;
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
                    GM.PauseUI.SetActive(false);

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
                    GM.PauseUI.SetActive(false);

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

                        //目標フレームレートを60に設定
                        Application.targetFrameRate = 60;

                        //プレイ画面UIを有効化
                        IM.InputUISetActive_Play(true);
                        IM.InputUISetActive_Player(true);

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
    }

    private static void InitializePauseState()
    {
        //ポーズ状態を解除
        currentPauseState = PauseState.Play;
    }
}
