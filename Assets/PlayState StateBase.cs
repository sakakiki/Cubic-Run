using TMPro;

public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected GameStateStateMachine gameStateMachine;
    protected GameManager GM;
    protected TerrainManager TM;
    public static float playTime;
    private TextMeshProUGUI scoreText;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameStateMachine = stateMachine.gameStateMachine;
        GM = GameManager.Instance;
        TM = TerrainManager.Instance;
        scoreText = GM.scoreText;
    }

    public abstract void Enter();

    public virtual void Update(float deltaTime)
    {
        //�v���C���ԉ��Z
        playTime += deltaTime;

        //�ړ����̒n�`���Ǘ�
        TM.ManageMovingTerrain();

        //�X�R�A�X�V
        GM.score = (int)(playTime * 100);
        scoreText.SetText("" + GM.score);
    }

    public abstract void Exit();
}
