using System;
using UnityEngine;

public class TouchCheck : MonoBehaviour
{
    [NonSerialized] public bool isTouch;    //�ڐG����
    private int touchCount = 0; //�����ɐڐG���Ă���Collider�̐�

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        isTouch = true;
        touchCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        touchCount--;
        if(touchCount == 0) isTouch = false;
    }

    public void Initialize()
    {
        isTouch = false;
        touchCount = 0;
    }
}
