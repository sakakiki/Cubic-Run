using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine;
    [SerializeField] private TouchCheck trigerFoot;
    [SerializeField] private TouchCheck trigerFront;
    public Transform tf;
    public Rigidbody2D rb;
    [NonSerialized] public bool isGrounded;
    public GameObject SkinDefault;
    public GameObject SkinAttack;
    public GameObject SkinGameOver;

    void Start()
    {
        stateMachine = new PlayerStateMachine(this);
        stateMachine.Initialize(stateMachine.state_Run);
    }

    void Update()
    {
        InputManager.Instance.GetInput();

        isGrounded = trigerFoot.isTouch;

        if (trigerFront.isTouch && stateMachine.currentState != stateMachine.state_GameOver) 
            stateMachine.ChangeState(stateMachine.state_GameOver);

        stateMachine.Update();
    }
}
