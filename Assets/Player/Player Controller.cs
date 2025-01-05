using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine;
    private GameStateStateMachine gameStateMachine;
    public GameObject player;
    public Transform tf;
    public Rigidbody2D rb;
    public TouchCheck trigerFoot;
    public TouchCheck trigerFront;
    public GameObject SkinDefault;
    public GameObject SkinAttack;
    public GameObject SkinGameOver;
    public GameObject[] playerColliders;
    public SpriteRenderer[] playerSkins;
    public Transform centerTf;
    public Transform eyeTf;
    public GameObject eyeDragged;
    public Vector2 playerPos_Menu { get; private set; } = Vector2.right * 5 + Vector2.up * 2.5f;
    public Vector2 playerPos_GameStart { get; private set; } = Vector2.zero;
    public Vector2 eyePos_Model { get; private set; } = Vector2.up * 0.11f;
    public Vector2 eyePos_Play { get; private set; } = Vector2.one * 0.11f;

    private void Start()
    {
        //�X�e�[�g�}�V���̃C���X�^���X���E������
        stateMachine = new PlayerStateMachine(this);
        stateMachine.Initialize(stateMachine.state_Model_Kinematic);

        //GameState�̃X�e�[�g�}�V����o�^
        gameStateMachine = GameManager.Instance.gameStateMachine;
    }

    private void Update()
    {
        //�X�e�[�g�ɉ�����Update���s
        stateMachine.Update(); 
    }

    public void SetLayer(int layerNum)
    {
        //Collider�̃��C���[�ύX
        for (int i = 0; i < playerColliders.Length; i++)
            playerColliders[i].layer = layerNum;
    }

    public void SetSortingLayer(string layerName)
    {
        //�X�L���̕`��D��x�ύX
        for (int i = 0; i < playerSkins.Length; i++)
            playerSkins[i].sortingLayerName = layerName;
    }

    //���j���[��ʂŃ^�b�v���ꂽ��X�e�[�g�J��
    public void OnMouseDown()
    {
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeState(stateMachine.state_Model_Dragged);
    }

    //���j���[��ʂŗ����ꂽ�班���ҋ@���Ďp����߂�
    public void OnMouseUp()
    {
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeStateDelay(stateMachine.state_Model_ResetRotation, 3);
    }
}
