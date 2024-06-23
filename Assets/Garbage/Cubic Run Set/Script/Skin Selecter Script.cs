using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkinSelecterScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]private GameObject SkinSelecterWheel;
    [SerializeField]private GameObject SelectFrame;
    [System.NonSerialized] public int skinNumber;
    private bool isDrag;
    private float lastPointerPosY;
    private float turnSpeed;
    private float[] YPointDeltas = new float[5];


    private void Start()
    {
        transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().transform.localScale = Vector3.one + Vector3.up * (1/Camera.main.aspect - 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPointerPosY = eventData.position.y;
        isDrag = true;
        SelectFrame.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(GameManager.Instance.currentGameState == GameManager.GameState.SkinSelect)
        {
            SkinSelecterWheel.transform.eulerAngles += Vector3.forward * (eventData.position.y - lastPointerPosY) / 4;
            for (int i = 4; i > 0; i--)
                YPointDeltas[i] = YPointDeltas[i - 1];
            YPointDeltas[0] = eventData.position.y - lastPointerPosY;
            lastPointerPosY = eventData.position.y;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        turnSpeed = (YPointDeltas.Sum())/20;
        isDrag = false;
    }

    private void Update()
    {
        if(!isDrag)
        {
            SkinSelecterWheel.transform.eulerAngles += Vector3.forward * turnSpeed;
            turnSpeed *= 0.9f;
            if(Mathf.Abs(turnSpeed) < 0.2f && turnSpeed != 0)
            {
                turnSpeed = 0;
                for(int i = 0; i < 16; i++)
                {
                    if (22.5f * i - 11.25f < SkinSelecterWheel.transform.eulerAngles.z && SkinSelecterWheel.transform.eulerAngles.z < 22.5f * i + 11.25f)
                    {
                        SkinSelecterWheel.transform.eulerAngles = Vector3.forward * 22.5f * i + Vector3.up * 90;
                        skinNumber = i;
                        break;
                    }
                    else if (i == 15)
                    {
                        skinNumber = 0;
                        SkinSelecterWheel.transform.eulerAngles = Vector3.up * 90;
                    }
                }
                SelectFrame.SetActive(true);
            }
        }
    }
}
