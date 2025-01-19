using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] GameObject screenButtonObj;
    [SerializeField] private Button_Tap button_Screen;
    [SerializeField] private Button_Push button_Menu_Play;
    [SerializeField] private Button_Push button_Menu_Skin;
    [SerializeField] private Button_Push button_Menu_Tutorial;
    [SerializeField] private Button_Tap button_Play_Pause;
    private bool isInputPlayerActive = true;
    [SerializeField] private Button_Push button_Pause_Continue;
    [SerializeField] private Button_Push button_Pause_Retire;
    [SerializeField] private Button_Push button_Result_Title;
    [SerializeField] private Button_Push button_Result_Retry;

    public bool is_Screen_Tap { get; private set; }

    public bool is_Menu_Play_Push { get; private set; }
    public bool is_Menu_Skin_Push { get; private set; }
    public bool is_Menu_Tutorial_Push { get; private set; }

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

    public bool is_Result_Title_Tap { get; private set; }
    public bool is_Result_Retry_Tap { get; private set; }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    //SetActiveメソッド
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
    }

    public void InputUISetActive_Play(bool isActive)
    {
        button_Play_Pause.enabled = isActive;
    }

    public void InputUISetActive_Player(bool isActive)
    {
        isInputPlayerActive = isActive;
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



    //GetInputメソッド
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
    }

    public void GetInput_Play()
    {
        is_Play_Pause_Tap = button_Play_Pause.GetIsTapped();
    }

    public void GetInput_Player()
    {
        if (!isInputPlayerActive) return;
        is_Player_Jump_Push = Input.GetKeyDown(KeyCode.UpArrow);
        is_Player_Squat_Push = Input.GetKeyDown(KeyCode.DownArrow);
        is_Player_Squat_Hold = Input.GetKey(KeyCode.DownArrow);
        is_Player_Squat_Release = Input.GetKeyUp(KeyCode.DownArrow);
        is_Player_Attack_Push = Input.GetKeyDown(KeyCode.RightArrow);
        is_Player_Attack_Hold = Input.GetKey(KeyCode.RightArrow);
        is_Player_Attack_Release = Input.GetKeyUp(KeyCode.RightArrow);
    }

    public void GetInput_Pause()
    {
        is_Pause_Continue_Push = button_Pause_Continue.GetIsPushed();
        is_Pause_Retire_Push = button_Pause_Retire.GetIsPushed();
    }

    public void GetInput_Result()
    {
        is_Result_Title_Tap = button_Result_Title.GetIsPushed();
        is_Result_Retry_Tap = button_Result_Retry.GetIsPushed();
    }
}
