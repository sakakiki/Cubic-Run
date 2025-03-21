using TMPro;
using UnityEngine;

public class GameStateState_UnlockPlay : GameStateStateBase
{
    private float elapsedTime;
    private RectTransform[] unlockPlayModeRtf;
    private TextMeshProUGUI unlockPlayMessage;

    public GameStateState_UnlockPlay(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        unlockPlayModeRtf = GM.unlockPlayModeRtf;
        unlockPlayMessage = GM.unlockPlayMessage;
    }


    public override void Enter()
    {
        //プレイモードアンロック画面の有効化
        GM.unlockPlay.SetActive(true);

        //経過時間リセット
        elapsedTime = 0;

        //画面タップ検出ボタンの有効化
        IM.InputUISetActive_Screen(true);

        //スケール・透明度の初期化
        unlockPlayModeRtf[0].localScale = Vector3.zero;
        unlockPlayModeRtf[1].localScale = Vector3.zero;
        unlockPlayMessage.color = Color.clear;

        //SEの再生
        AudioManager.Instance.PlaySE(AudioManager.Instance.SE_SkinUnlock);

        //プレイボタンをアンロック
        InputManager.Instance.SetPlayButtonLock(false);
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
            unlockPlayModeRtf[0].localScale = Vector3.one * Mathf.Lerp(0, 1, elapsedTime * 2);
            unlockPlayModeRtf[1].localScale = Vector3.one * Mathf.Lerp(0, 1, elapsedTime * 2);

            //テキスト透明度変更
            unlockPlayMessage.color = Color.Lerp(Color.clear, Color.black, elapsedTime - 1);
        }

        //2秒経過以降は
        else
        {
            //テキストの明滅
            unlockPlayMessage.color = Color.Lerp(Color.clear, Color.black, (Mathf.Cos(elapsedTime * Mathf.PI) + 1) / 2f);

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
        //プレイモードアンロック画面の無効化
        GM.unlockPlay.SetActive(false);

        //画面タップ検出ボタンの無効化
        IM.InputUISetActive_Screen(false);
    }
}
