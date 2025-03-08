using TMPro;
using UnityEngine;

public class AccountInputManager : MonoBehaviour
{
    #region �A�J�E���g�o�^
    public void Registration()
    {
        PopupUIManager.Instance.SetupPopup(
            "�A�J�E���g�o�^",
            "�A�J�E���g�Ƃ��Ďg�p���郁�[���A�h���X����͂��Ă��������B",
            TMP_InputField.ContentType.EmailAddress,
            "�A�J�E���g�ɓo�^����p�X���[�h����͂��Ă��������B",
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
            case"����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "�A�J�E���g�o�^����",
                    "���[���A�h���X�ƃp�X���[�h��o�^���܂����B\n\n" +
                    AuthManager.Instance.GetEmail() + "\n\n" +
                    "��L���[���A�h���X�ɔF�؃��[���𑗐M���Ă��܂��B\n" +
                    "�����N���J���A�A�h���X�̔F�؂��s���Ă��������B");
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("�A�J�E���g�o�^�G���[", resultMessage, Registration);
                break;
        }
    }
    #endregion

    #region ���O�C��
    public void Login()
    {
        PopupUIManager.Instance.SetupPopup(
            "���O�C��",
            "�A�J�E���g�ɓo�^����Ă��郁�[���A�h���X����͂��Ă��������B",
            TMP_InputField.ContentType.EmailAddress,
            "�A�J�E���g�ɐݒ肵���p�X���[�h����͂��Ă��������B",
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
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "���O�C������",
                    "�A�J�E���g�̃��O�C���ɐ������܂����B\n\n" +
                    "�ă��O�C�����܂��B",
                    ReLogin);
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupPopupMessage("���O�C�����s", "���O�C���Ɏ��s���܂����B", this.Login);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("���O�C�����s", resultMessage, this.Login);
                break;
        }
    }
    #endregion

    #region ���[���A�h���X�ύX
    public void CangeEmail_Reauthenticate()
    {
        PopupUIManager.Instance.SetupPopup(
            "�ă��O�C��",
            "���݂̃��[���A�h���X����͂��Ă��������B",
            TMP_InputField.ContentType.EmailAddress,
            "�A�J�E���g�ɐݒ肵���p�X���[�h����͂��Ă��������B",
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
            case "����I��":
                PopupUIManager.Instance.SetupPopup(
                    "���[���A�h���X�ύX",
                    "�V�������[���A�h���X����͂��Ă��������B",
                    TMP_InputField.ContentType.EmailAddress,
                    this.CangeEmail_Result);
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("�ă��O�C�����s", resultMessage, this.CangeEmail_Reauthenticate);
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
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "���[���A�h���X�ύX",
                    PopupUIManager.Instance.inputText1 + "\n\n" +
                    "��L���[���A�h���X�ɔF�؃��[���𑗐M���Ă��܂��B\n" +
                    "�����N���J���ăA�h���X�̔F�؂��s���A���� OK �̃{�^�����������Ă��������B",
                    this.CangeEmail_Check);
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("���[���A�h���X�ύX���s", resultMessage, this.CangeEmail);
                break;
        }
    }
    public async void CangeEmail_Check()
    {
        Debug.Log("�`�F�b�N");

        string resultMessage =
            await AuthManager.Instance.Reauthenticate(
            PopupUIManager.Instance.inputText1,
            PopupUIManager.Instance.inputText2);

        switch (resultMessage)
        {
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "���[���A�h���X�ύX����",
                    "���[���A�h���X�̕ύX���������܂����B");
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupPopup(
                    "���[���A�h���X���F��",
                    PopupUIManager.Instance.inputText1 + "\n\n" +
                    "��L���[���A�h���X�ɔF�؃��[���𑗐M���Ă��܂��B\n" +
                    "�����N���J���ăA�h���X�̔F�؂��s���A���� OK �̃{�^�����������Ă��������B",
                    this.CangeEmail_Check);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopup(
                    "���[���A�h���X���F��",
                    PopupUIManager.Instance.inputText1 + "\n\n" +
                    "��L���[���A�h���X�ɔF�؃��[���𑗐M���Ă��܂��B\n" +
                    "�����N���J���ăA�h���X�̔F�؂��s���A���� OK �̃{�^�����������Ă��������B",
                    this.CangeEmail_Check);
                break;
        }
    }
    #endregion

    #region �p�X���[�h�ύX
    public void CangePassword_Reauthenticate()
    {
        PopupUIManager.Instance.SetupPopup(
            "�ă��O�C��",
            "�A�J�E���g�ɓo�^�������[���A�h���X����͂��Ă��������B",
            TMP_InputField.ContentType.EmailAddress,
            "���݂̃p�X���[�h����͂��Ă��������B",
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
            case "����I��":
                PopupUIManager.Instance.SetupPopup(
                    "�p�X���[�h�ύX",
                    "�V�����p�X���[�h����͂��Ă��������B",
                    TMP_InputField.ContentType.Password,
                    this.CangePassword_Result);
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("�ă��O�C�����s", resultMessage, this.CangePassword_Reauthenticate);
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
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "�p�X���[�h�ύX",
                    "�p�X���[�h�̕ύX���������܂����B");
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("�p�X���[�h�ύX���s", resultMessage, this.CangePassword);
                break;
        }
    }
    #endregion

    #region �p�X���[�h���Z�b�g
    public void ResetPassword()
    {
        PopupUIManager.Instance.SetupPopup(
            "�p�X���[�h���Z�b�g",
            AuthManager.Instance.GetEmail() + "\n\n" +
            "��L���[���A�h���X�Ƀp�X���[�h�Đݒ�p�̃��[���𑗐M���܂��B",
            this.ResetPassword_Result);

    }
    public async void ResetPassword_Result()
    {
        string resultMessage =
            await AuthManager.Instance.SendPasswordResetEmail(
            AuthManager.Instance.GetEmail());

        switch (resultMessage)
        {
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "���[�����M����",
                    "�p�X���[�h�Đݒ�p�̃��[���𑗐M���܂����B\n" +
                    "�����N���J���A�p�X���[�h�����Z�b�g���Ă��������B\n\n" +
                    "<color=#E20000><B>�p�X���[�h�ύX��͍ă��O�C�����K�v�ł��B</color></B>",
                ResetPassword_ReLogin);
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("���[�����M���s", resultMessage);
                break;
        }
    }
    public void ResetPassword_ReLogin()
    {
        PopupUIManager.Instance.SetupPopup(
            "���O�C��",
            "�A�J�E���g�ɓo�^����Ă��郁�[���A�h���X����͂��Ă��������B",
            TMP_InputField.ContentType.EmailAddress,
            "�A�J�E���g�ɐݒ肵���p�X���[�h����͂��Ă��������B",
            TMP_InputField.ContentType.Password,
            this.Login_Result);
    }
    #endregion

    #region �F�؃��[���đ�
    public void SendEmail()
    {
        PopupUIManager.Instance.SetupPopup(
            "�F�؃��[���đ�",
            AuthManager.Instance.GetEmail() + "\n\n" +
            "��L���[���A�h���X�ɔF�؃��[���𑗐M���܂��B",
            this.SendEmail_Result);

    }
    public async void SendEmail_Result()
    {
        string resultMessage =
            await AuthManager.Instance.SendEmailVerification();

        switch (resultMessage)
        {
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "���[�����M����",
                    "�F�؃��[���𑗐M���܂����B\n\n" +
                    "�����N���J���A�A�h���X�̔F�؂��s���Ă��������B");
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("���O�C�����s", resultMessage, this.Login);
                break;
        }
    }
    #endregion

    #region �f�[�^�폜
    public void DeleteData()
    {
        PopupUIManager.Instance.SetupPopup(
            "�f�[�^�폜",
            "���݂̃A�J�E���g�̃f�[�^���폜���܂��B\n" +
            "<color=#E20000><B>���̑���͎������܂���B</color></B>\n\n" +
            "�{���ɂ�낵���ł����H",
            DeleteData_Result);
    }
    public async void DeleteData_Result()
    {
        if (await FirestoreManager.Instance.SaveNewPlayerData())
        {
            PopupUIManager.Instance.SetupPopupMessage(
                "�f�[�^�폜����",
                "�f�[�^�̍폜���������܂����B\n\n" +
                "�ă��O�C�����܂��B",
                ReLogin);
        }
        else
        {
            PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
        }
    }
    public void ReLogin()
    {
        GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Login);
    }
    #endregion
}
