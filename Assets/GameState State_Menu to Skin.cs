using UnityEngine;

public class GameStateState_MenuToSkin : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform skinHingeRtf_U;
    public RectTransform skinHingeRtf_B;

    public GameStateState_MenuToSkin(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        skinHingeRtf_U = GM.skinHingeRtf_U;
        skinHingeRtf_B = GM.skinHingeRtf_B;
    }



    public override void Enter()
    {
        //�o�ߎ��ԃ��Z�b�g
        elapsedTime = 0;

        //�X�L���Z���N�^�[�X�N���v�g�̗L����
        GameManager.Instance.skinSelecter.enabled = true;
    }



    public override void Update(float deltaTime)
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += deltaTime;

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //0.5�b�ҋ@
        if (elapsedTime < 0.5) return;

        //���O�v�Z
        float lerpValue = (elapsedTime - 0.5f) / 1.5f;

        //UI����]
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        skinHingeRtf_U.localEulerAngles = Vector3.Lerp(Vector3.right * -180, Vector3.zero, lerpValue);
        skinHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 180, Vector3.zero, Mathf.Pow(lerpValue, 0.3f));

        //�w�莞�Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Skin);
    }



    public override void Exit()
    {

    }
}
