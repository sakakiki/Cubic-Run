using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RankingBoard: MonoBehaviour, IPointerDownHandler
{
    private static GameManager GM;
    private static GameStateStateMachine gameStateMachine;
    private float rankingUpdateTime = -10;  //rankingUpdateSpan * -1 �ɂ���
    private float rankingUpdateSpan = 10;   //�����L���O�X�V�̍ŒZ�Ԋu
    [SerializeField] RectTransform contentFieldRtf;
    [SerializeField] private string scoreType;
    [SerializeField] private RankingBoard anotherTab;
    [SerializeField] private TextMeshProUGUI tabText;
    private List<(string name, int score, int experience, int skin)> rankingList 
        = new List<(string name, int score, int experience, int skin)>();
    private (int userRank, float userPercentile) userRankData = (1, 0);
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private Image[] modelImage;
    [SerializeField] private Image[] crystalImage;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts;
    [SerializeField] private TextMeshProUGUI[] playerRankTexts;
    [SerializeField] private Image[] panelCovers;
    [SerializeField] private TextMeshProUGUI scoreNameText;
    [SerializeField] private TextMeshProUGUI userScoreText;
    [SerializeField] private TextMeshProUGUI userRankPercentileText;

    public void Start()
    {
        if (GM == null) 
            GM = GameManager.Instance;
        if (gameStateMachine == null)
            gameStateMachine = GameManager.Instance.gameStateMachine;
    }

    //�^�b�v�F�^�b�v���Ɏ��s
    //�}�E�X�F�N���b�N���Ɏ��s
    public async void OnPointerDown(PointerEventData eventData)
    {
        //���j���[��ʂłȂ���Ή������Ȃ�
        if (gameStateMachine.currentState != gameStateMachine.state_Menu) return;

        //���̃X�N���v�g�𖳌���
        this.enabled = false;

        await UpdateRanking();

        //������Z��
        tabText.color = Color.black;

        //�����̃X�N���v�g��L����
        anotherTab.enabled = true;
    }



    //���̎擾�E�\���̍X�V
    public async Task UpdateRanking()
    {
        //�X�N���v�g���L����ԂȂ牽�����Ȃ�
        if (this.enabled) return;

        //�O�񃉃��L���O�X�V����w�莞�Ԉȏ�o�߂��Ă���Ώ����Ď擾
        if (Time.time - rankingUpdateTime > rankingUpdateSpan)
        {
            //�����L���O�X�V�������X�V
            rankingUpdateTime = Time.time;

            //�g�b�v10�̃��X�g���擾
            List<(string name, int score, int experience, int skin)> newList
                = await FirestoreManager.Instance.GetTop10Ranking(scoreType);
            //����Ɏ擾�ł��Ă�΍X�V
            if (newList.Count == playerNameTexts.Length)
                rankingList = newList;

            //���[�U�[�̏��ʂƏ�ʃp�[�Z���g�̎擾
            (int rank, float percentile) newUserData =
                await FirestoreManager.Instance.GetUserRanking(
                    scoreType, 
                    scoreType == "highScore" ? GM.highScore : GM.GetPlayerScore());
            //����Ɏ擾�ł��Ă�΍X�V
            if (newUserData.rank != -1)
                userRankData = newUserData;
        }

        //�����L���O�\���ʒu��1�ʂ̏ꏊ�ɖ߂�
        contentFieldRtf.localPosition = Vector2.zero;

        //�p�l���ɐF�����Ă���΃��Z�b�g
        for (int i = 0; i < panelCovers.Length; i++)
            panelCovers[i].color = Color.clear;

        #region �����L���O���̔��f
        //�g�b�v10�����L���O�̔��f
        for (int i = 0; rankingList.Count > i; i++)
        {
            scoreTexts[i].SetText("" + rankingList[i].score);
            playerNameTexts[i].SetText(rankingList[i].name);
            modelImage[i].sprite = (rankingList[i].skin < 8) ? GM.squarSprite : GM.cicleSprite;
            modelImage[i].color = SkinDataBase.Instance.skinData[rankingList[i].skin].skinColor;
            crystalImage[i].enabled = (rankingList[i].skin == 7 || rankingList[i].skin == 15);
            playerRankTexts[i].SetText("" + GM.CalculatePlayerRank(rankingList[i].experience));
        }

        //���[�U�[�̕\�����X�V
        scoreNameText.SetText(scoreType == "highScore" ? "�n�C�X�R�A" : "�v���C���[�X�R�A");
        userScoreText.SetText(((scoreType == "highScore") ? GM.highScore : GM.GetPlayerScore()).ToString());
        userRankPercentileText.SetText(
            userRankData.userRank + "��\n���" + 
            userRankData.userPercentile.ToString("F1", CultureInfo.CurrentCulture) + "%");

        //���[�U�[���g�b�v10�ɓ����Ă���΃p�l����ڗ�������
        if (userRankData.userRank <= rankingList.Count)
            panelCovers[userRankData.userRank-1].color = GameManager.Instance.panelSelectedColor - Color.black * 0.9f;
        #endregion
    }



    //�X�N���v�g���L�����i�I�������j��
    private void OnEnable()
    {
        //�����𔖂�
        tabText.color = Color.gray;
    }
}
