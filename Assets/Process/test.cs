using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //�C�x���g�n�̃C���^�[�t�F�[�X���g�����Ƃ̐錾

//Canvas��ɂ���Q�[���I�u�W�F�N�g�ɃA�^�b�`
public class test : MonoBehaviour, IPointerDownHandler //�C���^�[�t�F�[�X�̎���
{
    [SerializeField] bool isSaveMode;
    [SerializeField] private string playerName;
    [SerializeField] private int playerRank;
    [SerializeField] private int higghScore;
    [SerializeField] private int playerExp;

    FirestoreManager FSM;

    public void Start()
    {
        FSM = FirestoreManager.Instance;
    }


    public async void OnPointerDown(PointerEventData eventData) //�C���^�[�t�F�[�X�ɉ��������\�b�h��
    {
        Debug.Log("click");

        if (isSaveMode)
        {
            await FSM.SavePlayerName(playerName);
            await FSM.SavePlayerRank(playerRank);
            await FSM.SaveHighScore(higghScore);
            await FSM.SaveExperience(playerExp);
            await FSM.MergeSkinUnlocks();
        }
        else
        {
            await FSM.LoadPlayerName();
            await FSM.LoadPlayerRank();
            await FSM.LoadHighScore();
            await FSM.LoadExperience();
            await FSM.MergeSkinUnlocks();
        }


    }
}
