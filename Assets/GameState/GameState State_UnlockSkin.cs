using TMPro;
using UnityEngine;

public class GameStateState_UnlockSkin : GameStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer unlockSkinModelSprite;
    private SpriteMask unlockSkinMask;
    private RectTransform unlockSkinModelRtf;
    private TextMeshProUGUI unlockSkinName;
    private RectTransform unlockSkinNameRtf;
    private TextMeshProUGUI unlockSkinMessage;

    public GameStateState_UnlockSkin(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        unlockSkinModelSprite = GM.unlockSkinModelSprite;
        unlockSkinMask = GM.unlockSkinMask;
        unlockSkinModelRtf = GM.unlockSkinModelRtf;
        unlockSkinName = GM.unlockSkinName;
        unlockSkinNameRtf = GM.unlockSkinNameRtf;
        unlockSkinMessage = GM.unlockSkinMessage;
    }


    public override void Enter()
    {
        //スキンアンロック画面の有効化
        GM.unlockSkin.SetActive(true);

        //経過時間リセット
        elapsedTime = 0;

        //画面タップ検出ボタンの有効化
        IM.InputUISetActive_Screen(true);

        //表示の切り替え
        int skinID = GM.newSkinQueue.Dequeue();
        unlockSkinModelSprite.color = SkinDataBase.Instance.skinData[skinID].skinColor;
        unlockSkinModelSprite.sprite =
            SkinDataBase.Instance.skinData[skinID].bodyType == SkinData.BodyType.Cube ? GM.squarSprite : GM.cicleSprite;
        unlockSkinMask.enabled = SkinDataBase.Instance.skinData[skinID].isEnabledMask;
        unlockSkinName.SetText(SkinDataBase.Instance.skinData[skinID].name);

        //スケール・透明度の初期化
        unlockSkinModelRtf.localScale = Vector3.zero;
        unlockSkinNameRtf.localScale = Vector3.zero;
        unlockSkinMessage.color = Color.clear;

        //SEの再生
        AudioManager.Instance.PlaySE(AudioManager.SE.SkinUnlock);
    }



    public override void Update(float deltaTime)
    {
        //経過時間の加算
        elapsedTime += deltaTime;

        //タップの検出
        IM.GetInput_Screen();

        //開始後2秒以内なら
        if (elapsedTime < 2)
        {
            //画面がタップされれば演出を飛ばす
            if (IM.is_Screen_Tap) elapsedTime = 2;

            //スケール変更
            unlockSkinModelRtf.localScale = Vector3.one * Mathf.Lerp(0, 600, elapsedTime * 2);
            unlockSkinNameRtf.localScale = Vector3.one * Mathf.Lerp(0, 1, elapsedTime * 2);

            //テキスト透明度変更
            unlockSkinMessage.color = Color.Lerp(Color.clear, Color.black, elapsedTime - 1);
        }

        //2秒経過以降は
        else
        {
            //テキストの明滅
            unlockSkinMessage.color = Color.Lerp(Color.clear, Color.black, (Mathf.Cos(elapsedTime * Mathf.PI) + 1) / 2f);

            //画面がタップされればステート遷移
            if (IM.is_Screen_Tap)
            {
                if (GM.newSkinQueue.Count > 0) stateMachine.ChangeState(stateMachine.state_UnlockSkin);
                else stateMachine.ChangeState(stateMachine.state_Result);
            }
        }
    }



    public override void Exit()
    {
        //スキンアンロック画面の無効化
        GM.unlockSkin.SetActive(false);

        //画面タップ検出ボタンの無効化
        IM.InputUISetActive_Screen(false);
    }
}
