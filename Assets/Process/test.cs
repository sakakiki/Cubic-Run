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
        /*
        Debug.Log("click");

        if (isSaveMode)
        {
            await FSM.SavePlayerName(playerName);
            await FSM.SaveHighScore(higghScore);
            await FSM.SaveExperience(playerExp);
            await FSM.SavePlayerScore();
            for (int i = 1; i <= 15; i++)
                await FSM.SaveTrainingClearCount(i);
            await FSM.SaveUsingSkin();
            await FSM.SaveRunDistance(playerExp/10);
        }
        else
        {
            await FSM.LoadPlayerName();
            await FSM.LoadHighScore();
            await FSM.LoadExperience();
            await FSM.LoadPlayerScore();
            await FSM.LoadTrainingClearCounts();
            await FSM.LoadUsingSkin();
            await FSM.LoadRunDistance();
        }
        */

    }
}
