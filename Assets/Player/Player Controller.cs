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
    public Vector2 playerPos_Skin { get; private set; } = Vector2.right * 5 + Vector2.up * 3;
    public Vector2 eyePos_Model { get; private set; } = Vector2.up * 0.11f;
    public Vector2 eyePos_Play { get; private set; } = Vector2.one * 0.11f;
    public float cubeAngularDrag { get; private set; } = 0.05f;
    public float sphereAngularDrag { get; private set; } = 20;



    private void Start()
    {
        //ステートマシンのインスタンス化・初期化
        stateMachine = new PlayerStateMachine(this);
        stateMachine.Initialize(stateMachine.state_Model_Kinematic);

        //GameStateのステートマシンを登録
        gameStateMachine = GameManager.Instance.gameStateMachine;
    }



    private void Update()
    {
        //ステートに応じたUpdate実行
        stateMachine.Update(); 
    }




    //Colliderのレイヤー変更
    public void SetLayer(int layerNum)
    {
        for (int i = 0; i < playerColliders.Length; i++)
            playerColliders[i].layer = layerNum;
    }



    //描画レイヤー変更
    public void SetSortingLayer(string layerName)
    {
        //スキンの描画優先度変更
        for (int i = 0; i < playerSkins.Length; i++)
            playerSkins[i].sortingLayerName = layerName;

        //マスクの範囲変更
        playerMasks[0].backSortingLayerID = playerSkins[0].sortingLayerID;
        playerMasks[0].frontSortingLayerID = playerSkins[0].sortingLayerID;
    }



    //スキンを変更
    public void ChangeSkin(int skinID)
    {
        //スキン関係の全ての色の変更
        for (int i = 0; i < playerColorParts.Length; i++)
            playerColorParts[i].color = SkinDataBase.Instance.skinData[skinID].skinColor;

        //マスクの有効・無効の切り替え
        for (int i = 0; i < playerMasks.Length; i++)
            playerMasks[i].enabled = SkinDataBase.Instance.skinData[skinID].isEnabledMask;

        //スキンの形状の変更
        //形状がキューブなら
        if (SkinDataBase.Instance.skinData[skinID].bodyType == SkinData.BodyType.Cube)
        {
            //コライダー変更
            boxCol.enabled = true;
            capsuleCol.enabled = false;

            //回転抗力変更
            rb.angularDrag = cubeAngularDrag;

            //描画変更
            for (int i = 0; i < playerShapeParts_sprite.Length; i++)
                playerShapeParts_sprite[i].sprite = squar;
            for (int i = 0; i < playerShapeParts_mask.Length; i++)
                playerShapeParts_mask[i].sprite = squar;
        }
        //形状がスフィアなら
        else
        {
            //コライダー変更
            boxCol.enabled = false;
            capsuleCol.enabled = true;

            //回転抗力変更
            rb.angularDrag = sphereAngularDrag;

            //描画変更
            for (int i = 0; i < playerShapeParts_sprite.Length; i++)
                playerShapeParts_sprite[i].sprite = cicle;
            for (int i = 0; i < playerShapeParts_mask.Length; i++)
                playerShapeParts_mask[i].sprite = cicle;

        }
    }



    //メニュー画面でタップされたらステート遷移
    public void OnMouseDown()
    {
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeState(stateMachine.state_Model_Dragged);
    }

    //メニュー画面で離されたら少し待機して姿勢を戻す
    public void OnMouseUp()
    {
        if (gameStateMachine.currentState == gameStateMachine.state_Menu)
            stateMachine.ChangeStateDelay(stateMachine.state_Model_ResetRotation, 3);
    }
}
