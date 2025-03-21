using UnityEngine;

public class TutorialStateState_Fall_4 : TutorialStateStateBase
{
    private const int actionNum_Squat = 1;
    private SpriteRenderer buttonSprite_Squat;



    public TutorialStateState_Fall_4(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�Q�[���̈ꎞ��~
        Time.timeScale = 0;

        //�S�A�N�V�����̗L����
        PlayerStateBase_Play.EnableAllAction();

        //���Ⴊ�݃{�^�������点�A����ȊO���Â�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            if (IM.actionAllocation[i] == actionNum_Squat)
            {
                buttonSprite_Squat = IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i];
                buttonSprite_Squat.color = Color.white - Color.black * 0.9f;
            }
            else
                IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.black * 0.8f;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���Ԓ�~���̂��ߋ^�����ԉ��Z
        elapsedTime += 1f / GM.defaultFrameRate;

        //�^�b�v����{�^�������点��
        buttonSprite_Squat.color = Color.white - Color.black * (1 - Mathf.Abs(elapsedTime % 2 - 1) * 0.1f);

        //�w��̓��͂𖞂����΃X�e�[�g�J��
        if (IM.is_Player_Squat_Push)
            stateMachine.ChangeState(stateMachine.state_Fall_5);
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
