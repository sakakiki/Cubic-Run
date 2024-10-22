using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform tf;
    [SerializeField] private Rigidbody2D rb;
    private Transform playerTf;
    private PlayerStateMachine playerStMach;

    private bool isActive;

    void Start()
    {
        playerTf = GameManager.Instance.playerTf;
        playerStMach = GameManager.Instance.playerCon.stateMachine;
        isActive = true;
    }

    private void OnEnable()
    {
        rb.isKinematic = true;
        transform.localPosition = Vector3.left;
    }

    void Update()
    {
        if (!isActive) return;

        if ((tf.position.x) < (playerTf.localScale.x / 2))
        {
            if (playerStMach.currentState == playerStMach.state_Attack)
            {
                rb.isKinematic = false;
                rb.velocity += Vector2.up * 10;
            }
            else
            {

            }

            isActive = false;
        } 
    }
}
