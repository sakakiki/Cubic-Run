using UnityEngine;

public class PlayerState_GameOver : PlayerStateBase
{
    private Vector2 tunnelScale = new Vector2(1.3f, 0.6f);

    public PlayerState_GameOver(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //ゲームステートの遷移
        gameStateMachine.ChangeState(gameStateMachine.state_PlaytoResult);

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
    }

    public override void Update() { }

    public override void Exit()
    {
        //通常スキンに切り替え
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinGameOver.SetActive(false);
    }
}
