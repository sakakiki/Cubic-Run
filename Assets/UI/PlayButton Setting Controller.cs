using TMPro;
using UnityEngine;

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
        //�X�e�[�g�̏�����
        currentSettingState = ButtonSettingState.PatternSelect;

        //UI���Z�b�g
        selectPattern.SetActive(true);
        setButton.SetActive(false);

        //���݂̑I���𔽉f
        patternNum = InputManager.Instance.playButtonPatternNum;
        selectSquareRtf.anchoredPosition = previewRtf[patternNum].anchoredPosition;
    }

    public void OnPushOK()
    {
        switch (currentSettingState)
        {
            case ButtonSettingState.PatternSelect:

                //�X�e�[�g�؂�ւ�
                currentSettingState = ButtonSettingState.ButtonSet;

                //UI�̐؂�ւ�
                selectPattern.SetActive(false);
                setButton.SetActive(true);

                //���蓖��UI�̐؂�ւ�
                for (int i = 0; i < buttonAllocator.Length; i++)
                    buttonAllocator[i].SetActive(false);
                buttonAllocator[patternNum].SetActive(true);

                //�{�^�����蓖��UI�̃��Z�b�g
                for (int i = 0; i < actionAllocation.Length; i++)
                {
                    actionAllocation[i] = InputManager.Instance.actionAllocation[i];
                    actionSelectorSet[patternNum].actionSelector[i].value = actionAllocation[i];
                }

                break;



            case ButtonSettingState.ButtonSet:

                //�ݒ�̓K�p
                InputManager.Instance.playButtonPatternNum = patternNum;
                for (int i = 0; i < actionAllocation.Length; i++)
                    InputManager.Instance.actionAllocation[i] = actionAllocation[i];
                InputManager.Instance.BindEvent();

                //�ݒ��ۑ�
                PlayerPrefs.SetInt("ButtonPattern", patternNum);
                for (int i = 0; i < actionAllocation.Length; i++)
                    PlayerPrefs.SetInt("ActionAllocation_" + i, actionAllocation[i]);

                //�{�^���z�u������
                PopupUIManager.Instance.SetupMessageBand("�{�^���̐ݒ��ۑ����܂���", 1.5f);

                //�{�^���z�u��ʂ����
                GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);

                break;
        }
    }

    public void OnPushCancel()
    {
        switch (currentSettingState)
        {
            case ButtonSettingState.PatternSelect:

                //�{�^���z�u��ʂ����
                GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);

                break;



            case ButtonSettingState.ButtonSet:

                //�X�e�[�g�؂�ւ�
                currentSettingState = ButtonSettingState.PatternSelect;

                //UI�̐؂�ւ�
                selectPattern.SetActive(true);
                setButton.SetActive(false);

                break;
        }
    }

    public void SelectPattern(int patternNum)
    {
        //�����p�^�[�����ꎞ�ۑ�
        this.patternNum = patternNum;

        //�I��g�̈ړ�
        selectSquareRtf.anchoredPosition = previewRtf[patternNum].anchoredPosition;

        //SE�̍Đ�
        AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Panel);
    }

    public void AllocateAction(int buttonNum)
    {
        //�{�^���̊��蓖��
        actionAllocation[buttonNum] = actionSelectorSet[patternNum].actionSelector[buttonNum].value;

        //SE�̍Đ�
        AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Close);

        //�X�V���Ă��Ȃ��{�^���̊��蓖�ă`�F�b�N
        for (int i = 1; i < actionAllocation.Length; i++)
        {
            int checkButtonNum = (buttonNum + i)%actionAllocation.Length;

            //���̃{�^���S�Ă��`�F�b�N
            for (int j = 1; j < actionAllocation.Length; j++)
            {
                int otherButtonNum = (checkButtonNum + j)%actionAllocation.Length;

                //�{�^���̊��蓖�Ă�����Ă���΍Ċ��蓖��
                if (actionAllocation[checkButtonNum] == actionAllocation[otherButtonNum])
                {
                    actionAllocation[checkButtonNum] = (actionAllocation[checkButtonNum] + 1) % actionAllocation.Length;

                    //�ă`�F�b�N
                    j = 0;
                    continue;
                }
            }

            //�`�F�b�N�����Ȃ���΍Ċ��蓖��
            actionSelectorSet[patternNum].actionSelector[checkButtonNum].value = actionAllocation[checkButtonNum];
        }
    }
}



[System.Serializable]
public class ActionSelectorSet
{
    public TMP_Dropdown[] actionSelector;
}
