using UnityEngine;

public class GameStateState_Tutorial : GameStateStateBase
{
    //�`���[�g���A���p�X�e�[�g�}�V��
    public TutorialStateStateMachine tutorialStateMachine { private set; get; }

    private static float playTimeScale;



    public enum PauseState
    {
        Play,
        Pause,
    }
    public static PauseState currentPauseState { get; private set; }



    public GameStateState_Tutorial(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        tutorialStateMachine = new TutorialStateStateMachine(stateMachine);

        //�|�[�Y��Ԃ�����
        InitializePauseState();
    }



    public override void Enter()
    {
        //�v���C��ʓ���UI�̗L����
        IM.InputUISetActive_Play(true);
        IM.InputUISetActive_Player(true);
        IM.isPauseGame = false;

        //�v���C�p�X�e�[�g�}�V���̐ݒ�
        tutorialStateMachine.Initialize(TutorialStateStateBase.continueState);

        //�v���C���[�̃`���[�g���A���p������
        if (tutorialStateMachine.currentState == tutorialStateMachine.state_Start)
            PlayerStateBase_Play.InitializeTutorial();

        //�n�`������
        TM.moveSpeed = 8;
        TM.SetSpeed(8);

        //�v���C�pBGM���Đ�����Ă��Ȃ���΂̍Đ�
        if (audioSource_BGM.clip != AM.BGM_Play_1)
        {
            audioSource_BGM.volume = AM.volume_BGM;
            audioSource_BGM.clip = AM.BGM_Play_1;
            audioSource_BGM.Play();
            AM.SetBGMSpeed(0.9f);
        }
    }



    public override void Update(float deltaTime)
    {
        switch (currentPauseState)
        {
            case PauseState.Play:

                //�`���[�g���A���p�X�e�[�g�}�V����Update�����s
                tutorialStateMachine.Update(deltaTime);

                //�v���C���UI�̓��͂̎擾
                IM.GetInput_Play();

                //�|�[�Y
                if (IM.is_Play_Pause_Tap)
                {
                    //SE�̍Đ�
                    AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Panel);

                    //�|�[�Y��Ԃ�
                    EnterPause();
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
                    GM.pauseUI.SetActive(false);

                    //�v���C���UI��L����
                    IM.InputUISetActive_Play(true);
                    IM.isPauseGame = false;

                    //�I�u�W�F�N�g�𓮂���
                    Time.timeScale = playTimeScale;

                    //�|�[�Y��Ԃ��I��
                    currentPauseState = PauseState.Play;
                }
                else if (IM.is_Pause_Retire_Push)
                {
                    //�|�[�Y���UI���\���E������
                    IM.InputUISetActive_Pause(false);
                    GM.pauseUI.SetActive(false);

                    //�I�u�W�F�N�g�𓮂���
                    Time.timeScale = 1;

                    //���j���[��ʂ֑J��
                    stateMachine.ChangeState(stateMachine.state_PauseToMenu);
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

        //�|�[�Y��Ԃ�����
        InitializePauseState();

        //������@��\��
        PopupUIManager.Instance.DeleteMessageText();

        //�{�^���\����߂�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.clear;

        //�`���[�g���A���p�X�e�[�g�}�V����Exit�����s
        tutorialStateMachine.currentState.Exit();
    }



    private static void InitializePauseState()
    {
        //�|�[�Y��Ԃ�����
        currentPauseState = PauseState.Play;
    }



    public static void EnterPause()
    {
        //�v���C���UI�𖳌���
        InputManager.Instance.InputUISetActive_Play(false);
        InputManager.Instance.isPauseGame = true;

        //�|�[�Y���UI��\���E�L����
        GameManager.Instance.pauseUI.SetActive(true);
        InputManager.Instance.InputUISetActive_Pause(true);

        //TimeScale��ۑ�
        playTimeScale = Time.timeScale;

        //�I�u�W�F�N�g���~
        Time.timeScale = 0;

        //�|�[�Y��Ԃ�
        currentPauseState = PauseState.Pause;
    }
}
