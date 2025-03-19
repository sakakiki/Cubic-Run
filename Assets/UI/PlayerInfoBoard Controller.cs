using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInfoBoardController : MonoBehaviour, IPointerDownHandler
{
    private GameStateStateMachine gameStateMachine;
    [SerializeField] private RectTransform buttonIconRtf;
    [SerializeField] private RectTransform boardUpRtf;
    [SerializeField] private GameObject addInfo;
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
            AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Panel);
        if (!isOpen)
            AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Close);
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

            //アイコンの回転
            buttonIconRtf.localEulerAngles = Vector3.forward * Mathf.Lerp(0, 180, openRate);

            //ボードの開閉
            boardUpRtf.anchoredPosition = Vector3.up * Mathf.Lerp(0, 200, openRate);

            //拡張情報の表示・非表示
            addInfo.SetActive(openRate > 1);
        }
    }
}
