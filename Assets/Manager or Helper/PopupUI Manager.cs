using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PopupUIManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static PopupUIManager Instance;

    public string inputText1 { private set; get; }
    public string inputText2 { private set; get; }

    [SerializeField] private SpriteRenderer bandBack;
    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private TextMeshProUGUI bandMessage;
    private bool isActiveBand;
    private float displayTime;
    private float colorAlpha;
    [SerializeField] private GameObject boardPopup;
    [SerializeField] private TextMeshProUGUI boardTitle;
    [SerializeField] private GameObject singleButton;
    [SerializeField] private GameObject doubleButton;
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

    [HideInInspector] public UnityEvent eventOnPushOK;
    [HideInInspector] public UnityEvent eventSaveInput;
    private UnityAction listenActionOnPushOK;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    private void Update()
    {
        //�A�N�e�B�u��ԂłȂ���Ή������Ȃ�
        if (!isActiveBand) return;

        //�c��\�����Ԃ����炷
        displayTime -= Time.deltaTime;

        //�\���I��0.5�b�ȓ��Ȃ�����Ă���
        if (displayTime < 0.5)
            colorAlpha -= Time.deltaTime * 2;
        //�\���J�n0.5�b�����ĔZ������
        else if (colorAlpha < 1)
            colorAlpha += Time.deltaTime * 2;
        else return;

        //�F�̍X�V
        bandBack.color = Color.white - Color.black * (1 - colorAlpha * 0.9f);
        bandMessage.color = Color.black * colorAlpha;

        //�\�����Ԃ��I���������A�N�e�B�u��Ԃ�
        if (displayTime < 0)
        {
            isActiveBand = false;
            colorAlpha = 0;
            bandBack.color = Color.clear;
            bandMessage.color = Color.clear;
        }
    }



    //�e�L�X�g�|�b�v�A�b�v�̍쐬
    public void SetupMessageText(string message)
    {
        textMessage.SetText(message);
    }
    //�e�L�X�g�|�b�v�A�b�v�̍폜
    public void DeleteMessageText()
    {
        textMessage.SetText("");
    }



    //�o���h�|�b�v�A�b�v�̍쐬
    public void SetupMessageBand(string message, float displayTime)
    {
        isActiveBand = true;
        this.displayTime = displayTime + 1;
        bandMessage.SetText(message);
    }



    //���b�Z�[�W�p�|�b�v�A�b�v�̍쐬�i�㑱�����Ȃ��j
    public void SetupPopupMessage(string title, string explanation)
    {
        boardPopup.SetActive(true);
        singleButton.SetActive(true);
        finalConfirmation.SetActive(true);
        boardTitle.SetText(title);
        explanation_FC.SetText(explanation);
    }



    //���b�Z�[�W�p�|�b�v�A�b�v�̍쐬�i�㑱��������j
    public void SetupPopupMessage(string title, string explanation, UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        singleButton.SetActive(true);
        finalConfirmation.SetActive(true);
        boardTitle.SetText(title);
        explanation_FC.SetText(explanation);
        eventOnPushOK.AddListener(onPushOK);
        listenActionOnPushOK = onPushOK;
    }



    //�m�F�p�|�b�v�A�b�v�̍쐬
    public void SetupPopup(string title, string explanation, UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        doubleButton.SetActive(true);
        finalConfirmation.SetActive(true);
        boardTitle.SetText(title);
        explanation_FC.SetText(explanation);
        eventOnPushOK.AddListener(onPushOK);
        listenActionOnPushOK = onPushOK;
    }



    //�P����̓|�b�v�A�b�v�̍쐬
    public void SetupPopup(
        string title, string explanation, TMP_InputField.ContentType contentType, UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        doubleButton.SetActive(true);
        singleInputfield.SetActive(true);
        boardTitle.SetText(title);
        explanation_SI.SetText(explanation);
        inputfield_SI.contentType = contentType;
        switch (contentType)
        {
            case TMP_InputField.ContentType.EmailAddress:
                inputfieldExplanation_SI.SetText("���[���A�h���X�����");
                break;

            case TMP_InputField.ContentType.Password:
                inputfieldExplanation_SI.SetText("�p�X���[�h�����");
                break;

            default: break;
        }
        eventOnPushOK.AddListener(onPushOK);
        listenActionOnPushOK = onPushOK;
        eventSaveInput.AddListener(SaveInput1);
    }



    //�v���C���[���p�P����̓|�b�v�A�b�v�̍쐬
    public void SetupPopup(
        string title, string explanation, string currentPlayerName, UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        doubleButton.SetActive(true);
        singleInputfield.SetActive(true);
        boardTitle.SetText(title);
        explanation_SI.SetText(explanation);
        inputfield_SI.contentType = TMP_InputField.ContentType.Standard;
        inputfieldExplanation_SI.SetText("�v���C���[�������");
        inputfield_SI.text = currentPlayerName;
        eventOnPushOK.AddListener(onPushOK);
        listenActionOnPushOK = onPushOK;
        eventSaveInput.AddListener(SaveInput1);
    }



    //2���̓|�b�v�A�b�v�̍쐬
    public void SetupPopup(
        string title,
        string explanation1, TMP_InputField.ContentType contentType1, 
        string explanation2, TMP_InputField.ContentType contentType2, 
        UnityAction onPushOK)
    {
        boardPopup.SetActive(true);
        doubleButton.SetActive(true);
        doubleInputfield.SetActive(true);
        boardTitle.SetText(title);
        explanation_DI_1.SetText(explanation1);
        inputfield_DI_1.contentType = contentType1;
        explanation_DI_2.SetText(explanation2);
        inputfield_DI_2.contentType = contentType2;
        switch (contentType1)
        {
            case TMP_InputField.ContentType.EmailAddress:
                inputfieldExplanation_DI_1.SetText("���[���A�h���X�����");
                inputfieldExplanation_DI_2.SetText("�p�X���[�h�����");
                break;

            default: break;
        }
        eventOnPushOK.AddListener(onPushOK);
        listenActionOnPushOK = onPushOK;
        eventSaveInput.AddListener(SaveInput2);
    }



    //OK�{�^���������ꂽ�ꍇ�̏���
    public void OnPushOK()
    {
        eventSaveInput?.Invoke();
        finalConfirmation.SetActive(false);
        singleInputfield.SetActive(false);
        doubleInputfield.SetActive(false);
        singleButton.SetActive(false);
        doubleButton.SetActive(false);
        boardPopup.SetActive(false);
        inputfield_SI.text = "";
        inputfield_DI_1.text = "";
        inputfield_DI_2.text = "";
        UnityAction tempAction = listenActionOnPushOK;
        eventOnPushOK?.Invoke();
        eventOnPushOK.RemoveListener(tempAction);
    }



    //�L�����Z���{�^���������ꂽ�ꍇ�̏���

    public void OnPushCancel()
    {
        finalConfirmation.SetActive(false);
        singleInputfield.SetActive(false);
        doubleInputfield.SetActive(false);
        singleButton.SetActive(false);
        doubleButton.SetActive(false);
        boardPopup.SetActive(false);
        inputfield_SI.text = "";
        inputfield_DI_1.text = "";
        inputfield_DI_2.text = "";
        eventOnPushOK.RemoveListener(listenActionOnPushOK);
    }



    //�P����͂̓��͓��e���L��
    public void SaveInput1()
    {
        if (inputfield_SI.text != "")
        {
            inputText1 = inputfield_SI.text;
            inputfield_SI.text = "";
        }
    }



    //2���͂̓��͓��e���L��
    public void SaveInput2()
    {
        if (inputfield_DI_1.text != "")
        {
            inputText1 = inputfield_DI_1.text;
            inputfieldExplanation_DI_1.text = "";
        }
        if (inputfield_DI_2.text != "")
        {
            inputText2 = inputfield_DI_2.text;
            inputfieldExplanation_DI_2.text = "";
        }
    }
}
