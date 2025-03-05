using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupUIManager : MonoBehaviour
{
    //自身のインスタンス
    public static PopupUIManager Instance;

    [SerializeField] private GameObject bandPopup;
    [SerializeField] private TextMeshProUGUI bandMessage;
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
    [SerializeField] private TextMeshProUGUI explanation_DI_2;
    [SerializeField] private TextMeshProUGUI inputfieldExplanation_DI_2;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    void Start()
    {
        
    }



    void Update()
    {
        
    }
}
