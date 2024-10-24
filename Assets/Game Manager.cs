using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int level = 1;
    public GameObject[] ObstacleList;
    public Transform playerTf;
    public PlayerController playerCon;

    public float moveSpeed {  get; private set; }
    private Transform previousObstacleTf;
    private int createObstacleNum;
    private int previousObstacleNum1;
    private int previousObstacleNum2;
    private float stageRightEdge;
    private float obstacleWidth;
    private float obstacleHeight;
    private Queue<Transform> activeObstacleTfQueue = new Queue<Transform>();
    private Queue<int> activeObstacleNumQueue = new Queue<int>();

    private ObjectPool<GameObject>[] pool;
    private int poolNum;

    private Queue<Transform> approachingObstacleTfQueue = new Queue<Transform>();
    private Queue<int> approachingObstacleNumQueue = new Queue<int>();
    private Transform currentObstacleTf;
    public int currentObstacleNum;
    private int nextObstacleNum;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        //速度設定
        moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;

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

        //初期地面生成
        CreateObstacle(0, 0, 5, 1, moveSpeed);
        CreateObstacle(0, previousObstacleTf.position.x, 5, 1, moveSpeed);
        CreateObstacle(0, previousObstacleTf.position.x, 5, 1, moveSpeed);
        nextObstacleNum = approachingObstacleNumQueue.Dequeue();
        UpdateCurrentObstacle();
    }

    void Update()
    {
        stageRightEdge = previousObstacleTf.position.x;

        if (stageRightEdge < 30)
        {
            do
            {
                createObstacleNum = Random.Range(1, ObstacleList.Length);
            } 
            while (createObstacleNum == previousObstacleNum1 || createObstacleNum == previousObstacleNum2);
            previousObstacleNum2 = previousObstacleNum1;
            previousObstacleNum1 = createObstacleNum;

            //createObstacleNum = 4;

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
            stageRightEdge = previousObstacleTf.position.x;
            obstacleWidth = Random.Range(level * 0.2f + 6.5f, level * 0.2f + 7.5f);
            CreateObstacle(0, stageRightEdge, obstacleWidth, 1, moveSpeed);
        }

        //プレイヤーの位置の地面の種類の更新必要性の確認
        switch (nextObstacleNum)
        {
            case 0:
            case 1:
                if (currentObstacleTf.position.x < - playerTf.localScale.x / 2)
                    UpdateCurrentObstacle();
                break;

            case 2:
            case 4:
            case 5:
                if (currentObstacleTf.position.x < playerTf.localScale.x / 2)
                    UpdateCurrentObstacle();
                break;

            case 3:
                if (currentObstacleTf.position.x < 0)
                    UpdateCurrentObstacle();
                break;
        }

        //障害物削除処理
        if (activeObstacleTfQueue.Peek().position.x < -5)
            pool[activeObstacleNumQueue.Dequeue()].Release(activeObstacleTfQueue.Dequeue().gameObject);
    }

    // 障害物生成メソッド
    private void CreateObstacle(int obstacleNum, float leftEdgePosX, float width, float height, float moveSpeed)
    {
        poolNum = obstacleNum;
        previousObstacleTf = pool[poolNum].Get().transform;
        previousObstacleTf.position = Vector2.right * (leftEdgePosX + width);
        previousObstacleTf.localScale = new Vector3(width, height, 1);
        previousObstacleTf.GetComponent<Rigidbody2D>().velocity = Vector3.left * moveSpeed;
        activeObstacleTfQueue.Enqueue(previousObstacleTf);
        activeObstacleNumQueue.Enqueue(obstacleNum);
        approachingObstacleTfQueue.Enqueue(previousObstacleTf);
        approachingObstacleNumQueue.Enqueue(obstacleNum);
    }

    //プレイヤーの位置の地面の種類を更新
    public void UpdateCurrentObstacle()
    {
        currentObstacleTf = approachingObstacleTfQueue.Dequeue();
        currentObstacleNum = nextObstacleNum;
        nextObstacleNum = approachingObstacleNumQueue.Dequeue();
    }

    public void StopObstacle()
    {
        foreach (Transform obstacleTf in activeObstacleTfQueue)
            obstacleTf.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
