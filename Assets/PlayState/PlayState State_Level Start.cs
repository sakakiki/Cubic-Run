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

        //BGMの切り替え
        if (GM.level <= 5)
        {
            if (AudioManager.Instance.playngBGM != AudioManager.BGM.Play_1)
            {
                AudioManager.Instance.PlayBGM(AudioManager.BGM.Play_1);
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(0.9f, 1.1f, (GM.level - 1) / 4f));
        }
        else if (6 <= GM.level && GM.level <= 10)
        {
            if (AudioManager.Instance.playngBGM != AudioManager.BGM.Play_2)
            {
                AudioManager.Instance.PlayBGM(AudioManager.BGM.Play_2);
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(1.05f, 1.25f, (GM.level - 6) / 4f));
        }
        else if (11 <= GM.level && GM.level <= 15)
        {
            if (AudioManager.Instance.playngBGM != AudioManager.BGM.Play_3)
            {
                AudioManager.Instance.PlayBGM(AudioManager.BGM.Play_3);
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(1.1f, 1.25f, (GM.level - 11) / 4f));
        }
        else if (16 <= GM.level && GM.level <= 20)
        {
            if (AudioManager.Instance.playngBGM != AudioManager.BGM.Play_4)
            {
                AudioManager.Instance.PlayBGM(AudioManager.BGM.Play_4);
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(1.1f, 1.2f, (GM.level - 16) / 4f));
        }
        else if (21 <= GM.level)
        {
            if (AudioManager.Instance.playngBGM != AudioManager.BGM.Play_5)
            {
                AudioManager.Instance.PlayBGM(AudioManager.BGM.Play_5);
                AudioManager.Instance.SetBGMSpeed(1);
            }
        }

        //トレーニングモードなら障害物生成終了を遅らせる
        if (GM.isTraining)
            ((PlayStateState_Play)stateMachine.state_Play).levelEndScore += 250;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //レベルテキストの位置・大きさ・色調整
        int shortageScore = levelStartScore - scoreCorrection - GM.score;
        levelTf.position = Vector3.Lerp(levelMarker.position, centerPos, shortageScore / 75f);
        levelText.fontSize = Mathf.Lerp(72, 300, shortageScore / 75f);
        levelText.color = Color.Lerp(Color.black, Color.clear, (shortageScore - 200) / 50f);

        //スコアが基準を満たせばステート遷移
        if (GM.score > levelStartScore - scoreCorrection)
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
