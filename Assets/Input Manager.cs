using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public bool jumpButtonPush {  get; private set; }
    public bool squatButtonPush { get; private set; }
    public bool squatButtonHold { get; private set; }
    public bool squatButtonRelease { get; private set; }
    public bool attackButtonPush { get; private set; }
    public bool attackButtonHold { get; private set; }
    public bool attackButtonRelease { get; private set; }

    public InputManager()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    public void GetInput()
    {
        jumpButtonPush = Input.GetKeyDown(KeyCode.UpArrow);
        squatButtonPush = Input.GetKeyDown(KeyCode.DownArrow);
        squatButtonHold = Input.GetKey(KeyCode.DownArrow);
        squatButtonRelease = Input.GetKeyUp(KeyCode.DownArrow);
        attackButtonPush = Input.GetKeyDown(KeyCode.RightArrow);
        attackButtonHold = Input.GetKey(KeyCode.RightArrow);
        attackButtonRelease = Input.GetKeyUp(KeyCode.RightArrow);
    }
}
