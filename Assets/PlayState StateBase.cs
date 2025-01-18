using TMPro;
using UnityEngine;

public abstract class PlayStateStateBase
{
    protected PlayStateStateMachine stateMachine;
    protected GameStateStateMachine gameStateMachine;
    protected GameManager GM;
    protected InputManager IM;
    protected TerrainManager TM;
    public static float playTime;
    private TextMeshProUGUI scoreText;
    public static int scoreCorrection;
    private Transform scoreGageTf;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameStateMachine = stateMachine.gameStateMachine;
        GM = GameManager.Instance;
        IM = InputManager.Instance;
        TM = TerrainManager.Instance;
        scoreText = GM.scoreText;
        scoreGageTf = GM.scoreGageTf;
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

        //�X�R�A�Q�[�W����
        scoreGageTf.localScale = 
            Vector3.right + Vector3.forward + 
            Vector3.up * (float)(GM.score % GM.levelUpSpan)/ (float)GM.levelUpSpan * 2;
    }

    public abstract void Exit();
}
