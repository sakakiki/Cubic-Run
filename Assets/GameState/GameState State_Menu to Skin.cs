using UnityEngine;

public class GameStateState_MenuToSkin : GameStateStateBase
{
    private float elapsedTime;
    public RectTransform menuHingeRtf_L;
    public RectTransform menuHingeRtf_R;
    public RectTransform skinHingeRtf_U;
    public RectTransform skinHingeRtf_B;

    public GameStateState_MenuToSkin(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        menuHingeRtf_L = GM.menuHingeRtf_L;
        menuHingeRtf_R = GM.menuHingeRtf_R;
        skinHingeRtf_U = GM.skinHingeRtf_U;
        skinHingeRtf_B = GM.skinHingeRtf_B;
    }



    public override void Enter()
    {
        //経過時間リセット
        elapsedTime = 0;

        //スキンセレクタースクリプトの有効化
        GameManager.Instance.skinSelecter.enabled = true;

        //条件を満たしているスキンのロック解除
        GM.CheckSkinUnlock();
    }



    public override void Update(float deltaTime)
    {
        //経過時間加算
        elapsedTime += deltaTime;

        //地形を管理
        TM.ManageMovingTerrain();

        //0.5秒待機
        if (elapsedTime < 0.5) return;

        //事前計算
        float lerpValue = (elapsedTime - 0.5f) / 1.5f;

        //UIを回転
        menuHingeRtf_L.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * -180, lerpValue);
        menuHingeRtf_R.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * 180, lerpValue);
        skinHingeRtf_U.localEulerAngles = Vector3.Lerp(Vector3.right * -180, Vector3.zero, lerpValue);
        skinHingeRtf_B.localEulerAngles = Vector3.Lerp(Vector3.right * 180, Vector3.zero, Mathf.Pow(lerpValue, 0.3f));

        //指定時間経過でステート遷移
        if (elapsedTime > 2)
            stateMachine.ChangeState(stateMachine.state_Skin);
    }



    public override void Exit()
    {

    }
}
