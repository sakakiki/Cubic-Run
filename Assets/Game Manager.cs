using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int level = 1;
    public GameObject[] ObstacleList;

    private float moveSpeed = 5;
    private Transform previousObstacleTf;
    private int createObstacleNum;
    private int previousObstacleNum1;
    private int previousObstacleNum2;
    private float stageRightEdge;
    private float obstacleWidth;
    private float obstacleHeight;
    private Queue<Transform> obstacleTfQueue = new Queue<Transform>();

    void Start()
    {
        //速度設定
        moveSpeed = 5 + Mathf.Pow(level, 0.7f) * 3;

        //初期地面生成
        CreateObstacle(0, 0, 15, 1, moveSpeed);
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
                    }
                    break;
            }
            stageRightEdge = previousObstacleTf.position.x;
            obstacleWidth = Random.Range(level * 0.2f + 6.5f, level * 0.2f + 7.5f);
            CreateObstacle(0, stageRightEdge, obstacleWidth, 1, moveSpeed);
        }

        if (obstacleTfQueue.Peek().position.x < -5)
            Destroy(obstacleTfQueue.Dequeue().gameObject);
    }

    // 障害物生成メソッド
    private void CreateObstacle(int obstacleNum, float leftEdgePosX, float width, float height, float moveSpeed)
    {
        previousObstacleTf = Instantiate(ObstacleList[obstacleNum], Vector2.right * (leftEdgePosX + width), Quaternion.identity).transform;
        previousObstacleTf.localScale = new Vector3(width, height, 1);
        previousObstacleTf.GetComponent<Rigidbody2D>().velocity = Vector3.left * moveSpeed;
        obstacleTfQueue.Enqueue(previousObstacleTf);
    }
}
