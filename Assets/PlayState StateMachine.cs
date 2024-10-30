using UnityEngine;

public class PlayStateStateMachine
{
    public PlayStateStateBase currentState {  get; private set; }

    public PlayStateStateBase state_Play;


    public TerrainManager TM { get; private set; }
    protected Transform playerTf;

    public PlayStateStateMachine()
    {
        TM = TerrainManager.Instance;
        playerTf = GameManager.Instance.playerTf;

        state_Play = new PlayStateState_Play(this);
    }

    public void Initialize(PlayStateStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
    }

    public void ChangeState(PlayStateStateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }

    public void Update()
    {
        TM.stageRightEdge = TM.previousObstacleTf.position.x;

        //ステートに応じたUpdateの実行
        if (currentState != null) currentState.Update();

        //プレイヤーの位置の地面の種類の更新必要性の確認
        switch (TM.nextTerrainNum)
        {
            case 0:
            case 1:
                if (TM.currentTerrainTf.position.x < -playerTf.localScale.x / 2)
                    TM.UpdateCurrentTerrain();
                break;

            case 2:
            case 4:
            case 5:
                if (TM.currentTerrainTf.position.x < playerTf.localScale.x / 2)
                    TM.UpdateCurrentTerrain();
                break;

            case 3:
                if (TM.currentTerrainTf.position.x < 0)
                    TM.UpdateCurrentTerrain();
                break;
        }

        //不要Obstacle削除処理
        if (TM.activeTerrainTfQueue.Peek().position.x < -5)
            TM.pool[TM.activeTerrainNumQueue.Dequeue()].Release(TM.activeTerrainTfQueue.Dequeue().gameObject);
    }
}
