using UnityEngine;

public abstract class PlayerStateBase_Play : PlayerStateBase
{
    protected InputManager IM;
    protected AudioManager AM;
    protected bool isGrounded;
    private TouchCheck trigerFoot;
    private TouchCheck trigerFront;

    public PlayerStateBase_Play(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        IM = InputManager.Instance;
        AM = AudioManager.Instance;
        trigerFoot = stateMachine.playerController.trigerFoot;
        trigerFront = stateMachine.playerController.trigerFront;
    }

    public override void Update()
    {
        //ポーズ状態なら何もしない
        if (GameStateState_Play.currentPauseState != GameStateState_Play.PauseState.Play)
            return;

        //入力取得
        IM.GetInput_Player();

        //接地判定
        isGrounded = trigerFoot.isTouch;

        //ゲームオーバーへの遷移
        if (trigerFront.isTouch || tf.position.y < -5)
            stateMachine.ChangeState(stateMachine.state_GameOver);
        //トレーニングモードクリア状態へ遷移
        else if (gameStateMachine.currentState == gameStateMachine.state_PlayToResult)
            stateMachine.ChangeState(stateMachine.state_Model_TrainingClear);
        //リタイア状態へ遷移
        else if (gameStateMachine.currentState == gameStateMachine.state_PauseToMenu)
            stateMachine.ChangeState(stateMachine.state_Model_PauseToMenu);
    }
}
