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
        //�ǂ�Collider��L����
        boxCol.enabled = true;
    }

    void Update()
    {
        //�v���C���łȂ���Ή������Ȃ�
        if (gameStateMachine.currentState != gameStateMachine.state_Play &&
            gameStateMachine.currentState != gameStateMachine.state_Tutorial) return;

        //�v���C���[��GameOver�X�e�[�g�Ȃ�ǂ�Collider�𖳌���
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            boxCol.enabled = false;
    }
}