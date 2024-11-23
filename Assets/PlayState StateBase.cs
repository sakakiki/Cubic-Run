using TMPro;

public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected PlayerController playerCon;
    protected PlayerStateMachine playerStateMachine;
    protected GameManager GM;
    protected TerrainManager TM;
    public static float playTime;
    private TextMeshProUGUI scoreText;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        GM = GameManager.Instance;
        TM = TerrainManager.Instance;
        playerCon = GM.playerCon;
        playerStateMachine = playerCon.stateMachine;
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

        //�v���C���[��GameOver�X�e�[�g�Ȃ�GameOver�X�e�[�g�ɑJ��
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }

    public abstract void Exit();
}
