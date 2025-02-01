using UnityEngine;

public class GameStateState_Skin : GameStateStateBase
{
    private SkinSelecter skinSelecter;

    public GameStateState_Skin(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        skinSelecter = GM.skinSelecter;
    }



    public override void Enter()
    {
        //�X�L���I����ʂ�UI��L����
        IM.InputUISetActive_Skin(true);

        //�X�L���Z���N�^�[�̗L����
        GameManager.Instance.skinSelecter.isActive = true;
    }



    public override void Update(float deltaTime)
    {
        //���͂̎擾
        IM.GetInput_Skin();

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //���͂�����Z���N�^�[�̃z�C�[������~���Ă���΃X�e�[�g�J��
        if (IM.is_Skin_OK_Push || IM.is_Skin_Cancel_Push)
            if (skinSelecter.isStop)
                stateMachine.ChangeState(stateMachine.state_SkinToMenu);
    }



    public override void Exit()
    {
        //�X�L���I����ʂ�UI�𖳌���
        IM.InputUISetActive_Skin(false);

        //�X�L���Z���N�^�[�X�N���v�g���ꎞ�I�ɖ������iSkinToMenu�X�e�[�g�ɂčėL�����j
        skinSelecter.enabled = false;
    }
}
