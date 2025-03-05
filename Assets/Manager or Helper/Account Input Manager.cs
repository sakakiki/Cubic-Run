using TMPro;
using UnityEngine;

public class AccountInputManager : MonoBehaviour
{
    public static string email;



    public static void Registration_Email()
    {
        PopupUIManager.Instance.SetupPopup(
            "アカウントとして使用するメールアドレスを入力してください。\n" +
            "実際に使用できるメールアドレスである必要があります。",
            TMP_InputField.ContentType.EmailAddress,
            Registration_Password);
    }

    public static void Registration_Password()
    {
        email = PopupUIManager.Instance.inputText1;
        PopupUIManager.Instance.SetupPopup(
            "アカウントに登録するパスワードを入力してください。",
            TMP_InputField.ContentType.Password,
            "確認のためもう一度入力してください。",
            TMP_InputField.ContentType.Password,
            Registration_3);
    }

    public static async void Registration_3()
    {
        if (PopupUIManager.Instance.inputText1 == PopupUIManager.Instance.inputText2)
        {
            await AuthManager.Instance.ConvertAnonymousToEmail(email, PopupUIManager.Instance.inputText1);
            //PopupUIManager.Instance.SetupMessageBand("")
        }
        else
        {

        }
    }
}
