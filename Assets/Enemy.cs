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
        //���s�ς݂Ȃ牽�����Ȃ�
        if (!isActive) return;

        //�v���C���[�ɐڐG����ʒu�܂ŗ����Ƃ�
        if ((tf.position.x) < (playerTf.localScale.x / 2))
        {
            //�v���C���[��Attack�X�e�[�g�Ȃ�|�����
            if (playerStateMachine.currentState == playerStateMachine.state_Attack)
            {
                rb.isKinematic = false;
                rb.velocity = parentRb.velocity + Vector2.up * 10;
            }
            //����ȊO�Ȃ�v���C���[��GameOver��
            else
            {
                playerStateMachine.ChangeState(playerStateMachine.state_GameOver);
            }

            //���s�ς݂ɂ���
            isActive = false;
        } 
    }
}
