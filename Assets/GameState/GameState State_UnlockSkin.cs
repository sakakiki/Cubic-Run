using TMPro;
using UnityEngine;

public class GameStateState_UnlockSkin : GameStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer unlockSkinModelSprite;
    private SpriteMask unlockSkinMask;
    private RectTransform unlockSkinModelRtf;
    private TextMeshProUGUI unlockSkinName;
    private RectTransform unlockSkinNameRtf;
    private TextMeshProUGUI unlockSkinMessage;

    public GameStateState_UnlockSkin(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        unlockSkinModelSprite = GM.unlockSkinModelSprite;
        unlockSkinMask = GM.unlockSkinMask;
        unlockSkinModelRtf = GM.unlockSkinModelRtf;
        unlockSkinName = GM.unlockSkinName;
        unlockSkinNameRtf = GM.unlockSkinNameRtf;
        unlockSkinMessage = GM.unlockSkinMessage;
    }


    public override void Enter()
    {
        //�X�L���A�����b�N��ʂ̗L����
        GM.unlockSkin.SetActive(true);

        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //��ʃ^�b�v���o�{�^���̗L����
        IM.InputUISetActive_Screen(true);

        //�\���̐؂�ւ�
        int skinID = GM.newSkinQueue.Dequeue();
        unlockSkinModelSprite.color = SkinDataBase.Instance.skinData[skinID].skinColor;
        unlockSkinModelSprite.sprite =
            SkinDataBase.Instance.skinData[skinID].bodyType == SkinData.BodyType.Cube ? GM.squarSprite : GM.cicleSprite;
        unlockSkinMask.enabled = SkinDataBase.Instance.skinData[skinID].isEnabledMask;
        unlockSkinName.SetText(SkinDataBase.Instance.skinData[skinID].name);

        //�X�P�[���E�����x�̏�����
        unlockSkinModelRtf.localScale = Vector3.zero;
        unlockSkinNameRtf.localScale = Vector3.zero;
        unlockSkinMessage.color = Color.clear;
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
            unlockSkinModelRtf.localScale = Vector3.one * Mathf.Lerp(0, 600, elapsedTime * 2);
            unlockSkinNameRtf.localScale = Vector3.one * Mathf.Lerp(0, 1, elapsedTime * 2);

            //�e�L�X�g�����x�ύX
            unlockSkinMessage.color = Color.Lerp(Color.clear, Color.black, elapsedTime - 1);
        }

        //2�b�o�߈ȍ~��
        else
        {
            //�e�L�X�g�̖���
            unlockSkinMessage.color = Color.Lerp(Color.clear, Color.black, (Mathf.Cos(elapsedTime * Mathf.PI) + 1) / 2f);

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
        //�X�L���A�����b�N��ʂ̖�����
        GM.unlockSkin.SetActive(false);

        //��ʃ^�b�v���o�{�^���̖�����
        IM.InputUISetActive_Screen(false);
    }
}
