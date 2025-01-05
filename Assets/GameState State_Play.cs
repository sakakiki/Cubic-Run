using UnityEditor;
using UnityEngine;

public class GameStateState_Play : GameStateStateBase
{
    //�v���C�p�X�e�[�g�}�V��
    private PlayStateStateMachine playStateMachine;

    //�|�[�Y�����֌W�ϐ�
    private int countinueCount;
    private int countinueCircleCount;
    private enum PauseState
    {
        Play,
        Pause,
        PauseToPlay
    }
    private static PauseState currentPauseState;



    public GameStateState_Play(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        playStateMachine = new PlayStateStateMachine(stateMachine);
    }



    public override void Enter()
    {
        //�v���C��ʓ���UI�̗L����
        IM.InputUISetActive_Play(true);
        IM.InputUISetActive_Player(true);

        //�v���C�p�X�e�[�g�}�V���̏�����
        playStateMachine.Initialize(playStateMachine.state_LevelStart);

        //�n�`������
        TM.moveSpeed = 8;
        TM.SetSpeed(8);

        //�|�[�Y��Ԃ�����
        InitializePauseState();
    }



    public override void Update(float deltaTime)
    {
        switch (currentPauseState)
        {
            case PauseState.Play:

                //�v���C�p�X�e�[�g�}�V����Update�����s
                playStateMachine.Update(deltaTime);

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
                    stateMachine.ChangeState(stateMachine.state_PauseToMenu);
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



    public override void Exit()
    {
        //�v���C��ʓ���UI�̖�����
        IM.InputUISetActive_Play(false);
        IM.InputUISetActive_Player(false);
    }

    private static void InitializePauseState()
    {
        //�|�[�Y��Ԃ�����
        currentPauseState = PauseState.Play;
    }
}
