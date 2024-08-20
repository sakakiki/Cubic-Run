using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorKey : MonoBehaviour
{
    [SerializeField] private GameObject Up;
    [SerializeField] private GameObject Down;
    [SerializeField] private GameObject Left;
    [SerializeField] private GameObject Right;
    [SerializeField] private GameObject PushedUp;
    [SerializeField] private GameObject PushedDown;
    [SerializeField] private GameObject PushedLeft;
    [SerializeField] private GameObject PushedRight;
    [SerializeField] private AudioSource audioSource;

    private bool isPushUp;
    private bool isPushDown;
    private bool isPushLeft;
    private bool isPushRight;

    int num;
    
    void Update()
    {
        isPushUp = Input.GetKey(KeyCode.UpArrow);
        isPushDown = Input.GetKey(KeyCode.DownArrow);
        isPushLeft = Input.GetKey(KeyCode.LeftArrow);
        isPushRight = Input.GetKey(KeyCode.RightArrow);

        Up.SetActive(!isPushUp);
        Down.SetActive(!isPushDown);
        Left.SetActive(!isPushLeft);
        Right.SetActive(!isPushRight);
        PushedUp.SetActive(isPushUp);
        PushedDown.SetActive(isPushDown);
        PushedLeft.SetActive(isPushLeft);
        PushedRight.SetActive(isPushRight);

        if (Input.anyKeyDown)
            audioSource.Play();



        if (isPushUp == true)
        {
            num = 1;
        }
        else if (isPushUp == false)
        {
            num = 2;
        }



        num = isPushUp ? 1 : 2;



        num *= 16;
        num <<= 4;
    }
}
