using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    [SerializeField] private TouchCheck trigerFoot;
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
        stateMachine.Update();
    }
}
