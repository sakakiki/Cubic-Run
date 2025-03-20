using TMPro;
using UnityEngine;

public abstract class TutorialStateStateBase
{
    protected TutorialStateStateMachine stateMachine;
    protected GameStateStateMachine gameStateMachine;
    protected GameManager GM;
    protected InputManager IM;
    protected TerrainManager TM;
    protected float elapsedTime;
    protected TextMeshProUGUI scoreText;
    protected Transform scoreGageTf;
    public static TutorialStateStateBase continueState;



    public TutorialStateStateBase(TutorialStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameStateMachine = stateMachine.gameStateMachine;
        GM = GameManager.Instance;
        IM = InputManager.Instance;
        TM = TerrainManager.Instance;
        scoreText = GM.scoreText;
        scoreGageTf = GM.scoreGageTf;
    }

    public virtual void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;
    }

    public virtual void Update(float deltaTime)
    {
        //�o�ߎ��Ԃ����Z
        elapsedTime += deltaTime;

        //�ړ����̒n�`���Ǘ�
        TM.ManageMovingTerrain();
    }

    public abstract void Exit();
}
