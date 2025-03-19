using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button_Tap_BindEvent : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent eventOnTap;

    public void OnPointerDown(PointerEventData eventData)
    {
        eventOnTap?.Invoke();
    }
}
