using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Push : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isPushed;
    private Vector3 defaultScale;
    [SerializeField] private SpriteRenderer cover;

    private void Start()
    {
        //��{�̃X�P�[�����L��
        defaultScale = transform.localScale;
    }

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public void OnPointerDown(PointerEventData eventData)
    {
        //�X�P�[�����g��
        transform.localScale *= 1.05f;

        //�J�o�[�𒅐F
        cover.color = GameManager.Instance.panelSelectedColor - Color.black * 0.95f;
    }

    //�^�b�v�F�^�b�v��Ɏw����ʂ��痣�ꂽ�Ƃ��A�w�̏ꏊ�Ɋւ�炸���s
    //�}�E�X�F�N���b�N��Ƀ}�E�X�{�^���𗣂����Ƃ��A�J�[�\���̈ʒu�Ɋւ�炸���s
    public void OnPointerUp(PointerEventData eventData)
    {
        //�X�P�[������{�̃X�P�[���ɖ߂�
        transform.localScale = defaultScale;

        //�J�o�[�𓧖���
        cover.color =�@Color.clear;
    }

    //�^�b�v�F�^�b�v��Ɏw����ʂ��痣�ꂽ�Ƃ��A�w���I�u�W�F�N�g��Ȃ���s
    //�}�E�X�F�N���b�N��Ƀ}�E�X�{�^���𗣂����Ƃ��A�J�[�\�����I�u�W�F�N�g��Ȃ���s
    public void OnPointerClick(PointerEventData eventData)
    {
        //�����̃t���O�𗧂Ă�
        isPushed = true;
    }

    //Input Manager������Ăяo��
    //�����̗L����Ԃ�
    public bool GetIsPushed()
    {
        //�Ԃ�l��ۑ�
        bool returnValue = isPushed;

        //�����̃t���O�����Z�b�g
        isPushed = false;

        return returnValue;
    }

    //�X�N���v�g�������������΃��Z�b�g����
    private void OnDisable()
    {
        //�X�P�[������{�̃X�P�[���ɖ߂�
        transform.localScale = defaultScale;

        //�J�o�[�𓧖���
        cover.color = Color.clear;

        //�����̃t���O�����Z�b�g
        isPushed = false;
    }
}
