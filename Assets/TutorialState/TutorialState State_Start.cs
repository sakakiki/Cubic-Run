using UnityEngine;
using TMPro;

public class TutorialStateState_Start : TutorialStateStateBase
{
    public TutorialStateState_Start(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //ポップアップメッセージ
        PopupUIManager.Instance.SetupMessageBand("チュートリアル", 1);
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Jump_1);
    }



    public override void Exit() { }
}
