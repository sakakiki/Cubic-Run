using UnityEngine;
using TMPro;

public class TutorialStateState_Start : TutorialStateStateBase
{
    private PopupUIManager PUIM;



    public TutorialStateState_Start(TutorialStateStateMachine stateMachine) : base(stateMachine)
    {
        PUIM = PopupUIManager.Instance;
    }



    public override void Enter()
    {
        base.Enter();

        //�|�b�v�A�b�v���b�Z�[�W
        PUIM.SetMessageTextColor(Color.clear);
        PUIM.SetupMessageText("�`���[�g���A��");

        //�Ē���X�e�[�g�̍X�V
        continueState = stateMachine.state_Jump_1;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //�|�b�v�A�b�v���b�Z�[�W
        PUIM.SetMessageTextColor(Color.black * (1 - Mathf.Abs(elapsedTime - 1)) * 2);

        //���Ԍo�߂ŃX�e�[�g�J��
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Jump_1);
    }



    public override void Exit()
    {
        PUIM.DeleteMessageText();
        PUIM.SetMessageTextColor(Color.black);
    }
}
