using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkinSelecterBoardScript : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color transparency;
    private Color transparencyChange = new Color(0, 0, 0, 1f);
    private TextMeshProUGUI text;
    private SpriteRenderer modelSkin;
    [System.NonSerialized] public Color modelColor;
    private SpriteRenderer modelEyeR;
    private SpriteRenderer modelEyeL;
    void Start()
    {
        sprite = transform.Find("Button Sprite").GetComponent<SpriteRenderer>();
        text = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        modelSkin = transform.Find("Model").GetComponent<SpriteRenderer>();
        modelEyeR = modelSkin.transform.Find("Model Eye _R").GetComponent<SpriteRenderer>();
        modelEyeL = modelSkin.transform.Find("Model Eye _L").GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (transform.position.z < transform.parent.position.z)
        {
            transparency = transparencyChange * Math.Abs(transform.position.y - transform.parent.position.y) * Camera.main.aspect / 5f;
            sprite.color = Color.white - transparency;
            text.color = Color.black - transparency;
            modelSkin.color = modelColor - transparency;
            modelEyeR.color = Color.white - transparency;
            modelEyeL.color = Color.white - transparency;
        }
        else 
        {
            sprite.color = Color.clear;
            text.color = Color.clear;
            modelSkin.color = Color.clear;
            modelEyeR.color = Color.clear;
            modelEyeL.color = Color.clear;
        }
    }
}
