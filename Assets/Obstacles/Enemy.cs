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
        //�v���C���łȂ���Ή������Ȃ�
        if (gameStateMachine.currentState != gameStateMachine.state_Play &&
            gameStateMachine.currentState != gameStateMachine.state_Tutorial) return;

        //���s�ς݂Ȃ牽�����Ȃ�
        if (!isActive) return;

        //�v���C���[�ɐڐG����ʒu�܂ŗ����Ƃ�
        if ((tf.position.x) < (playerTf.localScale.x / 2) && tf.position.x > -5)
        {
            //�v���C���[��Attack�X�e�[�g�Ȃ�|�����
            if (playerStateMachine.currentState == playerStateMachine.state_Play_Attack)
            {
                rb.isKinematic = false;
                rb.linearVelocity = parentRb.linearVelocity + Vector2.up * 10;

                //SE�Đ�
                AudioManager.Instance.PlaySE(AudioManager.SE.Enemy);
            }
            //����ȊO�Ȃ�v���C���[��GameOver��
            else playerStateMachine.ChangeState(playerStateMachine.state_GameOver);

            //���s�ς݂ɂ���
            isActive = false;
        } 
    }
}
