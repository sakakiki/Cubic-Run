using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_LevelSelecter: MonoBehaviour, IPointerClickHandler
{
    public bool isEnable;
    [SerializeField] private Transform tf;
    private int level;
    [SerializeField] private TextMeshProUGUI levelTMP;
    public TextMeshProUGUI clearTimesNumTMP;
    [SerializeField] private Image image;
    [SerializeField] private Image cover;
    private Color panelColor;
    private Color defaultColor = Color.black;
    private Color selectedColor = Color.blue;
    private float boardCenterY = -1;

    //タップ：タップ後に指が画面から離れたとき、指がオブジェクト上なら実行
    //マウス：クリック後にマウスボタンを離したとき、カーソルがオブジェクト上なら実行
    public void OnPointerClick(PointerEventData eventData)
    {
        //有効状態でなければ終了
        if (!isEnable) return;

        //入力の受け渡し
        GameManager.Instance.SetTrainingLevel(level);

        PushButton();
    }

    //押下処理
    public void PushButton()
    {
        //表示を選択状態へ変化
        image.color = selectedColor;
        cover.color = selectedColor - Color.black * 0.95f;

        //入力を無効化
        isEnable = false;
    }

    //初期化処理
    public void Initialize()
    {
        //表示を非選択状態へ変化
        image.color = defaultColor;
        cover.color = Color.clear;

        //入力を有効化
        isEnable = true;
    }

    //このスクリプトのレベルを設定
    public void SetLevel(int level)
    {
        this.level = level;
        levelTMP.SetText("" + level);
    }
}
