using UnityEngine;

public class TutorialStateState_Squat_2 : TutorialStateStateBase
{
    private const int actionNum_Squat = 1;
    private SpriteRenderer buttonSprite_Squat;



    public TutorialStateState_Squat_2(TutorialStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        base.Enter();

        //ゲームの一時停止
        Time.timeScale = 0;

        //ジャンプアクションの有効化
        PlayerStateBase_Play.isActive_Jump = true;
        //しゃがみアクションの有効化
        PlayerStateBase_Play.isActive_Squat = true;

        //ジャンプボタンを白っぽく、それ以外を暗く
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            if (IM.actionAllocation[i] == actionNum_Squat)
            {
                buttonSprite_Squat = IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i];
                buttonSprite_Squat.color = Color.white - Color.black * 0.9f;
            }
            else
                IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.black * 0.8f;

        //操作方法表示
        PopupUIManager.Instance.SetupMessageText("光っている部分をタップしてしゃがむ");
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
            stateMachine.ChangeState(stateMachine.state_Squat_3);
    }



    public override void Exit()
    {
        //ボタンを戻す
        for (int i = 0; i < IM.actionAllocation.Length; i++)
            IM.playButtonSet[IM.playButtonPatternNum].playButtonSprite[i].color = Color.clear;

        //操作方法非表示
        PopupUIManager.Instance.DeleteMessageText();

        //ゲームの再開
        Time.timeScale = 1;
    }
}
