using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private Button button_Menu_Play;
    [SerializeField] private Button button_Menu_Skin;
    [SerializeField] private Button button_Menu_Tutorial;
    public bool is_Menu_Play { get; private set; }
    public bool is_Menu_Skin { get; private set; }
    public bool is_Menu_Tutorial { get; private set; }

    public bool is_Play_Jump_Push {  get; private set; }
    public bool is_Play_Squat_Push { get; private set; }
    public bool is_Play_Squat_Hold { get; private set; }
    public bool is_Play_Squat_Release { get; private set; }
    public bool is_Play_Attack_Push { get; private set; }
    public bool is_Play_Attack_Hold { get; private set; }
    public bool is_Play_Attack_Release { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void GetInput_Menu()
    {
        is_Menu_Play = button_Menu_Play.GetIsPushed();
        is_Menu_Skin = button_Menu_Skin.GetIsPushed();
        is_Menu_Tutorial = button_Menu_Tutorial.GetIsPushed();
    }

    public void GetInput_Play()
    {
        is_Play_Jump_Push = Input.GetKeyDown(KeyCode.UpArrow);
        is_Play_Squat_Push = Input.GetKeyDown(KeyCode.DownArrow);
        is_Play_Squat_Hold = Input.GetKey(KeyCode.DownArrow);
        is_Play_Squat_Release = Input.GetKeyUp(KeyCode.DownArrow);
        is_Play_Attack_Push = Input.GetKeyDown(KeyCode.RightArrow);
        is_Play_Attack_Hold = Input.GetKey(KeyCode.RightArrow);
        is_Play_Attack_Release = Input.GetKeyUp(KeyCode.RightArrow);
    }
}
