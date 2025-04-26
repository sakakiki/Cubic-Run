using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayButtonSettingController : MonoBehaviour
{
    [SerializeField] private GameObject selectPattern;
    [SerializeField] private RectTransform selectSquareRtf;
    [SerializeField] private RectTransform[] previewRtf;

    [SerializeField] private GameObject setButton;
    [SerializeField] private GameObject[] buttonAllocator;
    [SerializeField] private ActionSelectorSet[] actionSelectorSet;

    private int patternNum;
    private int[] actionAllocation = new int[3];



    private enum ButtonSettingState
    {
        PatternSelect,
        ButtonSet
    }
    private ButtonSettingState currentSettingState;

    public void OnEnable()
    {
        //ステートの初期化
        currentSettingState = ButtonSettingState.PatternSelect;

        //UIをセット
        selectPattern.SetActive(true);
        setButton.SetActive(false);

        //現在の選択を反映
        patternNum = InputManager.Instance.playButtonPatternNum;
        selectSquareRtf.anchoredPosition = previewRtf[patternNum].anchoredPosition;
    }

    public void OnPushOK()
    {
        switch (currentSettingState)
        {
            case ButtonSettingState.PatternSelect:

                //ステート切り替え
                currentSettingState = ButtonSettingState.ButtonSet;

                //UIの切り替え
                selectPattern.SetActive(false);
                setButton.SetActive(true);

                //割り当てUIの切り替え
                for (int i = 0; i < buttonAllocator.Length; i++)
                    buttonAllocator[i].SetActive(false);
                buttonAllocator[patternNum].SetActive(true);

                //SEを一時的に停止
                float tempVolume = AudioManager.Instance.volume_SE;
                AudioManager.Instance.SetVolumeSE(0);

                //ボタン割り当てUIのリセット
                for (int i = 0; i < actionAllocation.Length; i++)
                {
                    actionAllocation[i] = InputManager.Instance.actionAllocation[i];
                    actionSelectorSet[patternNum].actionSelector[i].value = actionAllocation[i];
                }

                //SEを戻す
                AudioManager.Instance.SetVolumeSE(tempVolume);

                break;



            case ButtonSettingState.ButtonSet:

                //設定の適用
                InputManager.Instance.playButtonPatternNum = patternNum;
                for (int i = 0; i < actionAllocation.Length; i++)
                    InputManager.Instance.actionAllocation[i] = actionAllocation[i];
                InputManager.Instance.BindEvent();

                //設定を保存
                PlayerPrefs.SetInt("ButtonPattern", patternNum);
                for (int i = 0; i < actionAllocation.Length; i++)
                    PlayerPrefs.SetInt("ActionAllocation_" + i, actionAllocation[i]);

                //ボタン配置完了報告
                PopupUIManager.Instance.SetupMessageBand("ボタンの設定を保存しました", 1.5f);

                //ボタン配置画面を閉じる
                GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);

                break;
        }
    }

    public void OnPushCancel()
    {
        switch (currentSettingState)
        {
            case ButtonSettingState.PatternSelect:

                //ボタン配置画面を閉じる
                GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);

                break;



            case ButtonSettingState.ButtonSet:

                //ステート切り替え
                currentSettingState = ButtonSettingState.PatternSelect;

                //UIの切り替え
                selectPattern.SetActive(true);
                setButton.SetActive(false);

                break;
        }
    }

    public void SelectPattern(int patternNum)
    {
        //分割パターンを一時保存
        this.patternNum = patternNum;

        //選択枠の移動
        selectSquareRtf.anchoredPosition = previewRtf[patternNum].anchoredPosition;

        //SEの再生
        AudioManager.Instance.PlaySE(AudioManager.SE.Panel);
    }

    public void AllocateAction(int buttonNum)
    {
        //ボタンの割り当て
        actionAllocation[buttonNum] = actionSelectorSet[patternNum].actionSelector[buttonNum].value;

        //SEの再生
        AudioManager.Instance.PlaySE(AudioManager.SE.Close);

        //更新していないボタンの割り当てチェック
        for (int i = 1; i < actionAllocation.Length; i++)
        {
            int checkButtonNum = (buttonNum + i)%actionAllocation.Length;

            //他のボタン全てをチェック
            for (int j = 1; j < actionAllocation.Length; j++)
            {
                int otherButtonNum = (checkButtonNum + j)%actionAllocation.Length;

                //ボタンの割り当てが被っていれば再割り当て
                if (actionAllocation[checkButtonNum] == actionAllocation[otherButtonNum])
                {
                    actionAllocation[checkButtonNum] = (actionAllocation[checkButtonNum] + 1) % actionAllocation.Length;

                    //再チェック
                    j = 0;
                    continue;
                }
            }

            //チェックが問題なければ再割り当て
            actionSelectorSet[patternNum].actionSelector[checkButtonNum].value = actionAllocation[checkButtonNum];
        }
    }
}



[System.Serializable]
public class ActionSelectorSet
{
    public TMP_Dropdown[] actionSelector;
}
