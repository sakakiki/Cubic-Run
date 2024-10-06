using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    [SerializeField] private TouchCheck trigerFoot;

    public Rigidbody2D rb;
    [NonSerialized] public bool isGrounded;

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
