using UnityEngine;
using UnityEngine.EventSystems; //イベント系のインターフェースを使うことの宣言

//Canvas上にあるゲームオブジェクトにアタッチ
public class test : MonoBehaviour, IPointerDownHandler //インターフェースの実装
{
    public async void OnPointerDown(PointerEventData eventData) //インターフェースに応じたメソッド名
    {
        Debug.Log("click");


        await FirestoreManager.Instance.SavePlayerData("test", 100, 100);


    }
}
