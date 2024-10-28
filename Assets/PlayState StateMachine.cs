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

        //�X�e�[�g�ɉ�����Update�̎��s
        if (currentState != null) currentState.Update();

        //�v���C���[�̈ʒu�̒n�ʂ̎�ނ̍X�V�K�v���̊m�F
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

        //�s�vObstacle�폜����
        if (GM.activeObstacleTfQueue.Peek().position.x < -5)
            GM.pool[GM.activeObstacleNumQueue.Dequeue()].Release(GM.activeObstacleTfQueue.Dequeue().gameObject);
    }
}
