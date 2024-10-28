using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static GameManager Instance;

    //���O�L���b�V��
    public int level = 1;
    public GameObject[] ObstacleList;
    public Transform playerTf;
    public PlayerController playerCon;

    private PlayStateStateMachine stateMachine;

    //�ėp�ϐ�
    public float moveSpeed {  get; private set; }
    [HideInInspector] public Transform previousObstacleTf;
    [HideInInspector] public float stageRightEdge;

    //�I�u�W�F�N�g�v�[���֘A�ϐ�
    [HideInInspector] public ObjectPool<GameObject>[] pool;
    private int poolNum;

    //CreateObstacleRandom���\�b�h�p�ϐ�
    private int previousObstacleNum1;
    private int previousObstacleNum2;

    //Obstacle�Ǘ��p�ϐ�
    [HideInInspector] public Queue<Transform> activeObstacleTfQueue = new Queue<Transform>();
    [HideInInspector] public Queue<int> activeObstacleNumQueue = new Queue<int>();
    private Queue<Transform> approachingObstacleTfQueue = new Queue<Transform>();
    private Queue<int> approachingObstacleNumQueue = new Queue<int>();
    [HideInInspector] public Transform currentObstacleTf;
    public int currentObstacleNum { get; private set; } //�Q�[���I�[�o�[�����œǂݎ��
    [HideInInspector] public int nextObstacleNum;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    void Start()
    {
        stateMachine = new PlayStateStateMachine();
        stateMachine.Initialize(stateMachine.state_Play);

        //���x�ݒ�
        moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;

        //�I�u�W�F�N�g�v�[���쐬
        pool = new ObjectPool<GameObject>[ObstacleList.Length];
        for (int i = 0; i < ObstacleList.Length; i++)
        {
            poolNum = i;
            pool[poolNum] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(ObstacleList[poolNum]),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: true);
        }

        //�����n�ʐ���
        CreateObstacle(0, 0, 5, 1, moveSpeed);
        CreateObstacle(0, previousObstacleTf.position.x, 5, 1, moveSpeed);
        CreateObstacle(0, previousObstacleTf.position.x, 5, 1, moveSpeed);
        nextObstacleNum = approachingObstacleNumQueue.Dequeue();
        UpdateCurrentObstacle();
    }



    void Update()
    {
        stateMachine.Update();
    }



    public void CreateObstacleRandom()
    {
        //���������Q���̎�ނ�����
        int createObstacleNum;
        do createObstacleNum = Random.Range(1, ObstacleList.Length);
        while (createObstacleNum == previousObstacleNum1 || createObstacleNum == previousObstacleNum2);
        previousObstacleNum2 = previousObstacleNum1;
        previousObstacleNum1 = createObstacleNum;

        //createObstacleNum = 4;

        //�傫���������_���Ɍ��肵��Q���𐶐�
        float obstacleWidth;
        float obstacleHeight;
        switch (createObstacleNum)
        {
            case 1:
                obstacleWidth = Random.Range(level * 0.5f + 2.5f, moveSpeed * 0.5f);
                CreateObstacle(createObstacleNum, stageRightEdge, obstacleWidth, 1, moveSpeed);
                break;
            case 3:
            case 5:
                obstacleWidth = Random.Range(level * 0.2f + 2, level * 0.2f + 6);
                CreateObstacle(createObstacleNum, stageRightEdge, obstacleWidth, 1, moveSpeed);
                break;
            case 2:
                obstacleWidth = Random.Range(level * 0.2f + 4.5f, level * 0.2f + 6.5f);
                obstacleHeight = Random.Range(1.0f, 4.0f);
                CreateObstacle(createObstacleNum, stageRightEdge, obstacleWidth, obstacleHeight, moveSpeed);
                stageRightEdge = previousObstacleTf.position.x;
                obstacleWidth = Random.Range(level * 0.2f + 3.0f, level * 0.2f + 6.0f);
                obstacleHeight = Random.Range(2.0f, 4.0f);
                obstacleHeight += (obstacleHeight > previousObstacleTf.localScale.y) ? 1 : -1;
                CreateObstacle(createObstacleNum, stageRightEdge, obstacleWidth, obstacleHeight, moveSpeed);
                break;
            case 4:
                int number = Random.Range(1, 6);
                for (int i = 0; i < number; i++)
                {
                    CreateObstacle(createObstacleNum, stageRightEdge, 1, 1, moveSpeed);
                    stageRightEdge = previousObstacleTf.position.x;
                }
                break;
        }

        //��Q���Ԃ̒n�ʂ𐶐�
        stageRightEdge = previousObstacleTf.position.x;
        obstacleWidth = Random.Range(level * 0.2f + 6.5f, level * 0.2f + 7.5f);
        CreateObstacle(0, stageRightEdge, obstacleWidth, 1, moveSpeed);
    }



    public void CreateObstacle(int obstacleNum, float leftEdgePosX, float width, float height, float moveSpeed)
    {
        //�I�u�W�F�N�g�v�[�������Q���𐶐�
        poolNum = obstacleNum;
        previousObstacleTf = pool[poolNum].Get().transform;

        //�ʒu�ƃX�P�[���𒲐������x��t�^
        previousObstacleTf.position = Vector2.right * (leftEdgePosX + width);
        previousObstacleTf.localScale = new Vector3(width, height, 1);
        previousObstacleTf.GetComponent<Rigidbody2D>().velocity = Vector3.left * moveSpeed;

        //�Ǘ��pQueue�ɓo�^
        activeObstacleTfQueue.Enqueue(previousObstacleTf);
        activeObstacleNumQueue.Enqueue(obstacleNum);
        approachingObstacleTfQueue.Enqueue(previousObstacleTf);
        approachingObstacleNumQueue.Enqueue(obstacleNum);
    }



    //�v���C���[�̈ʒu�̒n�ʂ̎�ނ��X�V
    public void UpdateCurrentObstacle()
    {
        currentObstacleTf = approachingObstacleTfQueue.Dequeue();
        currentObstacleNum = nextObstacleNum;
        nextObstacleNum = approachingObstacleNumQueue.Dequeue();
    }



    //�S�Ă̏�Q�����~
    public void StopObstacle()
    {
        foreach (Transform obstacleTf in activeObstacleTfQueue)
            obstacleTf.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
