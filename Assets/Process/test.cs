using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //イベント系のインターフェースを使うことの宣言

//Canvas上にあるゲームオブジェクトにアタッチ
public class test : MonoBehaviour, IPointerDownHandler //インターフェースの実装
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


    public async void OnPointerDown(PointerEventData eventData) //インターフェースに応じたメソッド名
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
