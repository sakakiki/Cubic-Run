using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkinSelecter : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform wheelTf;
    private float wheelEulerAnglesX;
    private bool isDragged;
    private float angleVelocityX;
    private float currentPointY;
    private float currentTime;
    private Queue<float> pointQueue = new Queue<float>();
    private Queue<float> timeQueue = new Queue<float>();
    private bool isSnapping;
    private float targetAngleX;



    public void OnPointerDown(PointerEventData eventData)
    {
        //‰ñ“]ó‘Ô‚É•Ï‰»
        isDragged = true;

        //Œ»Ý’l‚Ì‹L‰¯‚ÆQueue‚Ö‚ÌŠi”[
        currentPointY = eventData.position.y;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);
    }



    public void OnDrag(PointerEventData eventData)
    {
        float newScreenPointY = eventData.position.y;

        //ƒzƒC[ƒ‹‚Ì‰ñ“]
        wheelEulerAnglesX += (newScreenPointY - currentPointY) / Screen.height * 160;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;

        //Œ»Ý’l‚Ì‹L‰¯‚ÆQueue‚Ö‚ÌŠi”[
        currentPointY = newScreenPointY;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);

        //Queue“à•s—vƒf[ƒ^íœ
        while (currentTime - timeQueue.Peek() > 0.1)
        {
            pointQueue.Dequeue();
            timeQueue.Dequeue();
        }
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        //‰ñ“]ó‘Ô‚ðI—¹
        isDragged = false;

        //ƒXƒiƒbƒvó‘Ô‰ðœ
        isSnapping = false;

        //‘¬“x•t—^
        if (timeQueue.Count >= 2)
            angleVelocityX = (currentPointY - pointQueue.Dequeue()) / (currentTime - timeQueue.Dequeue()) / Screen.height * 64;
        else angleVelocityX = 0;
        if (angleVelocityX > 1800) 
            angleVelocityX = 1800;

        //Queue“àƒf[ƒ^íœ
        timeQueue.Clear();
        pointQueue.Clear();
    }



    private void Update()
    {
        //ƒhƒ‰ƒbƒO’†‚Í‰½‚à‚µ‚È‚¢
        if (isDragged) return;

        //‰ñ“]—Ê•â³
        while (wheelEulerAnglesX > 360)
            wheelEulerAnglesX -= 360;
        while (wheelEulerAnglesX < 0)
            wheelEulerAnglesX += 360;

        //Œ¸‘¬‚µ‚È‚ª‚ç‰ñ“]
        if (Mathf.Abs(angleVelocityX) > 10)
        {
            //ˆ——Ž‚¿‚µ‚Ä‚¢‚½‚ç‰ñ“]‚ð’âŽ~i0œŽZ–hŽ~j
            if (Time.deltaTime * 2 > 1)
                angleVelocityX = 0;

            //‰ñ“]‚ðŒ¸‘¬
            angleVelocityX *= (1 - Time.deltaTime * 2);

            //ƒzƒC[ƒ‹‚ð‰ñ“]
            wheelEulerAnglesX += angleVelocityX * Time.deltaTime;
            wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
        }

        //ƒXƒiƒbƒvˆ—
        else
        {
            //ŠJŽnˆ—
            if (!isSnapping)
            {
                //ƒXƒiƒbƒv“®ìŠJŽn
                isSnapping = true;

                //‰ñ“]•ûŒü‚É•â³‚ð‚©‚¯‚Â‚Â–Ú•W‰ñ“]—Ê‚ðŽZo
                targetAngleX = Mathf.Floor(wheelEulerAnglesX / 22.5f) * 22.5f;
                if (wheelEulerAnglesX + angleVelocityX/3 - targetAngleX > 11.25f)
                    targetAngleX += 22.5f;

                //‰ñ“]‚ð’âŽ~
                angleVelocityX = 0;
            }

            //‰ñ“]‚ª•K—v‚È‚ç
            if (wheelTf.localEulerAngles.x != targetAngleX)
            {
                //•âŠÔ‰ñ“]
                wheelEulerAnglesX = Mathf.Lerp(wheelEulerAnglesX, targetAngleX, Time.deltaTime * 10);
                wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
            }
        }
    }
}
