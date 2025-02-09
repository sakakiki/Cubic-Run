using UnityEngine;

public class GameStateState_SkinToMenu : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform skinHingeRtf_U;
    public RectTransform skinHingeRtf_B;
    private SkinSelecter skinSelecter;
    private bool isMoving;
    private bool isReset;

    public GameStateState_SkinToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        skinHingeRtf_U = GM.skinHingeRtf_U;
        skinHingeRtf_B = GM.skinHingeRtf_B;
        skinSelecter = GM.skinSelecter;
    }



    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //�t���O���Z�b�g
        isMoving = false;
        isReset = IM.isSkinSelect;
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //0.5�b�ҋ@
        if (elapsedTime < 0.5) return;

        //�X�L���Z���N�^�[�X�N���v�g�̍ėL�����iSkin�X�e�[�g�ɂĖ������j
        if (!isMoving)
        {
            isMoving = true;
            skinSelecter.enabled = true;
        }

        //���O�v�Z
        float lerpValue = (elapsedTime - 0.5f) / 1.5f;

        //UI����]
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        skinHingeRtf_U.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * -180, lerpValue);
        skinHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, Mathf.Pow(lerpValue, 1.7f));

        //1.5�b�o�߂܂Ŏc��̏������΂�
        if (elapsedTime < 1.5) return;

        //���Z�b�g�̕K�v������Ȃ�
        if (!isReset)
        {
            //���Z�b�g�ς݃t���O�𗧂Ă�
            isReset = true;

            //�X�L���̐F��߂�
            GM.ChangePlayerSkin(GM.previousSkinID);
        }

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit()
    {
        //�K�v�ɉ����ăX�L���Z���N�^�[����]
        if (!IM.isSkinSelect)
            skinSelecter.SetWheelAngle(GM.previousSkinID);

        //�X�L���Z���N�^�[�X�N���v�g�̖�����
        skinSelecter.enabled = false;
    }
}
