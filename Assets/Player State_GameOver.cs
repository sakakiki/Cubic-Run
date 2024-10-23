using UnityEngine;

public class PlayerState_GameOver : PlayerStateBase
{
    public PlayerState_GameOver(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (GameManager.Instance.currentObstacleNum == 3)
            tf.localScale = new Vector2(1, 0.5f);
        else tf.localScale = Vector2.one;
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinGameOver.SetActive(true);
        GameManager.Instance.StopObstacle();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinGameOver.SetActive(false);
    }
}
