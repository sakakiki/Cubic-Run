using UnityEngine;

public class GameManager : MonoBehaviour
{
    //自身のインスタンス
    public static GameManager Instance;

    //インスペクター
    public int level = 1;
    public Transform playerTf;
    public PlayerController playerCon;

    //ステートマシン
    private PlayStateStateMachine stateMachine;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    void Start()
    {
        stateMachine = new PlayStateStateMachine();
        stateMachine.Initialize(stateMachine.state_Play);
    }



    void Update()
    {
        stateMachine.Update();
    }
}
