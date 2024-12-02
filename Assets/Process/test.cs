using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class test : MonoBehaviour,IPointerDownHandler
{
    private int count;

    public void OnPointerDown(PointerEventData eventData)
    {
        count++;
    }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(count);
        count = 0;
    }
}
