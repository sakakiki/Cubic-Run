using UnityEngine;
using UnityEngine.EventSystems;

public class Button_BoardTab : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Button_BoardTab anotherTab;
    [SerializeField] private SpriteRenderer boardSprite;
    [SerializeField] private bool isTraining;
    [SerializeField] private GameObject[] contents;

    //タップ：タップ時に実行
    //マウス：クリック時に実行
    public void OnPointerDown(PointerEventData eventData)
    {
        //ボードの表示を前に
        this.boardSprite.sortingOrder = 0;

        //他方のスクリプトを有効化
        anotherTab.enabled = true;

        //ゲームマネージャーの設定を変更
        GameManager.Instance.SetTrainingMode(isTraining);

        //コンテンツの有効化
        foreach (var item in contents)
            item.SetActive(true);

        //このスクリプトを無効化
        this.enabled = false;
    }

    //スクリプトが有効化時
    private void OnEnable()
    {
        //ボードの表示を後ろに
        this.boardSprite.sortingOrder = -1;

        //コンテンツの無効化
        foreach (var item in contents)
            item.SetActive(false);
    }
}
