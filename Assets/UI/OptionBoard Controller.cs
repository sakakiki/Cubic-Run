using UnityEngine;
using UnityEngine.EventSystems;

public class OptionBoardController : MonoBehaviour, IPointerDownHandler
{
    private GameStateStateMachine gameStateMachine;
    [SerializeField] private SpriteRenderer optionIcon;
    [SerializeField] private RectTransform optionRtf;
    [SerializeField] private SpriteRenderer[] closeIcon;
    [SerializeField] private RectTransform closeRtf;
    [SerializeField] private RectTransform boardRtf;
    private bool isOpen;
    private float openRate;

    public void Start()
    {
        gameStateMachine = GameManager.Instance.gameStateMachine;
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        //���j���[��ʂłȂ���Ή������Ȃ�
        if (gameStateMachine.currentState != gameStateMachine.state_Menu) return;

        //�t���O��؂�ւ�
        isOpen = !isOpen;

        //SE�̍Đ�
        if (isOpen)
            AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Panel);
        if (!isOpen)
            AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Close);
    }



    public void Update()
    {
        //���j���[��ʂ��I�v�V������ʂłȂ���Ε���
        if (gameStateMachine.currentState != gameStateMachine.state_Menu &&
            gameStateMachine.currentState != gameStateMachine.state_Account &&
            gameStateMachine.currentState != gameStateMachine.state_Volume &&
            gameStateMachine.currentState != gameStateMachine.state_Button &&
            gameStateMachine.currentState != gameStateMachine.state_Credit) 
            isOpen = false;

        //�J�����̕K�v������Ȃ�
        if (isOpen && openRate < 1 || !isOpen && openRate > 0)
        {
            //�J����X�V
            openRate += Time.deltaTime * (isOpen ? 5 : -5);

            //�A�C�R���̃J���[�ύX
            optionIcon.color = Color.white - Color.black * openRate;
            closeIcon[0].color = Color.black * openRate;
            closeIcon[1].color = Color.black * openRate;

            //�A�C�R���̉�]
            optionRtf.localEulerAngles = Vector3.forward * Mathf.Lerp(0, 90, openRate);
            closeRtf.localEulerAngles = Vector3.forward * Mathf.Lerp(-90, 0, openRate);

            //�{�[�h�̊J��
            boardRtf.localScale = Vector3.one * Mathf.Lerp(0, 1, openRate);
        }
    }
}
