using System;
using UnityEngine;

public class TouchCheck : MonoBehaviour
{
    [NonSerialized] public bool isTouch;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isTouch = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isTouch = false;
    }
}
