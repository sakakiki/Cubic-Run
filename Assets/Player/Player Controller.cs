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

    public void SetLayer(int layerNum)
    {
        //Colliderのレイヤー変更
        for (int i = 0; i < playerColliders.Length; i++)
            playerColliders[i].layer = layerNum;
    }

    public void SetSortingLayer(string layerName)
    {
        //スキンの描画優先度変更
        for (int i = 0; i < playerSkins.Length; i++)
            playerSkins[i].sortingLayerName = layerName;
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
