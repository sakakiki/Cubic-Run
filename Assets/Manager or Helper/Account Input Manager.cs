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
                PopupUIManager.Instance.SetupPopupMessage("ログイン失敗", "ログインに失敗しました。", this.Login);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("ログイン失敗", resultMessage, this.Login);
                break;
        }
    }
    #endregion

    #region メールアドレス変更
    public void CangeEmail_Reauthenticate()
    {
        PopupUIManager.Instance.SetupPopup(
            "再ログイン",
            "現在のメールアドレスを入力してください。",
            TMP_InputField.ContentType.EmailAddress,
            "アカウントに設定したパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            this.CangeEmail);

    }
    public async void CangeEmail()
    {
        string resultMessage =
            await AuthManager.Instance.Reauthenticate(
            PopupUIManager.Instance.inputText1,
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopup(
                    "メールアドレス変更",
                    "新しいメールアドレスを入力してください。",
                    TMP_InputField.ContentType.EmailAddress,
                    this.CangeEmail_Result);
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("再ログイン失敗", resultMessage, this.CangeEmail_Reauthenticate);
                break;
        }
    }
    public async void CangeEmail_Result()
    {
        string resultMessage =
            await AuthManager.Instance.UpdateUserEmail(
            PopupUIManager.Instance.inputText1);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "メールアドレス変更",
                    PopupUIManager.Instance.inputText1 + "\n\n" +
                    "上記メールアドレスに認証メールを送信しています。\n" +
                    "リンクを開いてアドレスの認証を行い、下の OK のボタンをを押してください。",
                    this.CangeEmail_Check);
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("メールアドレス変更失敗", resultMessage, this.CangeEmail);
                break;
        }
    }
    public async void CangeEmail_Check()
    {
        Debug.Log("チェック");

        string resultMessage =
            await AuthManager.Instance.Reauthenticate(
            PopupUIManager.Instance.inputText1,
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "メールアドレス変更完了",
                    "メールアドレスの変更が完了しました。");
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupPopup(
                    "メールアドレス未認証",
                    PopupUIManager.Instance.inputText1 + "\n\n" +
                    "上記メールアドレスに認証メールを送信しています。\n" +
                    "リンクを開いてアドレスの認証を行い、下の OK のボタンをを押してください。",
                    this.CangeEmail_Check);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopup(
                    "メールアドレス未認証",
                    PopupUIManager.Instance.inputText1 + "\n\n" +
                    "上記メールアドレスに認証メールを送信しています。\n" +
                    "リンクを開いてアドレスの認証を行い、下の OK のボタンをを押してください。",
                    this.CangeEmail_Check);
                break;
        }
    }
    #endregion

    #region パスワード変更
    public void CangePassword_Reauthenticate()
    {
        PopupUIManager.Instance.SetupPopup(
            "再ログイン",
            "アカウントに登録したメールアドレスを入力してください。",
            TMP_InputField.ContentType.EmailAddress,
            "現在のパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            this.CangePassword);
    }
    public async void CangePassword()
    {
        string resultMessage =
            await AuthManager.Instance.Reauthenticate(
            PopupUIManager.Instance.inputText1,
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopup(
                    "パスワード変更",
                    "新しいパスワードを入力してください。",
                    TMP_InputField.ContentType.Password,
                    this.CangePassword_Result);
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("再ログイン失敗", resultMessage, this.CangePassword_Reauthenticate);
                break;
        }
    }
    public async void CangePassword_Result()
    {
        string resultMessage =
            await AuthManager.Instance.UpdatePassword(
            PopupUIManager.Instance.inputText1);

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "パスワード変更",
                    "パスワードの変更が完了しました。");
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("パスワード変更失敗", resultMessage, this.CangePassword);
                break;
        }
    }
    #endregion

    #region パスワードリセット
    public void ResetPassword()
    {
        PopupUIManager.Instance.SetupPopup(
            "パスワードリセット",
            AuthManager.Instance.GetEmail() + "\n\n" +
            "上記メールアドレスにパスワード再設定用のメールを送信します。",
            this.ResetPassword_Result);

    }
    public async void ResetPassword_Result()
    {
        string resultMessage =
            await AuthManager.Instance.SendPasswordResetEmail(
            AuthManager.Instance.GetEmail());

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "メール送信完了",
                    "パスワード再設定用のメールを送信しました。\n" +
                    "リンクを開き、パスワードをリセットしてください。\n\n" +
                    "<color=#E20000><B>パスワード変更後は再ログインが必要です。</color></B>",
                ResetPassword_ReLogin);
                break;

            case "異常終了":
                PopupUIManager.Instance.SetupMessageBand("エラーが発生しました。", 2);
                break;

            case "ネットワークエラー":
                PopupUIManager.Instance.SetupMessageBand(
                    "ネットワークエラーが発生しました。接続を確認してください。", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("メール送信失敗", resultMessage);
                break;
        }
    }
    public void ResetPassword_ReLogin()
    {
        PopupUIManager.Instance.SetupPopup(
            "ログイン",
            "アカウントに登録されているメールアドレスを入力してください。",
            TMP_InputField.ContentType.EmailAddress,
            "アカウントに設定したパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            this.Login_Result);
    }
    #endregion

    #region 認証メール再送
    public void SendEmail()
    {
        PopupUIManager.Instance.SetupPopup(
            "認証メール再送",
            AuthManager.Instance.GetEmail() + "\n\n" +
            "上記メールアドレスに認証メールを送信します。",
            this.SendEmail_Result);

    }
    public async void SendEmail_Result()
    {
        string resultMessage =
            await AuthManager.Instance.SendEmailVerification();

        switch (resultMessage)
        {
            case "正常終了":
                PopupUIManager.Instance.SetupPopupMessage(
                    "メール送信完了",
                    "認証メールを送信しました。\n\n" +
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
                PopupUIManager.Instance.SetupPopupMessage("ログイン失敗", resultMessage, this.Login);
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
