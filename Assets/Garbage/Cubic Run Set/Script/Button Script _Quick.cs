using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript_Quick : MonoBehaviour, IPointerDownHandler
{
    [NonSerialized] public bool isButtonPushed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPushed = true;
    }
}
