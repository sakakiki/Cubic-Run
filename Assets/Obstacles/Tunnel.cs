using UnityEngine;

public class Tunnel : MonoBehaviour
{
    private GameStateStateMachine gameStateMachine;
    private PlayerStateMachine playerStateMachine;
    [SerializeField] private BoxCollider2D boxCol;

    void Start()
    {
        gameStateMachine = GameManager.Instance.gameStateMachine;
        playerStateMachine = GameManager.Instance.playerCon.stateMachine;
    }

    private void OnEnable()
    {
        //壁のColliderを有効化
        boxCol.enabled = true;
    }

    void Update()
    {
        //プレイ中でなければ何もしない
        if (gameStateMachine.currentState != gameStateMachine.state_Play &&
            gameStateMachine.currentState != gameStateMachine.state_Tutorial) return;

        //プレイヤーがGameOverステートなら壁のColliderを無効化
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            boxCol.enabled = false;
    }
}