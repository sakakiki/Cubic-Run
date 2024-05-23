using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTouchCheck : MonoBehaviour
{
    [System.NonSerialized] public bool isTouch;
    [System.NonSerialized] public List<GameObject> HitObjects = new List<GameObject>();
    [System.NonSerialized] public float tunnelStart;
    [System.NonSerialized] public float tunnelEnd;
    [System.NonSerialized] public float caveStart;
    [System.NonSerialized] public float caveEnd;
    private int touchCount = 0;
    private GameObject Tunnel;
    private GameObject Cave;

    private void OnEnable()
    {
        isTouch = false; 
        touchCount = 0;
        tunnelStart = -10;
        tunnelEnd = -10;
        caveStart = -10;
        caveEnd = -10;
        Tunnel = null;
        Cave = null;
    }

    private void Update()
    {
        isTouch = touchCount > 0;

        if(Tunnel != null)
        {
            tunnelStart = Tunnel.transform.parent.position.x;
            tunnelEnd = Tunnel.transform.parent.position.x + Tunnel.transform.parent.localScale.x;
        }
        if(Cave != null)
        {
            caveStart = Cave.transform.parent.localPosition.x;
            caveEnd = Cave.transform.parent.position.x + Cave.transform.parent.localScale.x;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            touchCount++;
            HitObjects.Add(other.gameObject);
            if (other.CompareTag("Tunnel"))
            {
                Tunnel = other.gameObject;
            }
            else if (other.CompareTag("Cave"))
            {
                Cave = other.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            touchCount--;
            HitObjects.Remove(other.gameObject);
        }
    }
}
