using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class TerrainManager : MonoBehaviour
{
    //自身のインスタンス
    public static TerrainManager Instance;

    //インスペクターから登録
    public GameObject[] TerrainList;

    //汎用変数
    private GameManager GM;
    private Transform playerTf;
    public float moveSpeed;
    public bool isCreateObstacle;
    private Transform previousTerrainTf;
    private float stageRightEdge;

    //オブジェクトプール関連変数
    private ObjectPool<GameObject>[] pool;
    private int poolNum;

    //CreateTerrainRandomメソッド用変数
    private int previousTerrainNum1;
    private int previousTerrainNum2;

    //Terrain管理用変数
    private Queue<Transform> activeTerrainTfQueue = new Queue<Transform>();
    private Queue<int> activeTerrainNumQueue = new Queue<int>();
    private Queue<Transform> approachingTerrainTfQueue = new Queue<Transform>();
    private Queue<int> approachingTerrainNumQueue = new Queue<int>();
    private Transform currentTerrainTf;
    public int currentTerrainNum { get; private set; } //ゲームオーバー処理で読み取り
    private int nextTerrainNum;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    void Start()
    {
        GM = GameManager.Instance;
        playerTf = GM.playerTf;

        //オブジェクトプール作成
        pool = new ObjectPool<GameObject>[TerrainList.Length];
        for (int i = 0; i < TerrainList.Length; i++)
        {
            poolNum = i;
            pool[poolNum] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(TerrainList[poolNum]),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: true);
        }

        //初期速度設定
        moveSpeed = 8;

        //初期地面生成
        CreateTerrain(0, 0, 5, 1, moveSpeed);
        CreateTerrain(0, previousTerrainTf.position.x, 5, 1, moveSpeed);
        CreateTerrain(0, previousTerrainTf.position.x, 5, 1, moveSpeed);
        nextTerrainNum = approachingTerrainNumQueue.Dequeue();
        UpdateCurrentTerrain();
    }



    public void ManageMovingTerrain()
    {
        //ステージ右端の座標を取得
        stageRightEdge = previousTerrainTf.position.x;

        //ステージの端が近づいていれば地形を追加で生成
        if (stageRightEdge < 25)
        {
            if (isCreateObstacle) CreateTerrainRandom();
            else CreateTerrain(0, stageRightEdge, 3, 1, moveSpeed);
        }

        //プレイヤーの位置の地面の種類の更新必要性の確認
        switch (nextTerrainNum)
        {
            case 0:
            case 1:
                if (currentTerrainTf.position.x < -playerTf.localScale.x / 2)
                    UpdateCurrentTerrain();
                break;

            case 2:
            case 4:
            case 5:
                if (currentTerrainTf.position.x < playerTf.localScale.x / 2)
                    UpdateCurrentTerrain();
                break;

            case 3:
                if (currentTerrainTf.position.x < 0)
                    UpdateCurrentTerrain();
                break;
        }

        //不要地形削除処理
        if (activeTerrainTfQueue.Peek().position.x < -5)
            pool[activeTerrainNumQueue.Dequeue()].Release(activeTerrainTfQueue.Dequeue().gameObject);
    }



    public void CreateTerrainRandom()
    {
        //生成する障害物の種類を決定
        int createTerrainNum;
        do createTerrainNum = Random.Range(1, TerrainList.Length);
        while (createTerrainNum == previousTerrainNum1 || createTerrainNum == previousTerrainNum2);
        previousTerrainNum2 = previousTerrainNum1;
        previousTerrainNum1 = createTerrainNum;

        //大きさをランダムに決定し障害物を生成
        float terrainWidth;
        float terrainHeight;
        switch (createTerrainNum)
        {
            case 1:
                terrainWidth = Random.Range(GM.level * 0.5f + 2.5f, moveSpeed * 0.5f);
                CreateTerrain(createTerrainNum, stageRightEdge, terrainWidth, 1, moveSpeed);
                break;
            case 3:
            case 5:
                terrainWidth = Random.Range(GM.level * 0.2f + 2, GM.level * 0.2f + 6);
                CreateTerrain(createTerrainNum, stageRightEdge, terrainWidth, 1, moveSpeed);
                break;
            case 2:
                terrainWidth = Random.Range(GM.level * 0.2f + 4.5f, GM.level * 0.2f + 6.5f);
                terrainHeight = Random.Range(1.0f, 4.0f);
                CreateTerrain(createTerrainNum, stageRightEdge, terrainWidth, terrainHeight, moveSpeed);
                stageRightEdge = previousTerrainTf.position.x;
                terrainWidth = Random.Range(GM.level * 0.2f + 3.0f, GM.level * 0.2f + 6.0f);
                terrainHeight = Random.Range(2.0f, 4.0f);
                terrainHeight += (terrainHeight > previousTerrainTf.localScale.y) ? 1 : -1;
                CreateTerrain(createTerrainNum, stageRightEdge, terrainWidth, terrainHeight, moveSpeed);
                break;
            case 4:
                int number = Random.Range(1, 6);
                for (int i = 0; i < number; i++)
                {
                    CreateTerrain(createTerrainNum, stageRightEdge, 1, 1, moveSpeed);
                    stageRightEdge = previousTerrainTf.position.x;
                }
                break;
        }

        //障害物間の地面を生成
        stageRightEdge = previousTerrainTf.position.x;
        terrainWidth = Random.Range(GM.level * 0.2f + 6.5f, GM.level * 0.2f + 7.5f);
        CreateTerrain(0, stageRightEdge, terrainWidth, 1, moveSpeed);
    }



    public void CreateTerrain(int terrainNum, float leftEdgePosX, float width, float height, float moveSpeed)
    {
        //オブジェクトプールから地形を生成
        poolNum = terrainNum;
        previousTerrainTf = pool[poolNum].Get().transform;

        //位置とスケールを調整し速度を付与
        previousTerrainTf.position = Vector2.right * (leftEdgePosX + width);
        previousTerrainTf.localScale = new Vector3(width, height, 1);
        previousTerrainTf.GetComponent<Rigidbody2D>().velocity = Vector3.left * moveSpeed;

        //管理用Queueに登録
        activeTerrainTfQueue.Enqueue(previousTerrainTf);
        activeTerrainNumQueue.Enqueue(terrainNum);
        approachingTerrainTfQueue.Enqueue(previousTerrainTf);
        approachingTerrainNumQueue.Enqueue(terrainNum);
    }



    //プレイヤーの位置の地面の種類を更新
    public void UpdateCurrentTerrain()
    {
        currentTerrainTf = approachingTerrainTfQueue.Dequeue();
        currentTerrainNum = nextTerrainNum;
        nextTerrainNum = approachingTerrainNumQueue.Dequeue();
    }



    //全ての地形の速度を変更
    public void SetSpeed(float speed)
    {
        foreach (Transform terrainTf in activeTerrainTfQueue)
            terrainTf.GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
    }
}
