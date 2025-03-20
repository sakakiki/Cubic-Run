using UnityEngine;
using TMPro;

public class TutorialStateState_Start : TutorialStateStateBase
{
    public TutorialStateState_Start(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //�|�b�v�A�b�v���b�Z�[�W
        PopupUIManager.Instance.SetupMessageBand("�`���[�g���A��", 1);
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Jump_1);
    }



    public override void Exit() { }
}
