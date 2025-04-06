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

        //ポップアップメッセージ
        PUIM.SetMessageTextColor(Color.clear);
        PUIM.SetupMessageText("チュートリアル");

        //再挑戦ステートの更新
        continueState = stateMachine.state_Jump_1;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //ポップアップメッセージ
        PUIM.SetMessageTextColor(Color.black * (1 - Mathf.Abs(elapsedTime - 1)) * 2);

        //時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Jump_1);
    }



    public override void Exit()
    {
        PUIM.DeleteMessageText();
        PUIM.SetMessageTextColor(Color.black);
    }
}
