using UnityEngine;

public class GameStateState_Tutorial : GameStateStateBase
{
    //チュートリアル用ステートマシン
    public TutorialStateStateMachine tutorialStateMachine { private set; get; }

    private static float playTimeScale;



    public enum PauseState
    {
        Play,
        Pause,
    }
    public static PauseState currentPauseState { get; private set; }



    public GameStateState_Tutorial(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        tutorialStateMachine = new TutorialStateStateMachine(stateMachine);

        //ポーズ状態を解除
        InitializePauseState();
    }



    public override void Enter()
    {
        //プレイ画面入力UIの有効化
        IM.InputUISetActive_Play(true);
        IM.InputUISetActive_Player(true);
        IM.isPauseGame = false;

        //プレイ用ステートマシンの設定
        tutorialStateMachine.Initialize(TutorialStateStateBase.continueState);

        //プレイヤーのチュートリアル用初期化
        if (tutorialStateMachine.currentState == tutorialStateMachine.state_Start)
            PlayerStateBase_Play.InitializeTutorial();

        //地形を減速
        TM.moveSpeed = 8;
        TM.SetSpeed(8);

        //プレイ用BGMが再生されていなければの再生
        if (audioSource_BGM.clip != AM.BGM_Play_1)
        {
            audioSource_BGM.volume = AM.volume_BGM;
            audioSource_BGM.clip = AM.BGM_Play_1;
            audioSource_BGM.Play();
            AM.SetBGMSpeed(0.9f);
        }
    }



    public override void Update(float deltaTime)
    {
        switch (currentPauseState)
        {
            case PauseState.Play:

                //チュートリアル用ステートマシンのUpdateを実行
                tutorialStateMachine.Update(deltaTime);

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

                    //プレイ画面UIを有効化
                    IM.InputUISetActive_Play(true);
                    IM.isPauseGame = false;

                    //オブジェクトを動かす
                    Time.timeScale = playTimeScale;

                    //ポーズ状態を終了
                    currentPauseState = PauseState.Play;
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

        //操作方法非表示
        PopupUIManager.Instance.DeleteMessageText();

        //ボタン表示を戻す
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.clear;

        //チュートリアル用ステートマシンのExitを実行
        tutorialStateMachine.currentState.Exit();
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

        //TimeScaleを保存
        playTimeScale = Time.timeScale;

        //オブジェクトを停止
        Time.timeScale = 0;

        //ポーズ状態へ
        currentPauseState = PauseState.Pause;
    }
}
