using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button_PlayInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent eventOnPush;
    public UnityEvent eventOnRelease;

    public void OnPointerDown(PointerEventData eventData)
    {
        eventOnPush?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        eventOnRelease?.Invoke();
    }

    //スクリプトが無効化されればリセット処理
    private void OnDisable()
    {
        eventOnRelease?.Invoke();
    }
}
