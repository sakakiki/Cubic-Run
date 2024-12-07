using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine;
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
    public Vector2 playerPos_GameStart { get; private set; } = Vector2.zero;
    public Vector2 eyePos_Model { get; private set; } = Vector2.up * 0.11f;
    public Vector2 eyePos_Play { get; private set; } = Vector2.one * 0.11f;

    void Start()
    {
        stateMachine = new PlayerStateMachine(this);
        stateMachine.Initialize(stateMachine.state_Model_Kinematic);
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

    //�^�b�v���ꂽ��X�e�[�g�J��
    public void OnMouseDown()
    {
        stateMachine.ChangeState(stateMachine.state_Model_Dragged);
    }

    //�����ꂽ�班���ҋ@���Ďp����߂�
    public void OnMouseUp()
    {
        stateMachine.ChangeStateDelay(stateMachine.state_Model_ResetRotation, 3);
    }
}
