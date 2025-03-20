using UnityEngine;

public class PlayerState_GameOver : PlayerStateBase
{
    private float elapsedTime;
    private Vector2 tunnelScale = new Vector2(1.3f, 0.6f);

    public PlayerState_GameOver(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //経過時間の初期化
        elapsedTime = 0;

        //ゲームステートの遷移
        //通常プレイならリザルトへ
        if (TutorialStateStateBase.continueState == 
            ((GameStateState_Tutorial)gameStateMachine.state_Tutorial).tutorialStateMachine.state_Start)
        gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        //チュートリアルならリトライ
        else
            gameStateMachine.ChangeState(gameStateMachine.state_TutorialRetry);

        //トンネル内なら専用のスケールに
        if (TerrainManager.Instance.currentTerrainNum == 3)
            tf.localScale = tunnelScale;

        //トンネル外かつ画面内なら跳ね返り処理
        else if(tf.position.y > -5)
        {
            tf.localScale = Vector2.one;
            Rigidbody2D playerRb = GameManager.Instance.playerCon.rb;
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.angularVelocity = TerrainManager.Instance.moveSpeed * 20;
            playerRb.velocity = Vector2.up * 15 + Vector2.left * TerrainManager.Instance.moveSpeed * 0.3f;
        }

        //ゲームオーバー用スキンに切り替え
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinGameOver.SetActive(true);

        //スキン形状がスフィアならコライダー変更
        if (SkinDataBase.Instance.skinData[GameManager.Instance.usingSkinID].bodyType == SkinData.BodyType.Sphere)
        {
            playerCon.boxCol.enabled = false;
            playerCon.capsuleCol.enabled = true;
        }

        //SE再生
        AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_GameOver);
    }

    public override void Update()
    {
        //経過時間の加算
        elapsedTime += Time.deltaTime;

        //ゲームステートの遷移に合わせてステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_ResultToMenu)
            stateMachine.ChangeState(stateMachine.state_Model_ResultToMenu);
        else if (gameStateMachine.currentState == gameStateMachine.state_ResultToPlay ||
                 gameStateMachine.currentState == gameStateMachine.state_TutorialRetry && elapsedTime > 1)
            stateMachine.ChangeState(stateMachine.state_Model_ResultToPlay);
    }

    public override void Exit()
    {
        //通常スキンに切り替え
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinGameOver.SetActive(false);
    }
}
