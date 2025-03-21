using UnityEngine;

public class TutorialStateState_Fall_2 : TutorialStateStateBase
{
    private const int actionNum_Jump = 0;
    private SpriteRenderer buttonSprite_Jump;



    public TutorialStateState_Fall_2(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�Q�[���̈ꎞ��~
        Time.timeScale = 0;

        //�W�����v�A�N�V�����̂ݗL����
        PlayerStateBase_Play.InitializeTutorial();
        PlayerStateBase_Play.isActive_Jump = true;

        //�W�����v�{�^�������点�A����ȊO���Â�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            if (IM.actionAllocation[i] == actionNum_Jump)
            {
                buttonSprite_Jump = IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i];
                buttonSprite_Jump.color = Color.white - Color.black * 0.9f;
            }
            else
                IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.black * 0.8f;

        //������@�\��
        PopupUIManager.Instance.SetupMessageText("�󒆂ł��Ⴊ�ނƋ}�~��");
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���Ԓ�~���̂��ߋ^�����ԉ��Z
        elapsedTime += 1f / GM.defaultFrameRate;

        //�^�b�v����{�^�������点��
        buttonSprite_Jump.color = Color.white - Color.black * (1 - Mathf.Abs(elapsedTime % 2 - 1) * 0.1f);

        //�w��̓��͂𖞂����΃X�e�[�g�J��
        if (IM.is_Player_Jump_Push)
            stateMachine.ChangeState(stateMachine.state_Fall_3);
    }



    public override void Exit()
    {
        //�{�^����߂�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.clear;

        //�Q�[���̍ĊJ
        Time.timeScale = 1;
    }
}
