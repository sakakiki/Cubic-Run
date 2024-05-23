using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeScript : MonoBehaviour
{
    public GameObject Player;

    private Vector3 initialLocalPosition;
    private Vector3 localPosition;


    void Awake()
    {
        initialLocalPosition = transform.position - Player.transform.position;
    }


    void LateUpdate()
    {
        localPosition.x = initialLocalPosition.x * (Player.transform.localScale.x - 0.125f) * 8 / 7;
        localPosition.y = initialLocalPosition.y * (Player.transform.localScale.y - 0.125f) * 8 / 7;
        transform.position = Player.transform.position + localPosition;
    }
}
