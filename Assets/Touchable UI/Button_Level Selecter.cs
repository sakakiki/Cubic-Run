using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_LevelSelecter: MonoBehaviour, IPointerClickHandler
{
    private int level;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private RectTransform rtf;
    private Vector2 defaultApos = new Vector2(-15, 15);
    private Vector2 pushedApos = new Vector2(-5, 5);

    //タップ：タップ後に指が画面から離れたとき、指がオブジェクト上なら実行
    //マウス：クリック後にマウスボタンを離したとき、カーソルがオブジェクト上なら実行
    public void OnPointerClick(PointerEventData eventData)
    {
        //押下状態へ変化
        rtf.anchoredPosition = pushedApos;

        //入力の受け渡し
        GameManager.Instance.SetTrainingLevel(level);

        //スクリプトを無効化
        this.enabled = false;
    }

    //初期化処理
    public void OnEnable()
    {
        rtf.anchoredPosition = defaultApos;
    }

    //このスクリプトのレベルを設定
    public void SetLevel(int level)
    {
        this.level = level;
        levelTMP.SetText("" + level);
    }
}
