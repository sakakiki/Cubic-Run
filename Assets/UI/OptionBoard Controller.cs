using UnityEngine;
using UnityEngine.EventSystems;

public class OptionBoardController : MonoBehaviour, IPointerDownHandler
{
    private GameStateStateMachine gameStateMachine;
    [SerializeField] private SpriteRenderer optionIcon;
    [SerializeField] private RectTransform optionRtf;
    [SerializeField] private SpriteRenderer[] closeIcon;
    [SerializeField] private RectTransform closeRtf;
    [SerializeField] private RectTransform boardRtf;
    private bool isOpen;
    private float openRate;

    public void Start()
    {
        gameStateMachine = GameManager.Instance.gameStateMachine;
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        //メニュー画面でなければ何もしない
        if (gameStateMachine.currentState != gameStateMachine.state_Menu) return;

        //フラグを切り替え
        isOpen = !isOpen;

        //SEの再生
        if (isOpen)
            AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Panel);
        if (!isOpen)
            AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Close);
    }



    public void Update()
    {
        //メニュー画面かオプション画面でなければ閉じる
        if (gameStateMachine.currentState != gameStateMachine.state_Menu &&
            gameStateMachine.currentState != gameStateMachine.state_Account &&
            gameStateMachine.currentState != gameStateMachine.state_Volume &&
            gameStateMachine.currentState != gameStateMachine.state_Button &&
            gameStateMachine.currentState != gameStateMachine.state_Credit) 
            isOpen = false;

        //開閉処理の必要があるなら
        if (isOpen && openRate < 1 || !isOpen && openRate > 0)
        {
            //開閉具合を更新
            openRate += Time.deltaTime * (isOpen ? 5 : -5);

            //アイコンのカラー変更
            optionIcon.color = Color.white - Color.black * openRate;
            closeIcon[0].color = Color.black * openRate;
            closeIcon[1].color = Color.black * openRate;

            //アイコンの回転
            optionRtf.localEulerAngles = Vector3.forward * Mathf.Lerp(0, 90, openRate);
            closeRtf.localEulerAngles = Vector3.forward * Mathf.Lerp(-90, 0, openRate);

            //ボードの開閉
            boardRtf.localScale = Vector3.one * Mathf.Lerp(0, 1, openRate);
        }
    }
}
