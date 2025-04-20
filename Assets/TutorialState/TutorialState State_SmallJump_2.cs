using UnityEngine;

public class TutorialStateState_SmallJump_2 : TutorialStateStateBase
{
    private const int actionNum_Squat = 1;
    private SpriteRenderer buttonSprite_Squat;



    public TutorialStateState_SmallJump_2(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //ゲームの一時停止
        Time.timeScale = 0;

        //ジャンプアクションの無効化
        PlayerStateBase_Play.isActive_Jump = false;
        //しゃがみアクションの有効化
        PlayerStateBase_Play.isActive_Squat = true;
        //攻撃アクションの無効化
        PlayerStateBase_Play.isActive_Attack = false;

        //しゃがみボタンを光らせ、それ以外を暗く
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            if (IM.actionAllocation[i] == actionNum_Squat)
            {
                buttonSprite_Squat = IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i];
                buttonSprite_Squat.color = Color.white - Color.black * 0.9f;
            }
            else
                IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.black * 0.8f;

        //操作方法表示
        PopupUIManager.Instance.SetupMessageText("しゃがみ中にジャンプすると小ジャンプ");
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //時間停止中のため疑似時間加算
        elapsedTime += 1f / GM.defaultFrameRate;

        //タップするボタンを光らせる
        buttonSprite_Squat.color = Color.white - Color.black * (1 - Mathf.Abs(elapsedTime % 2 - 1) * 0.1f);

        //指定の入力を満たせばステート遷移
        if (IM.is_Player_Squat_Push)
            stateMachine.ChangeState(stateMachine.state_SmallJump_3);
    }



    public override void Exit() { }
}
