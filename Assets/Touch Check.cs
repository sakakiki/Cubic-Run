using UnityEngine;

public class TouchCheck : MonoBehaviour
{
    public bool isTouch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        isTouch = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        isTouch = false;
    }
}
