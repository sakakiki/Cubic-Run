using UnityEngine;
using UnityEngine.EventSystems;

public class Button_BoardTab : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Button_BoardTab anotherTab;
    [SerializeField] private SpriteRenderer boardSprite;

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public void OnPointerDown(PointerEventData eventData)
    {
        //�{�[�h�̕\����O��
        this.boardSprite.sortingOrder = 0;

        //�����̃X�N���v�g��L����
        anotherTab.enabled = true;

        //���̃X�N���v�g�𖳌���
        this.enabled = false;
    }

    //�X�N���v�g���L���������΃{�[�h�̕\��������
    private void OnEnable()
    {
        this.boardSprite.sortingOrder = -1;
    }
}
