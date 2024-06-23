using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript_Normal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private GameManager.GameState ActiveGameState;

    [NonSerialized] public bool isButtonPushed;

    private Vector3 defaultScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.currentGameState == ActiveGameState) 
            transform.localScale *= 1.05f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.currentGameState == ActiveGameState)
            isButtonPushed = true;
    }
}
