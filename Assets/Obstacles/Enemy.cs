using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameStateStateMachine gameStateMachine;
    private PlayerStateMachine playerStateMachine;
    [SerializeField] private Transform tf;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D parentRb;
    private Transform playerTf;

    private bool isActive;

    void Start()
    {
        gameStateMachine = GameManager.Instance.gameStateMachine;
        playerStateMachine = GameManager.Instance.playerCon.stateMachine;
        playerTf = GameManager.Instance.playerTf;
    }

    private void OnEnable()
    {
        rb.isKinematic = true;
        transform.localPosition = Vector3.left;
        isActive = true;
    }

    void Update()
    {
        //プレイ中でなければ何もしない
        if (gameStateMachine.currentState != gameStateMachine.state_Play &&
            gameStateMachine.currentState != gameStateMachine.state_Tutorial) return;

        //実行済みなら何もしない
        if (!isActive) return;

        //プレイヤーに接触する位置まで来たとき
        if ((tf.position.x) < (playerTf.localScale.x / 2) && tf.position.x > -5)
        {
            //プレイヤーがAttackステートなら倒される
            if (playerStateMachine.currentState == playerStateMachine.state_Play_Attack)
            {
                rb.isKinematic = false;
                rb.velocity = parentRb.velocity + Vector2.up * 10;

                //SE再生
                AudioManager.Instance.PlaySE(AudioManager.SE.Enemy);
            }
            //それ以外ならプレイヤーをGameOverに
            else playerStateMachine.ChangeState(playerStateMachine.state_GameOver);

            //実行済みにする
            isActive = false;
        } 
    }
}
