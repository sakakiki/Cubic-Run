using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button_Push_BindEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent eventOnPush;
    private Vector3 defaultScale;
    [SerializeField] private SpriteRenderer cover;
    [SerializeField] private AudioClip clipSE = null;

    private void Start()
    {
        //基本のスケールを記憶
        defaultScale = transform.localScale;

        //効果音が未設定ならデフォルトの効果音を登録
        if (clipSE == null) clipSE = AudioManager.Instance.SE_Button;
    }

    //タップ：タップ時に実行
    //マウス：クリック時に実行
    public void OnPointerDown(PointerEventData eventData)
    {
        //スケールを拡大
        transform.localScale *= 1.05f;

        //カバーを着色
        cover.color = GameManager.Instance.panelSelectedColor - Color.black * 0.95f;

        //SEを再生
        AudioManager.Instance.PlaySE(clipSE);
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
        //イベントを実行
        eventOnPush?.Invoke();
    }

    //スクリプトが無効化されればリセット処理
    private void OnDisable()
    {
        //スケールを基本のスケールに戻す
        transform.localScale = defaultScale;

        //カバーを透明に
        cover.color = Color.clear;
    }
}
