using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelScript : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject Eye;


    #region モデルステート
    public enum ModelState
    {
        Idle,
        LookAround,
        Move,
        Roll,
        Reset,
        Stop
    }

    [System.NonSerialized] public static ModelState currentModelState;
    private bool modelStateEnter = true;

    private void ChangeModelState(ModelState newModelState)
    {
        currentModelState = newModelState;
        modelStateEnter = true;
    }
    #endregion


    private bool isClick;
    private Rigidbody2D rb;
    private RectTransform eyeRectTransform;
    [System.NonSerialized] public float moveStartTiming;
    private Vector2 lookDirection;
    private Vector2 moveVector;
    private float rollSpeed;
    private float rollStartTiming = 0;
    private float rollEndTiming;
    private bool isRolling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        eyeRectTransform = Eye.GetComponent<RectTransform>();
    }

    private void Update()
    {
        switch (currentModelState)
        {
            case ModelState.Idle:
                if (modelStateEnter)
                {
                    modelStateEnter = false;
                    isClick = false;
                    eyeRectTransform.anchoredPosition = Vector2.zero;
                    moveStartTiming = Time.time + 5;
                    transform.eulerAngles = Vector3.zero;
                    rb.freezeRotation = true;
                }
                if(moveStartTiming < Time.time)
                {
                    lookDirection = (UnityEngine.Random.Range(0, 2) == 0) ? Vector2.right : Vector2.left;
                    ChangeModelState(ModelState.LookAround);
                }
                break;



            case ModelState.LookAround:
                if (modelStateEnter)
                {
                    modelStateEnter = false;
                    moveStartTiming = Time.time + 1;
                }

                if (moveStartTiming < Time.time)
                {
                    if (rb.velocity.y == 0)
                    {
                        eyeRectTransform.anchoredPosition += lookDirection;

                        if (eyeRectTransform.anchoredPosition.x == 0 && GetComponent<RectTransform>().anchoredPosition.x != 0)
                        {
                            if (UnityEngine.Random.Range(0, 4) == 0)
                                ChangeModelState(ModelState.Roll);
                        }

                        if (Math.Abs(eyeRectTransform.anchoredPosition.x) > 10)
                        {
                            lookDirection *= -1;
                            moveStartTiming = Time.time + UnityEngine.Random.Range(0.5f, 1.5f);
                            if (Math.Sign(eyeRectTransform.anchoredPosition.x) != Math.Sign(GetComponent<RectTransform>().anchoredPosition.x))
                            {
                                if (UnityEngine.Random.Range(0, 2) == 0)
                                    ChangeModelState(ModelState.Move);
                            }
                        }
                    }
                }

                break;



            case ModelState.Move:
                if (modelStateEnter)
                {
                    modelStateEnter = false;
                    moveStartTiming = Time.time + 0.5f;
                    moveVector = Vector2.up * 8 + Vector2.right * Math.Sign(eyeRectTransform.anchoredPosition.x) * 3;
                }

                if(moveStartTiming < Time.time && rb.velocity.y == 0)
                {
                    if (rb.isKinematic) rb.isKinematic = false;
                    rb.velocity = moveVector;
                    if (Math.Sign(eyeRectTransform.anchoredPosition.x) == Math.Sign(GetComponent<RectTransform>().anchoredPosition.x))
                    {
                        if (UnityEngine.Random.Range(0, 6) == 0)
                        {
                            ChangeModelState(ModelState.LookAround);
                        }
                    }
                }
                break;



            case ModelState.Roll:
                if (modelStateEnter)
                {
                    modelStateEnter = false;
                    rb.freezeRotation = false;
                    moveStartTiming = Time.time + 0.5f;
                    rollStartTiming = Time.time + 1;
                    rollEndTiming = rollStartTiming + UnityEngine.Random.Range(1.5f, 4.5f);
                    rollSpeed = Math.Sign(GetComponent<RectTransform>().anchoredPosition.x) * 300; 
                    isRolling = false;
                }

                if (Time.time < moveStartTiming) { }
                else if (Time.time < rollStartTiming)
                {
                    if (!isRolling)
                    {
                        isRolling = true;
                        rb.angularVelocity = -rollSpeed * 3;
                    }
                }
                else if (Time.time < rollEndTiming)
                    rb.angularVelocity = rollSpeed;
                else if (rollEndTiming + 1 < Time.time)
                    ChangeModelState(ModelState.Reset);

                break;



            case ModelState.Reset:
                if (modelStateEnter)
                {
                    modelStateEnter = false;
                    rb.angularVelocity = 0;
                    eyeRectTransform.anchoredPosition = Vector2.zero;
                    if (transform.rotation.eulerAngles.z > 5 && transform.rotation.eulerAngles.z < 355)
                        rb.velocity = Vector2.up * 10;
                }

                if (transform.rotation.eulerAngles.z == 180) transform.eulerAngles += Vector3.forward * 10;
                else if (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270)
                    transform.eulerAngles += Vector3.forward * Math.Sign(transform.rotation.eulerAngles.z - 180) * 10;
                else if (transform.rotation.eulerAngles.z > 5 && transform.rotation.eulerAngles.z < 355)
                    transform.eulerAngles += Vector3.forward * Math.Sign(transform.rotation.eulerAngles.z - 180) * 4.5f;
                else
                {
                    transform.eulerAngles = Vector3.zero;
                    rb.freezeRotation = true;
                    ChangeModelState((GameManager.Instance.currentGameState == GameManager.GameState.ExitMenu || isClick) ? 
                        ModelState.Stop : ModelState.LookAround);
                } 
                break;



                case ModelState.Stop:
                if (GameManager.Instance.currentGameState == GameManager.GameState.Menu)
                    ChangeModelState(ModelState.Idle);
                break;
        }

        if(GameManager.Instance.currentGameState != GameManager.GameState.Menu 
            && currentModelState != ModelState.Reset
            && currentModelState != ModelState.Stop)
        {
            ChangeModelState(ModelState.Reset);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.Menu)
        {
            isClick = true;
            rb.isKinematic = false;
            if(transform.rotation.eulerAngles.z < 5 || transform.rotation.eulerAngles.z > 355)
            {
                rb.velocity += Vector2.up * 8 + Vector2.right * (transform.position.x - Camera.main.ScreenToWorldPoint(eventData.position).x) * 3;
                ChangeModelState(ModelState.Idle);
            }
            else ChangeModelState(ModelState.Reset);
        }
    }
}
