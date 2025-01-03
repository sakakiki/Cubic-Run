using UnityEngine;
using TMPro;

public class PlayStateState_LevelStart : PlayStateStateBase
{
    private int levelStartScore;
    private TextMeshProUGUI levelText;
    private Transform levelTf;
    private Vector3 centerPos;
    private RectTransform levelMarker;



    public PlayStateState_LevelStart(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        levelText = GameManager.Instance.levelText;
        levelTf = GM.levelTf;
        centerPos = GM.centerPos_World;
        levelMarker = GM.levelMarkerRtf_Play;
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
            GM.level * GM.levelUpSpan - (int)(300 + 3200 / TM.moveSpeed);
        /*
            値の詳細
            GM.level * GM.levelUpSpan　……レベル上昇のタイミング（スコア換算）
            300　……終了演出に使用する時間（スコア換算）
            3200 / TM.moveSpeed　……障害物が画面外に移動するのにかかる時間（スコア換算）
        */
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //レベルテキストの位置・大きさ・色調整
        int shortageScore = levelStartScore - GM.score;
        levelTf.position = Vector3.Lerp(levelMarker.position, centerPos, shortageScore / 75f);
        levelText.fontSize = Mathf.Lerp(72, 300, shortageScore / 75f);
        levelText.color = Color.Lerp(Color.black, Color.clear, (shortageScore - 200) / 50f);

        //スコアが基準を満たせばステート遷移
        if (GM.score > levelStartScore)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //レベルのテキストのZ座標を修正
        Vector3 levelPos = levelTf.position;
        levelPos.z = GM.scoreSetTf.position.z;
        levelTf.position = levelPos;
    }
}
