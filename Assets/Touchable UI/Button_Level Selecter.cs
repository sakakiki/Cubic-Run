using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_LevelSelecter: MonoBehaviour, IPointerClickHandler
{
    private int level;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private RectTransform rtf;
    private Vector2 defaultApos = new Vector2(-15, 15);
    private Vector2 pushedApos = new Vector2(-5, 5);

    //�^�b�v�F�^�b�v��Ɏw����ʂ��痣�ꂽ�Ƃ��A�w���I�u�W�F�N�g��Ȃ���s
    //�}�E�X�F�N���b�N��Ƀ}�E�X�{�^���𗣂����Ƃ��A�J�[�\�����I�u�W�F�N�g��Ȃ���s
    public void OnPointerClick(PointerEventData eventData)
    {
        //������Ԃ֕ω�
        rtf.anchoredPosition = pushedApos;

        //���͂̎󂯓n��
        GameManager.Instance.SetTrainingLevel(level);

        //�X�N���v�g�𖳌���
        this.enabled = false;
    }

    //����������
    public void OnEnable()
    {
        rtf.anchoredPosition = defaultApos;
    }

    //���̃X�N���v�g�̃��x����ݒ�
    public void SetLevel(int level)
    {
        this.level = level;
        levelTMP.SetText("" + level);
    }
}
