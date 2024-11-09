using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //自身のインスタンス
    public static GameManager Instance;

    //インスペクターから設定可能
    public int score = 0;
    public int level = 1;
    public Transform playerTf;
    public PlayerController playerCon;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

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
