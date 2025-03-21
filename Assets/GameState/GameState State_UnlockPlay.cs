using TMPro;
using UnityEngine;

public class GameStateState_UnlockPlay : GameStateStateBase
{
    private float elapsedTime;
    private RectTransform[] unlockPlayModeRtf;
    private TextMeshProUGUI unlockPlayMessage;

    public GameStateState_UnlockPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        unlockPlayModeRtf = GM.unlockPlayModeRtf;
        unlockPlayMessage = GM.unlockPlayMessage;
    }


    public override void Enter()
    {
        //�v���C���[�h�A�����b�N��ʂ̗L����
        GM.unlockPlay.SetActive(true);

        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //��ʃ^�b�v���o�{�^���̗L����
        IM.InputUISetActive_Screen(true);

        //�X�P�[���E�����x�̏�����
        unlockPlayModeRtf[0].localScale = Vector3.zero;
        unlockPlayModeRtf[1].localScale = Vector3.zero;
        unlockPlayMessage.color = Color.clear;

        //SE�̍Đ�
        AudioManager.Instance.PlaySE(AudioManager.Instance.SE_SkinUnlock);

        //�v���C�{�^�����A�����b�N
        InputManager.Instance.SetPlayButtonLock(false);
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��Ԃ̉��Z
        elapsedTime += deltaTime;

        //�^�b�v�̌��o
        IM.GetInput_Screen();

        //�J�n��2�b�ȓ��Ȃ�
        if (elapsedTime < 2)
        {
            //��ʂ��^�b�v�����Ή��o���΂�
            if (IM.is_Screen_Tap) elapsedTime = 2;

            //�X�P�[���ύX
            unlockPlayModeRtf[0].localScale = Vector3.one * Mathf.Lerp(0, 1, elapsedTime * 2);
            unlockPlayModeRtf[1].localScale = Vector3.one * Mathf.Lerp(0, 1, elapsedTime * 2);

            //�e�L�X�g�����x�ύX
            unlockPlayMessage.color = Color.Lerp(Color.clear, Color.black, elapsedTime - 1);
        }

        //2�b�o�߈ȍ~��
        else
        {
            //�e�L�X�g�̖���
            unlockPlayMessage.color = Color.Lerp(Color.clear, Color.black, (Mathf.Cos(elapsedTime * Mathf.PI) + 1) / 2f);

            //��ʂ��^�b�v�����΃X�e�[�g�J��
            if (IM.is_Screen_Tap)
            {
                if (GM.newSkinQueue.Count > 0) stateMachine.ChangeState(stateMachine.state_UnlockSkin);
                else stateMachine.ChangeState(stateMachine.state_Result);
            }
        }
    }



    public override void Exit()
    {
        //�v���C���[�h�A�����b�N��ʂ̖�����
        GM.unlockPlay.SetActive(false);

        //��ʃ^�b�v���o�{�^���̖�����
        IM.InputUISetActive_Screen(false);
    }
}
