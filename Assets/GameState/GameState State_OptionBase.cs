using UnityEngine;

public abstract class GameStateState_OptionBase : GameStateStateBase
{
    public GameStateState_OptionBase(GameStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //�I�v�V������ʂ̓��͂�L����
        IM.InputUISetActive_Option(true);

        //�I�v�V������ʂ�UI��L����
        GameManager.Instance.optionUIBase.SetActive(true);

        //���������ɃJ�o�[
        GameManager.Instance.frontScreenCover.color = Color.white - Color.black * 0.2f;
    }



    public override void Update(float deltaTime)
    {
        //���͂̎擾
        IM.GetInput_Option();

        //�n�`���Ǘ�
        TM.ManageMovingTerrain();

        //���͂ɉ������X�e�[�g�J��
        if (IM.is_Option_Close_Tap)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit()
    {
        //�I�v�V������ʂ̓��͂𖳌���
        IM.InputUISetActive_Option(false);

        //�I�v�V������ʂ�UI�𖳌���
        GameManager.Instance.optionUIBase.SetActive(false);

        //�J�o�[�𓧖���
        GameManager.Instance.frontScreenCover.color = Color.clear;

        //SE�̍Đ�
        AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Close);
    }
}
