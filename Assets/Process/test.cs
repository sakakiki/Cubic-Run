using UnityEngine;
using UnityEngine.EventSystems; //�C�x���g�n�̃C���^�[�t�F�[�X���g�����Ƃ̐錾

//Canvas��ɂ���Q�[���I�u�W�F�N�g�ɃA�^�b�`
public class test : MonoBehaviour, IPointerDownHandler //�C���^�[�t�F�[�X�̎���
{
    public async void OnPointerDown(PointerEventData eventData) //�C���^�[�t�F�[�X�ɉ��������\�b�h��
    {
        Debug.Log("click");


        await FirestoreManager.Instance.SavePlayerData("test", 100, 100);


    }
}
