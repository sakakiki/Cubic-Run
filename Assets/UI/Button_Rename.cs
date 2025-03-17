using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Rename : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //SE�Đ�
        AudioManager.Instance.PlaySE(AudioManager.Instance.SE_Panel);

        PopupUIManager.Instance.SetupPopup(
            "�v���C���[���ύX",
            "���p12�����i�S�p6�����j�ȓ���\n���͂��Ă��������B\n\n" +
            "<color=#E20000><B>" +
            "�s�K�؂ȃv���C���[�����m�F���ꂽ\n�ꍇ�A�A�J�E���g�������܂��͍폜\n����邱�Ƃ�����܂��B" +
            "</color></B>",
            GameManager.Instance.playerName,
            this.Rename_Result);
    }

    public async void Rename_Result()
    {
        string resultMessage =
            await FirestoreManager.Instance.SavePlayerName(
            PopupUIManager.Instance.inputText1);

        switch (resultMessage)
        {
            case "����I��":
                GameManager.Instance.UpdatePlayerInfo();
                PopupUIManager.Instance.SetupMessageBand("�v���C���[���̕ύX���������܂����B", 2);
                await RankingManager.UpdateRanking(RankingManager.RankingType.HighScore);
                await RankingManager.UpdateRanking(RankingManager.RankingType.PlayerScore);
                GameManager.Instance.highScoreRankingBoard.UpdateRanking();
                GameManager.Instance.playerScoreRankingBoard.UpdateRanking();
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�v���C���[���̕ύX�Ɏ��s���܂����B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupMessageBand(resultMessage, 2);
                break;
        }
    }
}
