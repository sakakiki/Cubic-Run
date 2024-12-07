using UnityEngine;
using UnityEngine.EventSystems; //イベント系のインターフェースを使うことの宣言

//Canvas上にあるゲームオブジェクトにアタッチ
public class test : MonoBehaviour, IPointerDownHandler //インターフェースの実装
{
    public void OnPointerDown(PointerEventData eventData) //インターフェースに応じたメソッド名
    {
        //クリック（タップ）時の処理
    }
}
