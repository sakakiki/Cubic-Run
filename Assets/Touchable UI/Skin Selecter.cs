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
        //��]��Ԃɕω�
        isDragged = true;

        //���ݒl�̋L����Queue�ւ̊i�[
        currentPointY = eventData.position.y;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);
    }



    public void OnDrag(PointerEventData eventData)
    {
        float newScreenPointY = eventData.position.y;

        //�z�C�[���̉�]
        wheelEulerAnglesX += (newScreenPointY - currentPointY) / Screen.height * 160;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;

        //���ݒl�̋L����Queue�ւ̊i�[
        currentPointY = newScreenPointY;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);

        //Queue���s�v�f�[�^�폜
        while (currentTime - timeQueue.Peek() > 0.1)
        {
            pointQueue.Dequeue();
            timeQueue.Dequeue();
        }
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        //��]��Ԃ��I��
        isDragged = false;

        //�X�i�b�v��ԉ���
        isSnapping = false;

        //���x�t�^
        if (timeQueue.Count >= 2)
            angleVelocityX = (currentPointY - pointQueue.Dequeue()) / (currentTime - timeQueue.Dequeue()) / Screen.height * 64;
        else angleVelocityX = 0;
        if (angleVelocityX > 1800) 
            angleVelocityX = 1800;

        //Queue���f�[�^�폜
        timeQueue.Clear();
        pointQueue.Clear();
    }



    private void Update()
    {
        //�h���b�O���͉������Ȃ�
        if (isDragged) return;

        //��]�ʕ␳
        while (wheelEulerAnglesX > 360)
            wheelEulerAnglesX -= 360;
        while (wheelEulerAnglesX < 0)
            wheelEulerAnglesX += 360;

        //�������Ȃ����]
        if (Mathf.Abs(angleVelocityX) > 10)
        {
            //�����������Ă������]���~�i0���Z�h�~�j
            if (Time.deltaTime * 2 > 1)
                angleVelocityX = 0;

            //��]������
            angleVelocityX *= (1 - Time.deltaTime * 2);

            //�z�C�[������]
            wheelEulerAnglesX += angleVelocityX * Time.deltaTime;
            wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
        }

        //�X�i�b�v����
        else
        {
            //�J�n����
            if (!isSnapping)
            {
                //�X�i�b�v����J�n
                isSnapping = true;

                //��]�����ɕ␳�������ڕW��]�ʂ��Z�o
                targetAngleX = Mathf.Floor(wheelEulerAnglesX / 22.5f) * 22.5f;
                if (wheelEulerAnglesX + angleVelocityX/3 - targetAngleX > 11.25f)
                    targetAngleX += 22.5f;

                //��]���~
                angleVelocityX = 0;
            }

            //��]���K�v�Ȃ�
            if (wheelTf.localEulerAngles.x != targetAngleX)
            {
                //��ԉ�]
                wheelEulerAnglesX = Mathf.Lerp(wheelEulerAnglesX, targetAngleX, Time.deltaTime * 10);
                wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
            }
        }
    }
}
