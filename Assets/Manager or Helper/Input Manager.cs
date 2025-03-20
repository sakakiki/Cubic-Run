using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] GameObject screenButtonObj;
    [SerializeField] private Button_Tap button_Screen;
    [SerializeField] private Button_Push button_Menu_Play;
    [SerializeField] private Button_Push button_Menu_Skin;
    [SerializeField] private Button_Push button_Menu_Tutorial;
    [SerializeField] private Button_Push button_Menu_Account;
    [SerializeField] private Button_Push button_Menu_Volume;
    [SerializeField] private Button_Push button_Menu_Button;
    [SerializeField] private Button_Push button_Menu_Credit;
    [SerializeField] private Button_Push button_Skin_OK;
    [SerializeField] private Button_Push button_Skin_Cancel;
    public bool isSkinSelect {  get; private set; }
    [SerializeField] private Button_Tap button_Play_Pause;
    public bool isPauseGame;
    public int playButtonPatternNum = 0;
    public int[] actionAllocation = new int[3];
    public PlayButtonSet[] playButtonSet;
    [SerializeField] private GameObject[] playButtonObject;
    [SerializeField] private Button_Push button_Pause_Continue;
    [SerializeField] private Button_Push button_Pause_Retire;
    [SerializeField] private Button_Push button_Result_Title;
    [SerializeField] private Button_Push button_Result_Retry;
    [SerializeField] private Button_Tap button_Option_Close;

    public bool is_Screen_Tap { get; private set; }

    public bool is_Menu_Play_Push { get; private set; }
    public bool is_Menu_Skin_Push { get; private set; }
    public bool is_Menu_Tutorial_Push { get; private set; }
    public bool is_Menu_Account_Push { get; private set; }
    public bool is_Menu_Volume_Push { get; private set; }
    public bool is_Menu_Button_Push { get; private set; }
    public bool is_Menu_Credit_Push { get; private set; }

    public bool is_Skin_OK_Push { get; private set; }
    public bool is_Skin_Cancel_Push { get; private set; }

    public bool is_Play_Pause_Tap { get; private set; }

    public bool is_Player_Jump_Push {  get; private set; }
    public bool is_Player_Squat_Push { get; private set; }
    public bool is_Player_Squat_Hold { get; private set; }
    public bool is_Player_Squat_Release { get; private set; }
    public bool is_Player_Attack_Push { get; private set; }
    public bool is_Player_Attack_Hold { get; private set; }
    public bool is_Player_Attack_Release { get; private set; }

    public bool is_Pause_Continue_Push { get; private set; }
    public bool is_Pause_Retire_Push { get; private set; }

    public bool is_Result_Title_Push { get; private set; }
    public bool is_Result_Retry_Push { get; private set; }

    public bool is_Option_Close_Tap { get; private set; }



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    public void LateUpdate()
    {
        ResetInput_Player();
    }



    #region SetActiveメソッド
    //有効・無効の切り替えに使用
    public void InputUISetActive_Screen(bool isActive)
    {
        screenButtonObj.SetActive(isActive);
    }

    public void InputUISetActive_Menu(bool isActive)
    {
        button_Menu_Play.enabled = isActive;
        button_Menu_Skin.enabled = isActive;
        button_Menu_Tutorial.enabled = isActive;
        button_Menu_Account.enabled = isActive;
        button_Menu_Volume.enabled = isActive;
        button_Menu_Button.enabled = isActive;
        button_Menu_Credit.enabled = isActive;
    }

    public void InputUISetActive_Skin(bool isActive)
    {
        button_Skin_OK.enabled = isActive;
        button_Skin_Cancel.enabled = isActive;
    }

    public void InputUISetActive_Play(bool isActive)
    {
        button_Play_Pause.enabled = isActive;
    }

    public void InputUISetActive_Player(bool isActive)
    {
        playButtonObject[playButtonPatternNum].SetActive(isActive);
    }

    public void InputUISetActive_Pause(bool isActive)
    {
        button_Pause_Continue.enabled = isActive;
        button_Pause_Retire.enabled = isActive;
    }

    public void InputUISetActive_Result(bool isActive)
    {
        button_Result_Retry.enabled = isActive;
        button_Result_Title.enabled = isActive;
    }

    public void InputUISetActive_Option(bool isActive)
    {
        button_Option_Close.enabled = isActive;
    }
    #endregion




    #region GetInputメソッド
    //入力情報を最新に更新
    public void GetInput_Screen()
    {
        is_Screen_Tap = button_Screen.GetIsTapped();
    }

    public void GetInput_Menu()
    {
        is_Menu_Play_Push = button_Menu_Play.GetIsPushed();
        is_Menu_Skin_Push = button_Menu_Skin.GetIsPushed();
        is_Menu_Tutorial_Push = button_Menu_Tutorial.GetIsPushed();
        is_Menu_Account_Push = button_Menu_Account.GetIsPushed();
        is_Menu_Volume_Push = button_Menu_Volume.GetIsPushed();
        is_Menu_Button_Push = button_Menu_Button.GetIsPushed();
        is_Menu_Credit_Push = button_Menu_Credit.GetIsPushed();
    }

    public void GetInput_Skin()
    {
        is_Skin_OK_Push = button_Skin_OK.GetIsPushed();
        is_Skin_Cancel_Push = button_Skin_Cancel.GetIsPushed();
        if (is_Skin_OK_Push) isSkinSelect = true;
        else if (is_Skin_Cancel_Push) isSkinSelect = false;
    }

    public void GetInput_Play()
    {
        is_Play_Pause_Tap = button_Play_Pause.GetIsTapped();
    }

    /* 開発用メソッド */
    public void GetInput_Player()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) InputEvent_Jump_Push();
        if (Input.GetKeyDown(KeyCode.DownArrow)) InputEvent_Squat_Push();
        if (Input.GetKeyUp(KeyCode.DownArrow)) InputEvent_Squat_Release();
        if (Input.GetKeyDown(KeyCode.RightArrow)) InputEvent_Attack_Push();
        if (Input.GetKeyUp(KeyCode.RightArrow)) InputEvent_Attack_Release();
    }
    //Push・Releaseを1フレームのみに制限
    public void ResetInput_Player()
    {
        is_Player_Jump_Push = false;

        //ポーズ状態なら以下はリセットしない
        if (isPauseGame) return;

        is_Player_Squat_Push = false;
        is_Player_Squat_Release = false;
        is_Player_Attack_Push = false;
        is_Player_Attack_Release = false;
    }

    public void GetInput_Pause()
    {
        is_Pause_Continue_Push = button_Pause_Continue.GetIsPushed();
        is_Pause_Retire_Push = button_Pause_Retire.GetIsPushed();
    }

    public void GetInput_Result()
    {
        is_Result_Title_Push = button_Result_Title.GetIsPushed();
        is_Result_Retry_Push = button_Result_Retry.GetIsPushed();
    }

    public void GetInput_Option()
    {
        is_Option_Close_Tap = button_Option_Close.GetIsTapped();
    }
    #endregion




    #region プレイ入力用イベント
    public void BindEvent()
    {
        Button_PlayInput[] playButton = playButtonSet[playButtonPatternNum].playButton;

        for (int i = 0; i < playButton.Length; i++)
        {
            switch (actionAllocation[i])
            {
                case 0: //ジャンプの登録
                    playButton[i].eventOnPush = InputEvent_Jump_Push;
                    playButton[i].eventOnRelease = null;
                    break;

                case 1: //しゃがみ・急降下の登録
                    playButton[i].eventOnPush = InputEvent_Squat_Push;
                    playButton[i].eventOnRelease = InputEvent_Squat_Release;
                    break;

                case 2: //攻撃の登録
                    playButton[i].eventOnPush = InputEvent_Attack_Push;
                    playButton[i].eventOnRelease = InputEvent_Attack_Release;
                    break;
            }
        }
    }

    public void InputEvent_Jump_Push()
    {
        is_Player_Jump_Push = true;
    }

    public void InputEvent_Squat_Push()
    {
        is_Player_Squat_Push = true;
        is_Player_Squat_Hold = true;

        if (isPauseGame)
        {
            is_Player_Squat_Release = false;
            is_Player_Attack_Push = false;
        }
    }

    public void InputEvent_Squat_Release()
    {
        is_Player_Squat_Hold = false;
        is_Player_Squat_Release = true;

        if (isPauseGame)
        {
            is_Player_Squat_Push = false;
        }
    }

    public void InputEvent_Attack_Push()
    {
        is_Player_Attack_Push = true;
        is_Player_Attack_Hold = true;

        if (isPauseGame)
        {
            is_Player_Attack_Release = false;
            is_Player_Squat_Push = false;
        }
    }

    public void InputEvent_Attack_Release()
    {
        is_Player_Attack_Hold = false;
        is_Player_Attack_Release = true;

        if (isPauseGame)
        {
            is_Player_Attack_Push = false;
        }
    }
    #endregion
}



[System.Serializable]
public class PlayButtonSet
{
    public Button_PlayInput[] playButton;
    public SpriteRenderer[] playButtonSprite;
}
