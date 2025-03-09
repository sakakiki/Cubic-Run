using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private FirebaseAuth auth;
    private FirebaseUser user;

    //�O������̏�Ԋm�F�p
    public enum LoginState
    {
        Unchecked,
        Login,
        NoAccount
    }
    public LoginState loginState {  get; private set; }



    /// <summary>
    /// �N����̏����ݒ�
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        auth = FirebaseAuth.DefaultInstance;

        // �F�؏�Ԃ̕ύX���Ď�
        auth.StateChanged += AuthStateChanged;

        //���`�F�b�N��Ԃŏ�����
        loginState = LoginState.Unchecked;


        //auth.SignOut();

    }



    /// <summary>
    /// �C�x���g�n���h���̉����i���������[�N�h�~�j
    /// </summary>
    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }



    /// <summary>
    /// �F�؏�񂪎擾�ł���Ύ������O�C�������s
    /// </summary>
    private async void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;


        if (user != null)
        {
            // �ŐV�̃��[�U�[�f�[�^���擾
            await UpdateUserData();

            Debug.Log("���O�C���ς݁@�@�A�h���X�o�^�F " + !string.IsNullOrEmpty(user.Email) + "�@�F�؁F" + user.IsEmailVerified);
            loginState = LoginState.Login;
        }
        else
        {
            Debug.Log("���O�C�����Ă��܂���");
            loginState = LoginState.NoAccount;
        }
    }



    /// <summary>
    /// �������O�C��
    /// </summary>
    public async Task<int> SignInAnonymously()
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            Debug.Log("�������O�C������");

            //�O������̊m�F�p
            loginState = LoginState.Login;

            // Firestore�ɐV�K�f�[�^���쐬
            if (await FirestoreManager.Instance.SaveNewPlayerData())
                //����I��
                return 0;
        }
        catch (FirebaseException e)
        {
            //�l�b�g���[�N�G���[�̓G���[�R�[�h1��Ԃ�
            if (e.ErrorCode == (int)AuthError.NetworkRequestFailed)
                return 1;

            //���̑��G���[�̓G���[�R�[�h9��Ԃ�
            else return 9;
        }
        catch (Exception ex)
        {

        }
        //���̑��G���[�̓G���[�R�[�h9��Ԃ�
        return 9;
    }



    /// <summary>
    /// �����A�J�E���g���烁�[���A�h���X�F�؂ɐ؂�ւ�
    /// </summary>
    public async Task<string> ConvertAnonymousToEmail(string email, string password)
    {
        user = auth.CurrentUser; // ���݂̓������[�U�[

        if (user == null || !user.IsAnonymous)
        {
            //�������[�U�[�łȂ�
            return "�ُ�I��";
        }

        try
        {
            // ���[�� & �p�X���[�h�̔F�؏����쐬
            Credential credential = EmailAuthProvider.GetCredential(email, password);

            // �����A�J�E���g�ƃ��[���A�J�E���g�������N
            await user.LinkWithCredentialAsync(credential);
            Debug.Log("�����A�J�E���g�����[���F�؂ɐ؂�ւ�����: " + email);

            // �F�؃��[�����M
            await user.SendEmailVerificationAsync();

            return "����I��";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    return "�����ȃ��[���A�h���X�ł�";

                case (int)AuthError.EmailAlreadyInUse:
                    return "���̃��[���A�h���X�͂��łɎg�p����Ă��܂�";

                case (int)AuthError.WeakPassword:
                    return
                        "�p�X���[�h���Ǝ�ł�\n\n" +
                        "�p�X���[�h��6�����ȏ�ŁA�p�������܂߂Ă��������B\n" +
                        "8�����ȏ�A�p���E�����E�L����g�ݍ��킹�邱�Ƃň��S�������サ�܂��B";

                case (int)AuthError.CredentialAlreadyInUse:
                    return "���̔F�؏��͂��łɕʂ̃A�J�E���g�Ƀ����N����Ă��܂�";

                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception ex)
        {
            return "�ُ�I��";
        }
    }



    /// <summary>
    /// ���[���A�h���X�ƃp�X���[�h�ł̃��O�C��
    /// ���O�C���O�������A�J�E���g�������ꍇ�̓A�J�E���g�폜
    /// </summary>
    public async Task<string> Login(string email, string password)
    {
        try
        {
            //���O�C���O�������A�J�E���g�Ȃ�폜
            if (user.IsAnonymous)
            {
                await FirestoreManager.Instance.DeleteDocument(user.UserId);
                await user.DeleteAsync();
                Debug.Log("�����A�J�E���g���폜���܂����B");
                await UpdateUserData();
            }

            //���[���A�h���X�ƃp�X���[�h�Ń��O�C��
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            //�A�J�E���g���X�V
            user = result.User;

            return "����I��";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.MissingEmail:
                    return"���[���A�h���X����͂��Ă��������B";

                case (int)AuthError.MissingPassword:
                    return"�p�X���[�h����͂��Ă��������B";

                case (int)AuthError.InvalidEmail:
                    return"���[���A�h���X�̌`��������������܂���B";

                case (int)AuthError.WrongPassword:
                    return"�p�X���[�h���Ԉ���Ă��܂��B";

                case (int)AuthError.UserNotFound:
                    return"���̃��[���A�h���X�̃��[�U�[�͓o�^����Ă��܂���B";

                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                case (int)AuthError.TooManyRequests:
                    return"���s�񐔂��������܂��B���΂炭�҂��Ă���Ď��s���Ă��������B";

                case (int)AuthError.UserDisabled:
                    return"���̃A�J�E���g�͖���������Ă��܂��B";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception ex)
        {
            return "�ُ�I��";
        }
    }



    /// <summary>
    /// �ĔF��
    /// </summary>
    public async Task<string> Reauthenticate(string email, string password)
    {
        if (auth.CurrentUser == null)
        {
            return "���[�U�[���F�؂���Ă��܂���B";
        }

        try
        {
            var credential = EmailAuthProvider.GetCredential(email, password);
            await auth.CurrentUser.ReauthenticateAsync(credential);
            return "����I��";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    return "�����ȃ��[���A�h���X�ł��B";

                case (int)AuthError.WrongPassword:
                    return "�p�X���[�h���Ԉ���Ă��܂��B";

                case (int)AuthError.UserNotFound:
                    return "���̃��[���A�h���X�̃A�J�E���g�͑��݂��܂���B";

                case (int)AuthError.UserDisabled:
                    return "���̃A�J�E���g�͖���������Ă��܂��B";

                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception e)
        {
            return "�ُ�I��";
        }
    }



    /// <summary>
    /// ���[���A�h���X�ύX
    /// </summary>
    public async Task<string> UpdateUserEmail(string newEmail)
    {
        if (auth.CurrentUser == null)
        {
            return "���[�U�[���F�؂���Ă��܂���B";
        }

        try
        {
            await auth.CurrentUser.SendEmailVerificationBeforeUpdatingEmailAsync(newEmail);
            return "����I��";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    return "�����ȃ��[���A�h���X�ł��B";

                case (int)AuthError.EmailAlreadyInUse:
                    return "���̃��[���A�h���X�͂��łɎg�p����Ă��܂��B";

                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception e)
        {
            return "�ُ�I��";
        }
    }



    /// <summary>
    /// �p�X���[�h�ύX
    /// </summary>
    public async Task<string> UpdatePassword(string newPassword)
    {
        if (auth.CurrentUser == null)
        {
            return "���[�U�[���F�؂���Ă��܂���B";
        }

        try
        {
            await auth.CurrentUser.UpdatePasswordAsync(newPassword);
            return "����I��";
        }

        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.WeakPassword:
                    return
                        "�p�X���[�h���Ǝ�ł�\n\n" +
                        "�p�X���[�h��6�����ȏ�ŁA�p�������܂߂Ă��������B\n" +
                        "8�����ȏ�A�p���E�����E�L����g�ݍ��킹�邱�Ƃň��S�������サ�܂��B";

                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception ex)
        {
            return "�ُ�I��";
        }
    }



    /// <summary>
    /// �p�X���[�h�̃��Z�b�g
    /// </summary>
    public async Task<string> SendPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return "���[���A�h���X��񂪌�����܂���ł����B";
        }

        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            return "����I��";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.TooManyRequests:
                    return "�Z���ԂɃ��N�G�X�g���������܂��B���΂炭�҂��Ă���Ď��s���Ă��������B";

                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception e)
        {
            return "�ُ�I��";
        }
    }



    /// <summary>
    /// �F�؃��[���̍đ�
    /// </summary>
    public async Task<string> SendEmailVerification()
    {
        try
        {
            await user.SendEmailVerificationAsync();
            return "����I��";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.NetworkRequestFailed:
                    return "�l�b�g���[�N�G���[";

                default:
                    return "�ُ�I��";
            }
        }
        catch (Exception e)
        {
            return "�ُ�I��";
        }
    }





    /// <summary>
    /// ���O�C����Ԃ��ǂ������擾
    /// </summary>
    public LoginState GetLoginState()
    {
        return loginState;
    }



    /// <summary>
    /// �������O�C�����ǂ������擾
    /// </summary>
    public bool GetIsAnonymous()
    {
        user = auth.CurrentUser;
        return user.IsAnonymous;
    }



    /// <summary>
    /// ���[���A�h���X���F�؍ς��ǂ������擾
    /// </summary>
    public bool GetIsEmailVerified()
    {
        user = auth.CurrentUser;
        return user.IsEmailVerified;
    }



    /// <summary>
    /// ���[���A�h���X���擾
    /// </summary>
    public string GetEmail()
    {
        try
        {
            user = auth.CurrentUser;
            string result = user.Email;
            return result; // ����I��
        }
        catch (Exception ex)
        {
            return "���ݒ�"; // �ُ�I��
        }
    }



    /// <summary>
    /// �ŐV�̃��[�U�[�f�[�^���擾
    /// </summary>
    public async Task<bool> UpdateUserData()
    {
        try
        {
            await user.ReloadAsync();
            return true; // ����I��
        }
        catch (Exception ex)
        {
            return false; // �ُ�I��
        }
    }
}
