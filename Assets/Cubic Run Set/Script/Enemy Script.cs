using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bCol;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        bCol = GetComponent<BoxCollider2D>();
        rb.isKinematic = true;
        bCol.enabled = true;
        Vector3 startPosition = transform.localPosition;
        startPosition.y = 0.5f;
        transform.localPosition = startPosition;
    }

    private void Update()
    {
        transform.localPosition = Vector3.up * transform.localPosition.y + Vector3.right * 0.5f;
    }
    
}
