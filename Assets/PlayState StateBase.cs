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
    private int countinueCount;
    private int countinueCircleCount;
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
                    //�v���C���UI�𖳌���
                    IM.InputUISetActive_Play(false);
                    IM.InputUISetActive_Player(false);

                    //�|�[�Y���UI��\���E�L����
                    GM.PauseUI.SetActive(true);
                    IM.InputUISetActive_Pause(true);

                    //�I�u�W�F�N�g���~
                    Time.timeScale = 0;

                    //�|�[�Y��Ԃ�
                    currentPauseState = PauseState.Pause;
                }
                break;


            case PauseState.Pause:

                //�|�[�Y��ʓ��͂̎擾
                IM.GetInput_Pause();

                //���͂ɉ������J��
                if (IM.is_Pause_Continue_Push)
                {
                    //�|�[�Y���UI���\���E������
                    IM.InputUISetActive_Pause(false);
                    GM.PauseUI.SetActive(false);

                    //�ڕW�t���[�����[�g��30�ɐݒ�
                    Application.targetFrameRate = 30;

                    //�ĊJ�܂ł̃J�E���g�_�E����3��
                    countinueCount = 3;
                    GM.countinueCountText.SetText("" + countinueCount);

                    //�J�E���g�T�[�N���̕\��
                    countinueCircleCount = 30;
                    for (int i = 0; i < GM.countinueCircleSquares.Length; i++)
                        GM.countinueCircleSquares[i].SetActive(true);

                    //�ĊJ�̃J�E���g�_�E���Ɉڍs
                    currentPauseState = PauseState.PauseToPlay;
                }
                else if (IM.is_Pause_Retire_Push)
                {
                    //�|�[�Y���UI���\���E������
                    IM.InputUISetActive_Pause(false);
                    GM.PauseUI.SetActive(false);

                    //�I�u�W�F�N�g�𓮂���
                    Time.timeScale = 1;

                    //���j���[��ʂ֑J��
                    gameStateMachine.ChangeState(gameStateMachine.state_PauseToMenu);
                }
                break;


            case PauseState.PauseToPlay:

                //�J�E���g�T�[�N���̐������炷
                countinueCircleCount--;

                //�܂��J�E���g�T�[�N��������Ȃ��\����
                if (countinueCircleCount >= 0)
                    GM.countinueCircleSquares[countinueCircleCount].SetActive(false);
                //�J�E���g�T�[�N���������Ȃ�
                else
                {
                    //�ĊJ�̃J�E���g�_�E�������炷
                    countinueCount--;

                    //�J�E���g�_�E���������Ȃ�
                    if (countinueCount > 0)
                    {
                        //�ĊJ�̃J�E���g�_�E�����X�V
                        GM.countinueCountText.SetText("" + countinueCount);

                        //�J�E���g�T�[�N���̍ĕ\��
                        countinueCircleCount = 30;
                        for (int i = 0; i < GM.countinueCircleSquares.Length; i++)
                            GM.countinueCircleSquares[i].SetActive(true);
                    }
                    //�J�E���g�_�E�����I���Ȃ�
                    else
                    {
                        //�ĊJ�̃J�E���g�_�E�����\��
                        GM.countinueCountText.SetText("");

                        //�ڕW�t���[�����[�g��60�ɐݒ�
                        Application.targetFrameRate = 60;

                        //�v���C���UI��L����
                        IM.InputUISetActive_Play(true);
                        IM.InputUISetActive_Player(true);

                        //�I�u�W�F�N�g�𓮂���
                        Time.timeScale = 1;

                        //�|�[�Y��Ԃ��I��
                        currentPauseState = PauseState.Play;
                    }
                }
                break;


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
