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
        //�v���C���[��GameOver�X�e�[�g�Ȃ�ǂ�Collider�𖳌���
        if (playerStMach.currentState == playerStMach.state_GameOver)
            boxCol.enabled = false;
    }
}