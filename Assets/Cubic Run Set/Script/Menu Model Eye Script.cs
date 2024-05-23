using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuModelEyeScript : MonoBehaviour
{
    public GameObject MenuModel;

    [System.NonSerialized] public static float modelScale =1f;

    private Vector3 initialScale;
    private Vector3 localScale;
    public static float closeTiming;
    private float eyeOpen = 1;
    void Awake()
    {
        localScale = transform.localScale;
        initialScale = transform.localScale;
        closeTiming = Time.time + 5;
    }

    void LateUpdate()
    {
        localScale.x = initialScale.x / MenuModel.transform.localScale.x * modelScale;
        localScale.y = (initialScale.y / MenuModel.transform.localScale.y) * eyeOpen * modelScale;
        transform.localScale = localScale;

        if (Time.time < closeTiming)
            eyeOpen = 1;
        else if (Time.time < closeTiming + 0.1f)
            eyeOpen -= Time.deltaTime * 10;
        else if (Time.time < closeTiming + 0.2f)
            eyeOpen += Time.deltaTime * 10;
        else closeTiming = Time.time + UnityEngine.Random.Range(0.5f, 5.5f);
    }
}
