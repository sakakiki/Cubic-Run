using TMPro;
using UnityEngine;

public class AccountInputManager : MonoBehaviour
{
    public static string email;



    public static void Registration_Email()
    {
        PopupUIManager.Instance.SetupPopup(
            "�A�J�E���g�Ƃ��Ďg�p���郁�[���A�h���X����͂��Ă��������B\n" +
            "���ۂɎg�p�ł��郁�[���A�h���X�ł���K�v������܂��B",
            TMP_InputField.ContentType.EmailAddress,
            Registration_Password);
    }

    public static void Registration_Password()
    {
        email = PopupUIManager.Instance.inputText1;
        PopupUIManager.Instance.SetupPopup(
            "�A�J�E���g�ɓo�^����p�X���[�h����͂��Ă��������B",
            TMP_InputField.ContentType.Password,
            "�m�F�̂��߂�����x���͂��Ă��������B",
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
