using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Tap : MonoBehaviour, IPointerDownHandler
{
    private bool isTapped;

    //タップ：タップ時に実行
    //マウス：クリック時に実行
    public void OnPointerDown(PointerEventData eventData)
    {
        //押下のフラグを立てる
        isTapped = true;
    }

    //Input Manager側から呼び出し
    //押下の有無を返す
    public bool GetIsTapped()
    {
        //返り値を保存
        bool returnValue = isTapped;

        //押下のフラグをリセット
        isTapped = false;

        return returnValue;
    }

    //スクリプトが無効化されればリセット処理
    private void OnDisable()
    {
        //押下のフラグをリセット
        isTapped = false;
    }
}
