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
    public BoxCollider2D boxCol;
    public CapsuleCollider2D capsuleCol;
    public GameObject SkinDefault;
    public GameObject SkinAttack;
    public GameObject SkinGameOver;
    public GameObject[] playerColliders;
    public SpriteRenderer[] playerSkins;
    public SpriteRenderer[] playerColorParts;
    public Sprite squar;
    public Sprite cicle;
    public SpriteRenderer[] playerShapeParts_sprite;
    public SpriteMask[] playerShapeParts_mask;
    public SpriteMask[] playerMasks;
    public Transform centerTf;
    public Transform eyeTf;
    public GameObject eyeDragged;
    public Vector2 playerPos_Menu { get; private set; } = Vector2.right * 5 + Vector2.up * 2.5f;
    public Vector2 playerPos_GameStart { get; private set; } = Vector2.zero;
    public Vector2 playerPos_TrainingClear { get; private set; } = Vector2.up * 3 + Vector2.left * 2;
    public Vector2 eyePos_Model { get; private set; } = Vector2.up * 0.11f;
    public Vector2 eyePos_Play { get; private set; } = Vector2.one * 0.11f;
    public float cubeAngularDrag { get; private set; } = 0.05f;
    public float sphereAngularDrag { get; private set; } = 20;



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




    //Collider�̃��C���[�ύX
    public void SetLayer(int layerNum)
    {
        for (int i = 0; i < playerColliders.Length; i++)
            playerColliders[i].layer = layerNum;
    }



    //�`�惌�C���[�ύX
    public void SetSortingLayer(string layerName)
    {
        //�X�L���̕`��D��x�ύX
        for (int i = 0; i < playerSkins.Length; i++)
            playerSkins[i].sortingLayerName = layerName;

        //�}�X�N�͈͕̔ύX
        playerMasks[0].backSortingLayerID = playerSkins[0].sortingLayerID;
        playerMasks[0].frontSortingLayerID = playerSkins[0].sortingLayerID;
    }



    //�X�L����ύX
    public void ChangeSkin(int skinID)
    {
        //�X�L���֌W�̑S�Ă̐F�̕ύX
        for (int i = 0; i < playerColorParts.Length; i++)
            playerColorParts[i].color = SkinDataBase.Instance.skinData[skinID].skinColor;

        //�}�X�N�̗L���E�����̐؂�ւ�
        for (int i = 0; i < playerMasks.Length; i++)
            playerMasks[i].enabled = SkinDataBase.Instance.skinData[skinID].isEnabledMask;

        //�X�L���̌`��̕ύX
        //�`�󂪃L���[�u�Ȃ�
        if (SkinDataBase.Instance.skinData[skinID].bodyType == SkinData.BodyType.Cube)
        {
            //�R���C�_�[�ύX
            boxCol.enabled = true;
            capsuleCol.enabled = false;

            //��]�R�͕ύX
            rb.angularDamping = cubeAngularDrag;

            //�`��ύX
            for (int i = 0; i < playerShapeParts_sprite.Length; i++)
                playerShapeParts_sprite[i].sprite = squar;
            for (int i = 0; i < playerShapeParts_mask.Length; i++)
                playerShapeParts_mask[i].sprite = squar;
        }
        //�`�󂪃X�t�B�A�Ȃ�
        else
        {
            //�R���C�_�[�ύX
            boxCol.enabled = false;
            capsuleCol.enabled = true;

            //��]�R�͕ύX
            rb.angularDamping = sphereAngularDrag;

            //�`��ύX
            for (int i = 0; i < playerShapeParts_sprite.Length; i++)
                playerShapeParts_sprite[i].sprite = cicle;
            for (int i = 0; i < playerShapeParts_mask.Length; i++)
                playerShapeParts_mask[i].sprite = cicle;

        }
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
