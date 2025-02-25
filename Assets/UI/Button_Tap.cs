using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Tap : MonoBehaviour, IPointerDownHandler
{
    private bool isTapped;

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public void OnPointerDown(PointerEventData eventData)
    {
        //�����̃t���O�𗧂Ă�
        isTapped = true;
    }

    //Input Manager������Ăяo��
    //�����̗L����Ԃ�
    public bool GetIsTapped()
    {
        //�Ԃ�l��ۑ�
        bool returnValue = isTapped;

        //�����̃t���O�����Z�b�g
        isTapped = false;

        return returnValue;
    }

    //�X�N���v�g�������������΃��Z�b�g����
    private void OnDisable()
    {
        //�����̃t���O�����Z�b�g
        isTapped = false;
    }
}
