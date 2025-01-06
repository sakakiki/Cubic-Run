using UnityEngine;
using UnityEngine.EventSystems;

public class Button_BoardTab : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Button_BoardTab anotherTab;
    [SerializeField] private SpriteRenderer boardSprite;
    [SerializeField] private bool isTraining;
    [SerializeField] private GameObject[] contents;

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public void OnPointerDown(PointerEventData eventData)
    {
        //�{�[�h�̕\����O��
        this.boardSprite.sortingOrder = 0;

        //�����̃X�N���v�g��L����
        anotherTab.enabled = true;

        //�Q�[���}�l�[�W���[�̐ݒ��ύX
        GameManager.Instance.SetTrainingMode(isTraining);

        //�R���e���c�̗L����
        foreach (var item in contents)
            item.SetActive(true);

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
