using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine;
    public TouchCheck trigerFoot;
    public TouchCheck trigerFront;
    public Transform tf;
    public Rigidbody2D rb;
    public GameObject SkinDefault;
    public GameObject SkinAttack;
    public GameObject SkinGameOver;

    void Start()
    {
        stateMachine = new PlayerStateMachine(this);
        stateMachine.Initialize(stateMachine.state_Run);
    }

    private void Update()
    {
        //ステートに応じたUpdate実行
        stateMachine.Update();
    }
}
