using TMPro;
using UnityEngine;

public class AccountInputManager : MonoBehaviour
{
    #region アカウント登録
    public void Registration()
    {
        PopupUIManager.Instance.SetupPopup(
            "アカウント登録",
            "アカウントとして使用するメールアドレスを入力してください。",
            TMP_InputField.ContentType.EmailAddress,
            "アカウントに登録するパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            this.Registration_Result);
    }
    public async void Registration_Result()
    {
        string resultMessage = 
            await AuthManager.Instance.ConvertAnonymousToEmail(
            PopupUIManager.Instance.inputText1, 
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case"正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "アカウント登録完了",
                    "メールアドレスとパスワードを登録しました。\n\n" +
                    AuthManager.Instance.GetEmail() + "\n\n" +
                    "上記メールアドレスに認証メールを送信しています。\n" +
                    "リンクを開き、アドレスの認証を行ってください。");
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("アカウント登録エラー", resultMessage, Registration);
                break;
        }
    }
    #endregion

    #region ログイン
    public void Login()
    {
        PopupUIManager.Instance.SetupPopup(
            "ログイン",
            "アカウントに登録されているメールアドレスを入力してください。",
            TMP_InputField.ContentType.EmailAddress,
            "アカウントに設定したパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            this.Login_Result);

    }
    public async void Login_Result()
    {
        string resultMessage =
            await AuthManager.Instance.Login(
            PopupUIManager.Instance.inputText1,
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "ログイン成功",
                "アカウントのログインに成功しました。\n\n" +
                "再ログインします。",
                ReLogin);
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("ログイン失敗", resultMessage, Login);
                break;
        }
    }
    #endregion

    #region メールアドレス変更
    public void CangeEmail()
    {
        PopupUIManager.Instance.SetupPopup(
            "メールアドレス変更",
            "アカウントに登録されているメールアドレスを入力してください。",
            TMP_InputField.ContentType.EmailAddress,
            "アカウントに設定したパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            this.CangeEmail_Result);

    }
    public async void CangeEmail_Result()
    {
        string resultMessage =
            await AuthManager.Instance.Login(
            PopupUIManager.Instance.inputText1,
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "ログイン成功",
                "アカウントのログインに成功しました。\n\n" +
                "再ログインします。",
                ReLogin);
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("ログイン失敗", resultMessage, Login);
                break;
        }
    }
    #endregion

    #region データ削除
    public void DeleteData()
    {
        PopupUIManager.Instance.SetupPopup(
            "データ削除",
            "現在のアカウントのデータを削除します。\n" +
            "<color=#E20000><B>この操作は取り消せません。</color></B>\n\n" +
            "本当によろしいですか？",
            DeleteData_Result);
    }
    public async void DeleteData_Result()
    {
        if (await FirestoreManager.Instance.SaveNewPlayerData())
        {
            PopupUIManager.Instance.SetupPopupMessage(
                "データ削除完了",
                "データの削除が完了しました。\n\n" +
                "再ログインします。",
                ReLogin);
        }
        else
        {
            PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
        }
    }
    public void ReLogin()
    {
        GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Login);
    }
    #endregion
}
