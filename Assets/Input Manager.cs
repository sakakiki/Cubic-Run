using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public bool button_Play_Jump_Push {  get; private set; }
    public bool button_Play_Squat_Push { get; private set; }
    public bool button_Play_Squat_Hold { get; private set; }
    public bool button_Play_Squat_Release { get; private set; }
    public bool button_Play_Attack_Push { get; private set; }
    public bool button_Play_Attack_Hold { get; private set; }
    public bool button_Play_Attack_Release { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void GetInput_Play()
    {
        button_Play_Jump_Push = Input.GetKeyDown(KeyCode.UpArrow);
        button_Play_Squat_Push = Input.GetKeyDown(KeyCode.DownArrow);
        button_Play_Squat_Hold = Input.GetKey(KeyCode.DownArrow);
        button_Play_Squat_Release = Input.GetKeyUp(KeyCode.DownArrow);
        button_Play_Attack_Push = Input.GetKeyDown(KeyCode.RightArrow);
        button_Play_Attack_Hold = Input.GetKey(KeyCode.RightArrow);
        button_Play_Attack_Release = Input.GetKeyUp(KeyCode.RightArrow);
    }
}
