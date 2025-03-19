using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInfoBoardController : MonoBehaviour, IPointerDownHandler
{
    private GameStateStateMachine gameStateMachine;
    [SerializeField] private RectTransform buttonIconRtf;
    [SerializeField] private RectTransform boardUpRtf;
    [SerializeField] private GameObject addInfo;
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
            AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Panel);
        if (!isOpen)
            AudioManager.Instance.audioSource_SE.PlayOneShot(AudioManager.Instance.SE_Close);
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

            //�A�C�R���̉�]
            buttonIconRtf.localEulerAngles = Vector3.forward * Mathf.Lerp(0, 180, openRate);

            //�{�[�h�̊J��
            boardUpRtf.anchoredPosition = Vector3.up * Mathf.Lerp(0, 200, openRate);

            //�g�����̕\���E��\��
            addInfo.SetActive(openRate > 1);
        }
    }
}
