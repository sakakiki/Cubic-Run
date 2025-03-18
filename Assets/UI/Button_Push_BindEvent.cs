using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button_Push_BindEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent eventOnPush;
    private Vector3 defaultScale;
    [SerializeField] private SpriteRenderer cover;
    [SerializeField] private AudioClip clipSE = null;

    private void Start()
    {
        //��{�̃X�P�[�����L��
        defaultScale = transform.localScale;

        //���ʉ������ݒ�Ȃ�f�t�H���g�̌��ʉ���o�^
        if (clipSE == null) clipSE = AudioManager.Instance.SE_Button;
    }

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public void OnPointerDown(PointerEventData eventData)
    {
        //�X�P�[�����g��
        transform.localScale *= 1.05f;

        //�J�o�[�𒅐F
        cover.color = GameManager.Instance.panelSelectedColor - Color.black * 0.95f;

        //SE���Đ�
        AudioManager.Instance.PlaySE(clipSE);
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
        //�C�x���g�����s
        eventOnPush?.Invoke();
    }

    //�X�N���v�g�������������΃��Z�b�g����
    private void OnDisable()
    {
        //�X�P�[������{�̃X�P�[���ɖ߂�
        transform.localScale = defaultScale;

        //�J�o�[�𓧖���
        cover.color = Color.clear;
    }
}
