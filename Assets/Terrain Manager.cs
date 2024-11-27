using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class TerrainManager : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static TerrainManager Instance;

    //�C���X�y�N�^�[����o�^
    public GameObject[] TerrainList;

    //�ėp�ϐ�
    private GameManager GM;
    private Transform playerTf;
    public float moveSpeed;
    public bool isCreateObstacle;
    private Transform previousTerrainTf;
    private float stageRightEdge;

    //�I�u�W�F�N�g�v�[���֘A�ϐ�
    private ObjectPool<GameObject>[] pool;
    private int poolNum;

    //CreateTerrainRandom���\�b�h�p�ϐ�
    private int previousTerrainNum1;
    private int previousTerrainNum2;

    //Terrain�Ǘ��p�ϐ�
    private Queue<Transform> activeTerrainTfQueue = new Queue<Transform>();
    private Queue<int> activeTerrainNumQueue = new Queue<int>();
    private Queue<Transform> approachingTerrainTfQueue = new Queue<Transform>();
    private Queue<int> approachingTerrainNumQueue = new Queue<int>();
    private Transform currentTerrainTf;
    public int currentTerrainNum { get; private set; } //�Q�[���I�[�o�[�����œǂݎ��
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

        //�I�u�W�F�N�g�v�[���쐬
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

        //�������x�ݒ�
        moveSpeed = 8;

        //�����n�ʐ���
        CreateTerrain(0, 0, 5, 1, moveSpeed);
        CreateTerrain(0, previousTerrainTf.position.x, 5, 1, moveSpeed);
        CreateTerrain(0, previousTerrainTf.position.x, 5, 1, moveSpeed);
        nextTerrainNum = approachingTerrainNumQueue.Dequeue();
        UpdateCurrentTerrain();
    }



    public void ManageMovingTerrain()
    {
        //�X�e�[�W�E�[�̍��W���擾
        stageRightEdge = previousTerrainTf.position.x;

        //�X�e�[�W�̒[���߂Â��Ă���Βn�`��ǉ��Ő���
        if (stageRightEdge < 25)
        {
            if (isCreateObstacle) CreateTerrainRandom();
            else CreateTerrain(0, stageRightEdge, 3, 1, moveSpeed);
        }

        //�v���C���[�̈ʒu�̒n�ʂ̎�ނ̍X�V�K�v���̊m�F
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

        //�s�v�n�`�폜����
        if (activeTerrainTfQueue.Peek().position.x < -5)
            pool[activeTerrainNumQueue.Dequeue()].Release(activeTerrainTfQueue.Dequeue().gameObject);
    }



    public void CreateTerrainRandom()
    {
        //���������Q���̎�ނ�����
        int createTerrainNum;
        do createTerrainNum = Random.Range(1, TerrainList.Length);
        while (createTerrainNum == previousTerrainNum1 || createTerrainNum == previousTerrainNum2);
        previousTerrainNum2 = previousTerrainNum1;
        previousTerrainNum1 = createTerrainNum;

        //�傫���������_���Ɍ��肵��Q���𐶐�
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

        //��Q���Ԃ̒n�ʂ𐶐�
        stageRightEdge = previousTerrainTf.position.x;
        terrainWidth = Random.Range(GM.level * 0.2f + 6.5f, GM.level * 0.2f + 7.5f);
        CreateTerrain(0, stageRightEdge, terrainWidth, 1, moveSpeed);
    }



    public void CreateTerrain(int terrainNum, float leftEdgePosX, float width, float height, float moveSpeed)
    {
        //�I�u�W�F�N�g�v�[������n�`�𐶐�
        poolNum = terrainNum;
        previousTerrainTf = pool[poolNum].Get().transform;

        //�ʒu�ƃX�P�[���𒲐������x��t�^
        previousTerrainTf.position = Vector2.right * (leftEdgePosX + width);
        previousTerrainTf.localScale = new Vector3(width, height, 1);
        previousTerrainTf.GetComponent<Rigidbody2D>().velocity = Vector3.left * moveSpeed;

        //�Ǘ��pQueue�ɓo�^
        activeTerrainTfQueue.Enqueue(previousTerrainTf);
        activeTerrainNumQueue.Enqueue(terrainNum);
        approachingTerrainTfQueue.Enqueue(previousTerrainTf);
        approachingTerrainNumQueue.Enqueue(terrainNum);
    }



    //�v���C���[�̈ʒu�̒n�ʂ̎�ނ��X�V
    public void UpdateCurrentTerrain()
    {
        currentTerrainTf = approachingTerrainTfQueue.Dequeue();
        currentTerrainNum = nextTerrainNum;
        nextTerrainNum = approachingTerrainNumQueue.Dequeue();
    }



    //�S�Ă̒n�`�̑��x��ύX
    public void SetSpeed(float speed)
    {
        foreach (Transform terrainTf in activeTerrainTfQueue)
            terrainTf.GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
    }
}
