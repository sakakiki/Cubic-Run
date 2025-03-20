using UnityEngine;

public class TutorialStateState_Attack_2 : TutorialStateStateBase
{
    private const int actionNum_Attack = 2;
    private SpriteRenderer buttonSprite_Attack;



    public TutorialStateState_Attack_2(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�Q�[���̈ꎞ��~
        Time.timeScale = 0;

        //�W�����v�A�N�V�����̗L����
        PlayerStateBase_Play.isActive_Jump = true;
        //�U���A�N�V�����̗L����
        PlayerStateBase_Play.isActive_Attack = true;

        //�W�����v�{�^���𔒂��ۂ��A����ȊO���Â�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            if (IM.actionAllocation[i] == actionNum_Attack)
            {
                buttonSprite_Attack = IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i];
                buttonSprite_Attack.color = Color.white - Color.black * 0.9f;
            }
            else
                IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.black * 0.8f;

        //������@�\��
        PopupUIManager.Instance.SetupMessageText("�����Ă��镔�����^�b�v���čU����Ԃ�");
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���Ԓ�~���̂��ߋ^�����ԉ��Z
        elapsedTime += 1f / GM.defaultFrameRate;

        //�^�b�v����{�^�������点��
        buttonSprite_Attack.color = Color.white - Color.black * (1 - Mathf.Abs(elapsedTime % 2 - 1) * 0.1f);

        //�w��̓��͂𖞂����΃X�e�[�g�J��
        if (IM.is_Player_Attack_Push)
            stateMachine.ChangeState(stateMachine.state_Attack_3);
    }



    public override void Exit()
    {
        //�{�^����߂�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.clear;

        //������@��\��
        PopupUIManager.Instance.DeleteMessageText();

        //�Q�[���̍ĊJ
        Time.timeScale = 1;
    }
}
