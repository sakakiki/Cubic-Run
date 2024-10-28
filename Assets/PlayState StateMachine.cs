using UnityEngine;

public class PlayStateStateMachine
{
    public PlayStateStateBase currentState {  get; private set; }

    public PlayStateStateBase state_Play;


    public GameManager GM { get; private set; }
    protected Transform playerTf;

    public PlayStateStateMachine()
    {
        GM = GameManager.Instance;
        playerTf = GM.playerTf;

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
        GM.stageRightEdge = GM.previousObstacleTf.position.x;

        //ステートに応じたUpdateの実行
        if (currentState != null) currentState.Update();

        //プレイヤーの位置の地面の種類の更新必要性の確認
        switch (GM.nextObstacleNum)
        {
            case 0:
            case 1:
                if (GM.currentObstacleTf.position.x < -playerTf.localScale.x / 2)
                    GM.UpdateCurrentObstacle();
                break;

            case 2:
            case 4:
            case 5:
                if (GM.currentObstacleTf.position.x < playerTf.localScale.x / 2)
                    GM.UpdateCurrentObstacle();
                break;

            case 3:
                if (GM.currentObstacleTf.position.x < 0)
                    GM.UpdateCurrentObstacle();
                break;
        }

        //不要Obstacle削除処理
        if (GM.activeObstacleTfQueue.Peek().position.x < -5)
            GM.pool[GM.activeObstacleNumQueue.Dequeue()].Release(GM.activeObstacleTfQueue.Dequeue().gameObject);
    }
}
