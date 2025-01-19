using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_LevelSelecter: MonoBehaviour, IPointerClickHandler
{
    public bool isEnable;
    [SerializeField] private Transform tf;
    private int level;
    [SerializeField] private TextMeshProUGUI levelTMP;
    public TextMeshProUGUI clearTimesNumTMP;
    [SerializeField] private Image image;
    [SerializeField] private Image cover;
    private Color panelColor;
    private Color defaultColor = Color.black;
    private Color selectedColor = Color.blue;
    private float boardCenterY = -1;

    //�^�b�v�F�^�b�v��Ɏw����ʂ��痣�ꂽ�Ƃ��A�w���I�u�W�F�N�g��Ȃ���s
    //�}�E�X�F�N���b�N��Ƀ}�E�X�{�^���𗣂����Ƃ��A�J�[�\�����I�u�W�F�N�g��Ȃ���s
    public void OnPointerClick(PointerEventData eventData)
    {
        //�L����ԂłȂ���ΏI��
        if (!isEnable) return;

        //���͂̎󂯓n��
        GameManager.Instance.SetTrainingLevel(level);

        PushButton();
    }

    //��������
    public void PushButton()
    {
        //�\����I����Ԃ֕ω�
        image.color = selectedColor;
        cover.color = selectedColor - Color.black * 0.95f;

        //���͂𖳌���
        isEnable = false;
    }

    //����������
    public void Initialize()
    {
        //�\�����I����Ԃ֕ω�
        image.color = defaultColor;
        cover.color = Color.clear;

        //���͂�L����
        isEnable = true;
    }

    //���̃X�N���v�g�̃��x����ݒ�
    public void SetLevel(int level)
    {
        this.level = level;
        levelTMP.SetText("" + level);
    }
}
