using UnityEngine;

public class Tunnel : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCol;
    private PlayerStateMachine playerStMach;

    void Start()
    {
        playerStMach = GameManager.Instance.playerCon.stateMachine;
    }

    private void OnEnable()
    {
        boxCol.enabled = true;
    }

    void Update()
    {
        //プレイヤーがGameOverステートなら壁のColliderを無効化
        if (playerStMach.currentState == playerStMach.state_GameOver)
            boxCol.enabled = false;
    }
}