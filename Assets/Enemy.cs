using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform tf;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D parentRb;
    private Transform playerTf;
    private PlayerStateMachine playerStateMachine;

    private bool isActive;

    void Start()
    {
        playerTf = GameManager.Instance.playerTf;
        playerStateMachine = GameManager.Instance.playerCon.stateMachine;
    }

    private void OnEnable()
    {
        rb.isKinematic = true;
        transform.localPosition = Vector3.left;
        isActive = true;
    }

    void Update()
    {
        //実行済みなら何もしない
        if (!isActive) return;

        //プレイヤーに接触する位置まで来たとき
        if ((tf.position.x) < (playerTf.localScale.x / 2))
        {
            //プレイヤーがAttackステートなら倒される
            if (playerStateMachine.currentState == playerStateMachine.state_Attack)
            {
                rb.isKinematic = false;
                rb.velocity = parentRb.velocity + Vector2.up * 10;
            }
            //それ以外ならプレイヤーをGameOverに
            else
            {
                playerStateMachine.ChangeState(playerStateMachine.state_GameOver);
            }

            //実行済みにする
            isActive = false;
        } 
    }
}
