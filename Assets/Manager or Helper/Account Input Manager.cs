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
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("���O�C�����s", resultMessage, Login);
                break;
        }
    }
    #endregion

    #region ���[���A�h���X�ύX
    public void CangeEmail()
    {
        PopupUIManager.Instance.SetupPopup(
            "���[���A�h���X�ύX",
            "�A�J�E���g�ɓo�^����Ă��郁�[���A�h���X����͂��Ă��������B",
            TMP_InputField.ContentType.EmailAddress,
            "�A�J�E���g�ɐݒ肵���p�X���[�h����͂��Ă��������B",
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
            case "����I��":
                PopupUIManager.Instance.SetupPopupMessage(
                    "���O�C������",
                "�A�J�E���g�̃��O�C���ɐ������܂����B\n\n" +
                "�ă��O�C�����܂��B",
                ReLogin);
                break;

            case "�ُ�I��":
                PopupUIManager.Instance.SetupMessageBand("�G���[���������܂����B", 2);
                break;

            case "�l�b�g���[�N�G���[":
                PopupUIManager.Instance.SetupMessageBand(
                    "�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă��������B", 2);
                break;

            default:
                PopupUIManager.Instance.SetupPopupMessage("���O�C�����s", resultMessage, Login);
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
