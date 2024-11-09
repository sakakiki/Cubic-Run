using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static GameManager Instance;

    //�C���X�y�N�^�[����ݒ�\
    public int score = 0;
    public int level = 1;
    public Transform playerTf;
    public PlayerController playerCon;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    //�X�e�[�g�}�V��
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
