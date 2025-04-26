using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Rename : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //SE再生
        AudioManager.Instance.PlaySE(AudioManager.SE.Panel);

        PopupUIManager.Instance.SetupPopup(
            "プレイヤー名変更",
            "半角12文字（全角6文字）以内で\n入力してください。\n\n" +
            "<color=#E20000><B>" +
            "不適切なプレイヤー名が確認された\n場合、アカウントが制限または削除\nされることがあります。" +
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
            case "正常終了":
                GameManager.Instance.UpdatePlayerInfo();
                PopupUIManager.Instance.SetupMessageBand("プレイヤー名の変更が完了しました。", 2);
                await RankingManager.UpdateRanking(RankingManager.RankingType.HighScore);
                await RankingManager.UpdateRanking(RankingManager.RankingType.PlayerScore);
                GameManager.Instance.highScoreRankingBoard.UpdateRanking();
                GameManager.Instance.playerScoreRankingBoard.UpdateRanking();
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("プレイヤー名の変更に失敗しました。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupMessageBand(resultMessage, 2);
                break;
        }
    }
}
