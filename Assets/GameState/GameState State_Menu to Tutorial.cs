using UnityEngine;

public class GameStateState_MenuToTutorial : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform playHingeRtf_R;
    private SpriteRenderer screenCover;
    private Color startCoverColor;
    private Color targetCoverColor;

    public GameStateState_MenuToTutorial(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        playHingeRtf_R = GM.playHingeRtf_R;
        screenCover = GM.screenCover;
        startCoverColor = GM.screenCoverColor_Menu;
        targetCoverColor = GM.screenCoverColor_Play;
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

        //事前計算
        float lerpValue = (elapsedTime - 0.5f) / 1.5f;

        //地形を管理
        TM.ManageMovingTerrain();

        //UIを回転
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        playHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);

        //スクリーンカバーの色を変更
        screenCover.color = 
            startCoverColor - Color.black * Mathf.Lerp(targetCoverColor.a, startCoverColor.a, lerpValue);

        //BGMのボリューム変更
        AM.SetBGMPlayVolume(2 - elapsedTime);

        //指定時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Tutorial);
    }



    public override void Exit() { }
}
