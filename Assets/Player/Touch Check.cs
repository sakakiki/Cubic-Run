using System;
using UnityEngine;

public class TouchCheck : MonoBehaviour
{
    [NonSerialized] public bool isTouch;
    private int touchCount = 0;

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
}
