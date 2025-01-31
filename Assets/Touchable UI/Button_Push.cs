using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Push : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isPushed;
    private Vector3 defaultScale;
    [SerializeField] private SpriteRenderer cover;

    private void Start()
    {
        //基本のスケールを記憶
        defaultScale = transform.localScale;
    }

    //タップ：タップ時に実行
    //マウス：クリック時に実行
    public void OnPointerDown(PointerEventData eventData)
    {
        //スケールを拡大
        transform.localScale *= 1.05f;

        //カバーを着色
        cover.color = GameManager.Instance.panelSelectedColor - Color.black * 0.95f;
    }

    //タップ：タップ後に指が画面から離れたとき、指の場所に関わらず実行
    //マウス：クリック後にマウスボタンを離したとき、カーソルの位置に関わらず実行
    public void OnPointerUp(PointerEventData eventData)
    {
        //スケールを基本のスケールに戻す
        transform.localScale = defaultScale;

        //カバーを透明に
        cover.color =　Color.clear;
    }

    //タップ：タップ後に指が画面から離れたとき、指がオブジェクト上なら実行
    //マウス：クリック後にマウスボタンを離したとき、カーソルがオブジェクト上なら実行
    public void OnPointerClick(PointerEventData eventData)
    {
        //押下のフラグを立てる
        isPushed = true;
    }

    //Input Manager側から呼び出し
    //押下の有無を返す
    public bool GetIsPushed()
    {
        //返り値を保存
        bool returnValue = isPushed;

        //押下のフラグをリセット
        isPushed = false;

        return returnValue;
    }

    //スクリプトが無効化されればリセット処理
    private void OnDisable()
    {
        //スケールを基本のスケールに戻す
        transform.localScale = defaultScale;

        //カバーを透明に
        cover.color = Color.clear;

        //押下のフラグをリセット
        isPushed = false;
    }
}
