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
        //経過時間リセット
        elapsedTime = 0;
    }

    public virtual void Update(float deltaTime)
    {
        //経過時間を加算
        elapsedTime += deltaTime;

        //移動中の地形を管理
        TM.ManageMovingTerrain();
    }

    public abstract void Exit();
}
