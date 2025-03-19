using UnityEngine;

public class GameStateState_SkinToMenu : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform skinHingeRtf_U;
    public RectTransform skinHingeRtf_B;
    private SkinSelecter skinSelecter;
    private bool isMoving;
    private bool isNeedReset;

    public GameStateState_SkinToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        skinHingeRtf_U = GM.skinHingeRtf_U;
        skinHingeRtf_B = GM.skinHingeRtf_B;
        skinSelecter = GM.skinSelecter;
    }



    public async override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //フラグリセット
        isMoving = false;
        isNeedReset = !IM.isSkinSelect;

        //リセットの必要が無ければ
        if (!isNeedReset)
        {
            //使用中のスキン情報をクラウドに保存
            await FirestoreManager.Instance.SaveUsingSkin();

            //ランキングを更新
            await RankingManager.UpdateRanking(RankingManager.RankingType.HighScore);
            await RankingManager.UpdateRanking(RankingManager.RankingType.PlayerScore);
            GM.highScoreRankingBoard.UpdateRanking();
            GM.playerScoreRankingBoard.UpdateRanking();
        }
        //リセットが必要ならUIの色だけ先に戻す
        else GM.ChangeUIColor(GM.previousSkinID);
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //地形を管理
        TM.ManageMovingTerrain();

        //0.5秒待機
        if (elapsedTime < 0.5) return;

        //スキンセレクタースクリプトの再有効化（Skinステートにて無効化）
        if (!isMoving)
        {
            isMoving = true;
            skinSelecter.enabled = true;
        }

        //事前計算
        float lerpValue = (elapsedTime - 0.5f) / 1.5f;

        //UIを回転
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.up * -180, Vector3.zero, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.up * 180, Vector3.zero, lerpValue);
        skinHingeRtf_U.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * -180, lerpValue);
        skinHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.right * 180, Mathf.Pow(lerpValue, 1.7f));

        //1.5秒経過まで残りの処理を飛ばす
        if (elapsedTime < 1.5) return;

        //リセットの必要があるなら
        if (isNeedReset)
        {
            //リセット要求フラグを下ろす
            isNeedReset = false;

            //スキンの色を戻す
            GM.ChangePlayerSkin(GM.previousSkinID);

            //ランキングを更新
            GM.highScoreRankingBoard.UpdateRanking();
            GM.playerScoreRankingBoard.UpdateRanking();
        }

        //指定時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override async void Exit()
    {
        //必要に応じてスキンセレクターを回転
        if (!IM.isSkinSelect)
            skinSelecter.SetWheelAngle(GM.previousSkinID);

        //スキンセレクタースクリプトの無効化
        skinSelecter.enabled = false;
    }
}
