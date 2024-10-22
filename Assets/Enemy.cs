using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform tf;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D parentRb;
    private Transform playerTf;
    private PlayerStateMachine playerStMach;

    private bool isActive;

    void Start()
    {
        playerTf = GameManager.Instance.playerTf;
        playerStMach = GameManager.Instance.playerCon.stateMachine;
    }

    private void OnEnable()
    {
        rb.isKinematic = true;
        transform.localPosition = Vector3.left;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        if ((tf.position.x) < (playerTf.localScale.x / 2))
        {
            //プレイヤーがAttackステートなら倒される
            if (playerStMach.currentState == playerStMach.state_Attack)
            {
                rb.isKinematic = false;
                rb.velocity = parentRb.velocity + Vector2.up * 10;
            }
            //それ以外ならプレイヤーをGameOverに
            else
            {
                playerStMach.ChangeState(playerStMach.state_GameOver);
            }

            //実行は1回のみ
            isActive = false;
        } 
    }
}
