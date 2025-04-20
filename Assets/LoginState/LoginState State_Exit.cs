using UnityEngine;

public class LoginStateState_Exit : LoginStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer screenCover;
    private bool isPlayable;

    public LoginStateState_Exit(LoginStateStateMachine stateMachine) : base(stateMachine)
    {
        screenCover = GM.frontScreenCover;
    }

    public override async void Enter()
    {
        //�f�[�^�̃��[�h�����܂ł̓Q�[�����J�n���Ȃ�
        isPlayable = false;

        //�o�ߎ��Ԃ̃��Z�b�g
        elapsedTime = 0;

        //���O�C���㏉���������s
        await GM.GameInitialize();

        //BGM�𗬂��n�߂�
        AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Menu;
        AudioManager.Instance.audioSource_BGM.Play();

        //���[�f�B���O�L���[�u���\��
        GM.loadingCube.SetActive(false);

        //�Q�[���v���C�\
        isPlayable = true;
    }

    public override void Update()
    {
        //�Q�[���v���C�\�łȂ��Ȃ牽�����Ȃ�
        if (!isPlayable) return;

        //�o�ߎ��Ԃ̉��Z
        elapsedTime += Time.deltaTime;

        //��ʃJ�o�[�̓����x�ύX
        screenCover.color = Color.white - Color.black * elapsedTime/2;

        //2�b�o�߂���΃X�e�[�g�J��
        if (elapsedTime > 2)
            gameStateMachine.ChangeState(gameStateMachine.state_Menu);
    }

    public override void Exit() { }
}
