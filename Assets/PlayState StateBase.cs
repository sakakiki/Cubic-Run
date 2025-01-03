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
    private enum PauseState
    {
        Play,
        Pause,
        PauseToPlay
    }
    private static PauseState currentPauseState;

    public PlayStateStateBase(PlayStateStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        gameStateMachine = stateMachine.gameStateMachine;
        GM = GameManager.Instance;
        IM = InputManager.Instance;
        TM = TerrainManager.Instance;
        scoreText = GM.scoreText;
    }

    public abstract void Enter();

    public virtual void Update(float deltaTime)
    {
        switch (currentPauseState)
        {
            case PauseState.Play:

                //�v���C���ԉ��Z
                playTime += deltaTime;

                //�ړ����̒n�`���Ǘ�
                TM.ManageMovingTerrain();

                //�X�R�A�X�V
                GM.score = (int)(playTime * 100);
                scoreText.SetText("" + GM.score);

                //�v���C���UI�̓��͂̎擾
                IM.GetInput_Play();
                
                //�|�[�Y
                if (IM.is_Play_Pause_Tap)
                {
                    GM.PauseUI.SetActive(true);
                    IM.InputUISetActive_Play(false);
                    IM.InputUISetActive_Player(false);
                    IM.InputUISetActive_Pause(true);
                    Time.timeScale = 0;
                    currentPauseState = PauseState.Pause;
                }
                break;


            case PauseState.Pause:

                //�|�[�Y��ʓ��͂̎擾
                IM.GetInput_Pause();

                //���͂ɉ������J��
                if (IM.is_Pause_Continue_Push)
                {
                    IM.InputUISetActive_Pause(false);
                    IM.InputUISetActive_Play(true);
                    IM.InputUISetActive_Player(true);
                    GM.PauseUI.SetActive(false);
                    Time.timeScale = 1;
                    currentPauseState = PauseState.Play;
                }
                else if (IM.is_Pause_Retire_Push)
                {

                }
                break;


            case PauseState.PauseToPlay: break;


            default: break;
        }
    }

    public abstract void Exit();

    public static void InitializePauseState()
    {
        //�|�[�Y��Ԃ�����
        currentPauseState = PauseState.Play;
    }
}
