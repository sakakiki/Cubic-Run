using UnityEngine;
using UnityEngine.EventSystems;

public class Button_BoardTab : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Button_BoardTab anotherTab;
    [SerializeField] private SpriteRenderer boardSprite;

    //タップ：タップ時に実行
    //マウス：クリック時に実行
    public void OnPointerDown(PointerEventData eventData)
    {
        //ボードの表示を前に
        this.boardSprite.sortingOrder = 0;

        //他方のスクリプトを有効化
        anotherTab.enabled = true;

        //このスクリプトを無効化
        this.enabled = false;
    }

    //スクリプトが有効化されればボードの表示を後ろに
    private void OnEnable()
    {
        this.boardSprite.sortingOrder = -1;
    }
}
