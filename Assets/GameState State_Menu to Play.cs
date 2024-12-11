using UnityEngine;

public class GameStateState_MenuToPlay : GameStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer screenCover;
    private Color startColor;
    private Color targetColor;

    public GameStateState_MenuToPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        screenCover = GM.screenCover;
        startColor = GM.screenCoverColor_Menu;
        targetColor = GM.screenCoverColor_Play;
    }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //障害物の生成を無効化
        TM.isCreateObstacle = false;

        //背景の地形を加速
        TM.moveSpeed = 20;
        TM.SetSpeed(20);
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //地形を管理
        TM.ManageMovingTerrain();

        //スクリーンカバーの色を変更
        screenCover.color = Color.Lerp(startColor, targetColor, elapsedTime - 1);

        //指定時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //地形を減速
        TM.moveSpeed = 8;
        TM.SetSpeed(8);
    }
}
