using UnityEngine;

public class PlayStateState_TrainingEnd: PlayStateStateBase
{
    public PlayStateState_TrainingEnd(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //障害物生成を無効化
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //基準スコアに到達すれば
        if (GM.score >= 5000)
        {
            //スコアを補正
            GM.score = 5000;
            GM.scoreText.SetText("5000");

            //スコアゲージ補正
            GM.scoreGageTf.localScale = Vector3.one * 2;

            //クリア回数を加算
            GM.AddClearTimesNum(GM.level);

            //トレーニングモードの最高レベルをクリアしたなら
            if (GM.trainingLevel == GM.highestLevel)
            {
                //トレーニングモードのレベルを追加
                GM.AddTrainingLevel();

                //リトライボタンのテキストを変更
                GM.retryButtonText.SetText("Lv." +  GM.trainingLevel + "へ");
            }
            //クリア済みのレベルなら
            else
            {
                //リトライボタンのテキストを変更
                GM.retryButtonText.SetText("リトライ");
            } 

            //ステート遷移
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        }
    }



    public override void Exit()
    {

    }
}
