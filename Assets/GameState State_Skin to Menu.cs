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
    private bool isReset;

    public GameStateState_SkinToMenu(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        skinHingeRtf_U = GM.skinHingeRtf_U;
        skinHingeRtf_B = GM.skinHingeRtf_B;
        skinSelecter = GM.skinSelecter;
    }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //フラグリセット
        isMoving = false;
        isReset = IM.isSkinSelect;
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
        if (!isReset)
        {
            //リセット済みフラグを立てる
            isReset = true;

            //スキンの色を戻す
            GM.ChangePlayerSkin(GM.previousSkinID);
        }

        //指定時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Menu);
    }



    public override void Exit()
    {
        //必要に応じてスキンセレクターを回転
        if (!IM.isSkinSelect)
            skinSelecter.SetWheelAngle(GM.previousSkinID);

        //スキンセレクタースクリプトの無効化
        skinSelecter.enabled = false;
    }
}
