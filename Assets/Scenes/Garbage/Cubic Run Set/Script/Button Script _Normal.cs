using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript_Normal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private GameManagerScript.GameState ActiveGameState;

    [NonSerialized] public bool isButtonPushed;

    private Vector3 defaultScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManagerScript.Instance.currentGameState == ActiveGameState) 
            transform.localScale *= 1.05f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManagerScript.Instance.currentGameState == ActiveGameState)
            isButtonPushed = true;
    }
}
