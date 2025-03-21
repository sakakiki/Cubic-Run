using UnityEngine;

public class TutorialStateState_SmallJump_2 : TutorialStateStateBase
{
    private const int actionNum_Squat = 1;
    private SpriteRenderer buttonSprite_Squat;



    public TutorialStateState_SmallJump_2(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�Q�[���̈ꎞ��~
        Time.timeScale = 0;

        //�W�����v�A�N�V�����̖�����
        PlayerStateBase_Play.isActive_Jump = false;
        //���Ⴊ�݃A�N�V�����̗L����
        PlayerStateBase_Play.isActive_Squat = true;
        //�U���A�N�V�����̖�����
        PlayerStateBase_Play.isActive_Attack = false;

        //���Ⴊ�݃{�^�������点�A����ȊO���Â�
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            if (IM.actionAllocation[i] == actionNum_Squat)
            {
                buttonSprite_Squat = IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i];
                buttonSprite_Squat.color = Color.white - Color.black * 0.9f;
            }
            else
                IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.black * 0.8f;

        //������@�\��
        PopupUIManager.Instance.SetupMessageText("���Ⴊ�ݒ��ɃW�����v����Ə��W�����v");
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
            stateMachine.ChangeState(stateMachine.state_SmallJump_3);
    }



    public override void Exit() { }
}
