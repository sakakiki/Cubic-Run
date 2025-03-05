using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PopupUIManager : MonoBehaviour
{
    //自身のインスタンス
    public static PopupUIManager Instance;

    public string inputText1 { private set; get; }
    public string inputText2 { private set; get; }

    [SerializeField] private SpriteRenderer bandBack;
    [SerializeField] private TextMeshProUGUI bandMessage;
    private bool isActiveBand;
    private float displayTime;
    private float colorAlpha;
    [SerializeField] private GameObject boardPopup;
    [SerializeField] private TextMeshProUGUI boardTitle;
    [SerializeField] private Button_Push button_OK;
    [SerializeField] private Button_Push button_Cancel;
    [SerializeField] private GameObject finalConfirmation;
    [SerializeField] private TextMeshProUGUI explanation_FC;
    [SerializeField] private GameObject singleInputfield;
    [SerializeField] private TextMeshProUGUI explanation_SI;
    [SerializeField] private TextMeshProUGUI inputfieldExplanation_SI;
    [SerializeField] private TMP_InputField inputfield_SI;
    [SerializeField] private GameObject doubleInputfield;
    [SerializeField] private TextMeshProUGUI explanation_DI_1;
    [SerializeField] private TextMeshProUGUI inputfieldExplanation_DI_1;
    [SerializeField] private TMP_InputField inputfield_DI_1;
    [SerializeField] private TextMeshProUGUI explanation_DI_2;
    [SerializeField] private TextMeshProUGUI inputfieldExplanation_DI_2;
    [SerializeField] private TMP_InputField inputfield_DI_2;

    private UnityEvent eventOnPushOK;
    private UnityEvent eventSaveInput;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    private void Start()
    {
        
    }



    private void Update()
    {
        if (!isActiveBand) return;

        displayTime -= Time.deltaTime;

        if (displayTime < 0.5)
            colorAlpha -= Time.deltaTime * 2;
        else if (colorAlpha < 1)
            colorAlpha += Time.deltaTime * 2;
        else return;

        bandBack.color = Color.white - Color.black * (1 - colorAlpha * 0.9f);
        bandMessage.color = Color.black * colorAlpha;

        if (displayTime < 0)
        {
            isActiveBand = false;
            colorAlpha = 0;
            bandBack.color = Color.clear;
            bandMessage.color = Color.clear;
        }
    }



    public void SetupMessageBand(string message, float displayTime)
    {
        isActiveBand = true;
        this.displayTime = displayTime + 1;
        bandMessage.SetText(message);
    }



    public void SetupPopup(string explanation, UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        finalConfirmation.SetActive(true);
        explanation_FC.SetText(explanation);
        eventOnPushOK.AddListener(onPushOK);
    }



    public void SetupPopup(string explanation, TMP_InputField.ContentType contentType, UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        singleInputfield.SetActive(true);
        explanation_SI.SetText(explanation);
        inputfield_SI.contentType = contentType;
        switch (contentType)
        {
            case TMP_InputField.ContentType.Standard:
                inputfieldExplanation_SI.SetText("プレイヤー名を入力");
                break;

            case TMP_InputField.ContentType.EmailAddress:
                inputfieldExplanation_SI.SetText("メールアドレスを入力");
                break;

            default: break;
        }
        eventOnPushOK.AddListener(onPushOK);
        eventSaveInput.AddListener(SaveInput1);
    }



    public void SetupPopup(
        string explanation1, TMP_InputField.ContentType contentType1, 
        string explanation2, TMP_InputField.ContentType contentType2, 
        UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        doubleInputfield.SetActive(true);
        explanation_DI_1.SetText(explanation1);
        inputfield_DI_1.contentType = contentType1;
        explanation_DI_2.SetText(explanation2);
        inputfield_DI_2.contentType = contentType2;
        switch (contentType1)
        {
            case TMP_InputField.ContentType.EmailAddress:
                inputfieldExplanation_DI_1.SetText("メールアドレスを入力");
                inputfieldExplanation_DI_2.SetText("パスワードを入力");
                break;

            case TMP_InputField.ContentType.Password:
                inputfieldExplanation_DI_1.SetText("パスワードを入力");
                inputfieldExplanation_DI_2.SetText("パスワードを入力（確認用）");
                break;

            default: break;
        }
        eventOnPushOK.AddListener(onPushOK);
        eventSaveInput.AddListener(SaveInput2);
    }



    public void OnPushOK()
    {
        eventSaveInput?.Invoke();
        finalConfirmation.SetActive(false);
        singleInputfield.SetActive(false);
        doubleInputfield.SetActive(false);
        boardPopup.SetActive(false);
        eventOnPushOK?.Invoke();
    }



    public void OnPushCancel()
    {
        finalConfirmation.SetActive(false);
        singleInputfield.SetActive(false);
        doubleInputfield.SetActive(false);
        boardPopup.SetActive(false);
    }



    private void SaveInput1()
    {
        inputText1 = inputfield_SI.text;
    }



    private void SaveInput2()
    {
        inputText1 = inputfield_DI_1.text;
        inputText2 = inputfield_DI_2.text;
    }
}
