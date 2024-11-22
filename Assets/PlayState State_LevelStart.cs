using UnityEngine;
using TMPro;

public class PlayStateState_LevelStart : PlayStateStateBase
{
    private PlayerStateMachine playerStateMachine;
    private int levelStartScore;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI levelText;



    public PlayStateState_LevelStart(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        playerStateMachine = playerCon.stateMachine;
        scoreText = GameManager.Instance.scoreText;
        levelText = GameManager.Instance.levelText;
    }



    public override void Enter()
    {
        //障害物生成を開始するスコアを算出
        levelStartScore = GM.level * GM.levelUpSpan + 300;

        //レベル上昇・更新
        GM.level++;
        levelText.SetText("Lv." + GM.level);

        //速度更新
        TM.moveSpeed = 5 + Mathf.Pow(GM.level, 0.7f) * 3;

        //障害物生成を終了するスコアを算出
        ((PlayStateState_Play)stateMachine.state_Play).levelEndScore = 
            GM.level * GM.levelUpSpan - (int)(300 + 2400 / TM.moveSpeed);

        //プレイヤーがGameOverステートならGameOverステートに遷移
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Update(float deltaTime)
    {
        //プレイ時間加算
        playTime += deltaTime;

        //移動中の地形を管理
        TM.ManageMovingTerrain();

        //スコア更新
        GM.score = (int)(playTime * 100);
        scoreText.SetText("" + GM.score);

        //スコアが基準を満たせばステート遷移
        if (GM.score > levelStartScore)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {

    }
}
