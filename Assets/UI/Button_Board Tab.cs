using UnityEngine;
using UnityEngine.EventSystems;

public class Button_BoardTab : MonoBehaviour, IPointerDownHandler
{
    private GameStateStateMachine gameStateMachine;
    [SerializeField] private Button_BoardTab anotherTab;
    [SerializeField] private SpriteRenderer boardSprite;
    [SerializeField] private bool isTraining;
    [SerializeField] private GameObject[] contents;

    public void Start()
    {
        gameStateMachine = GameManager.Instance.gameStateMachine;
    }

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public void OnPointerDown(PointerEventData eventData)
    {
        //���j���[��ʂłȂ���Ή������Ȃ�
        if (gameStateMachine.currentState != gameStateMachine.state_Menu) return;

        //�{�[�h�̕\����O��
        this.boardSprite.sortingOrder = 0;

        //�����̃X�N���v�g��L����
        anotherTab.enabled = true;

        //�Q�[���}�l�[�W���[�̐ݒ��ύX
        GameManager.Instance.SetTrainingMode(isTraining);

        //�R���e���c�̗L����
        foreach (var item in contents)
            item.SetActive(true);

        //SE�̍Đ�
        AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Panel);

        //���̃X�N���v�g�𖳌���
        this.enabled = false;
    }

    //�X�N���v�g���L������
    private void OnEnable()
    {
        //�{�[�h�̕\��������
        this.boardSprite.sortingOrder = -1;

        //�R���e���c�̖�����
        foreach (var item in contents)
            item.SetActive(false);
    }
}
