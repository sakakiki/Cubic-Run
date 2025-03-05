using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using System;
using System.Text.RegularExpressions;

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
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log("���O�C���ς�: " + user.Email + ",Email:" + user.IsEmailVerified);
            loginState = LoginState.Login;
        }
        else
        {
            Debug.Log("���O�C�����Ă��܂���");
            loginState = LoginState.NoAccount;
        }
    }



    /// <summary>
    /// ���[���A�h���X�ɂ��V�K�f�[�^�쐬
    /// </summary>
    public async Task<bool> Register(string email, string password)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = result.User;

            // Firestore�ɐV�K�f�[�^���쐬
            if (await FirestoreManager.Instance.SaveNewPlayerData())
                //����I��
                return true;
            else
                //�ُ�I��
                return false;
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"�o�^�G���[: {e.Message}");

            // �G���[�R�[�h���Ƃ̃��b�Z�[�W����
            switch (e.ErrorCode)
            {
                case (int)AuthError.MissingEmail:
                    Debug.LogError("���[���A�h���X����͂��Ă��������B");
                    break;
                case (int)AuthError.MissingPassword:
                    Debug.LogError("�p�X���[�h����͂��Ă��������B");
                    break;
                case (int)AuthError.WeakPassword:
                    Debug.LogError("�p�X���[�h���Z�����܂��B��苭�͂ȃp�X���[�h��ݒ肵�Ă��������B");
                    break;
                case (int)AuthError.InvalidEmail:
                    Debug.LogError("���[���A�h���X�̌`��������������܂���B");
                    break;
                case (int)AuthError.EmailAlreadyInUse:
                    Debug.LogError("���̃��[���A�h���X�͊��ɓo�^����Ă��܂��B");
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    Debug.LogError("�l�b�g���[�N�G���[���������܂����B�C���^�[�l�b�g�ڑ����m�F���Ă��������B");
                    break;
                case (int)AuthError.TooManyRequests:
                    Debug.LogError("���s�񐔂��������܂��B���΂炭�҂��Ă���Ď��s���Ă��������B");
                    break;
                default:
                    Debug.LogError("�o�^�Ɏ��s���܂����B");
                    break;
            }

            return false; // �o�^���s
        }
    }



    /// <summary>
    /// ���[���A�h���X�ƃp�X���[�h�ł̃��O�C��
    /// </summary>
    public async Task<bool> Login(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = result.User;
            Debug.Log("���O�C������: " + user.Email);
            return true; // ���O�C������
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"���O�C���G���[: {e.Message}");

            // �G���[�R�[�h�ɉ��������b�Z�[�W���o��
            switch (e.ErrorCode)
            {
                case (int)AuthError.MissingEmail:
                    Debug.LogError("���[���A�h���X����͂��Ă��������B");
                    break;
                case (int)AuthError.MissingPassword:
                    Debug.LogError("�p�X���[�h����͂��Ă��������B");
                    break;
                case (int)AuthError.InvalidEmail:
                    Debug.LogError("���[���A�h���X�̌`��������������܂���B");
                    break;
                case (int)AuthError.WrongPassword:
                    Debug.LogError("�p�X���[�h���Ԉ���Ă��܂��B");
                    break;
                case (int)AuthError.UserNotFound:
                    Debug.LogError("���̃��[���A�h���X�̃��[�U�[�͓o�^����Ă��܂���B");
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    Debug.LogError("�l�b�g���[�N�G���[���������܂����B�C���^�[�l�b�g�ڑ����m�F���Ă��������B");
                    break;
                case (int)AuthError.TooManyRequests:
                    Debug.LogError("���s�񐔂��������܂��B���΂炭�҂��Ă���Ď��s���Ă��������B");
                    break;
                case (int)AuthError.UserDisabled:
                    Debug.LogError("���̃A�J�E���g�͖���������Ă��܂��B");
                    break;
                default:
                    Debug.LogError("���O�C���Ɏ��s���܂����B");
                    break;
            }

            return false; // ���O�C�����s
        }
    }



    /// <summary>
    /// ���O�A�E�g�F�������[�U�[�Ȃ�폜�A����ȊO�͒ʏ�̃��O�A�E�g
    /// </summary>
    public async void Logout()
    {
        FirebaseUser currentUser = auth.CurrentUser;

        if (currentUser != null)
        {
            try
            {
                if (currentUser.IsAnonymous)
                {
                    await currentUser.DeleteAsync();
                    Debug.Log("�����A�J�E���g���폜���܂����B");
                }
                else
                {
                    auth.SignOut();
                    Debug.Log("�ʏ�A�J�E���g�Ń��O�A�E�g���܂����B");
                }
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"�A�J�E���g�폜�G���[: {e.Message}");
            }
        }
        else
        {
            Debug.Log("���O�C�����̃��[�U�[�����܂���B");
        }
    }



    /// <summary>
    /// ���[���A�h���X�ύX
    /// </summary>
    public async Task UpdateUserEmail(string newEmail)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("���[�U�[���F�؂���Ă��܂���");
            return;
        }

        try
        {
            await auth.CurrentUser.SendEmailVerificationBeforeUpdatingEmailAsync(newEmail);
            Debug.Log("�m�F���[���𑗐M���܂���: " + newEmail);
        }
        catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.RequiresRecentLogin)
        {
            Debug.LogWarning("�ŋ߂̃��O�C�����K�v�ł��B�����A�J�E���g�̔F�؏��ōĔF�؂��s���܂�...");

            try
            {
                // �����A�J�E���g�̔F�؏����擾
                Credential anonCredential = GoogleAuthProvider.GetCredential(user.Email, null);

                // �ĔF�؂����s
                await user.ReauthenticateAsync(anonCredential);
                Debug.Log("�ĔF�ؐ���");

                // �ĔF�،�ɍĎ��s
                await UpdateUserEmail(newEmail);
            }
            catch (FirebaseException reauthError)
            {
                Debug.LogError("�ĔF�؂Ɏ��s���܂���: " + reauthError.Message);
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError("���[���A�h���X�̍X�V�Ɏ��s���܂���: " + e.Message);
        }
    }



    /// <summary>
    /// �p�X���[�h�ύX
    /// </summary>
    public async Task UpdatePassword(string newPassword)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("���[�U�[���F�؂���Ă��܂���");
            return;
        }

        try
        {
            await auth.CurrentUser.UpdatePasswordAsync(newPassword);
            Debug.Log("�p�X���[�h�ύX����");
        }
        catch (FirebaseException e)
        {
            Debug.LogError("�p�X���[�h�ύX���s: " + e.Message);
        }
    }



    /// <summary>
    /// �p�X���[�h�̃��Z�b�g
    /// </summary>
    public async Task SendPasswordResetEmail(string email)
    {
        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            Debug.Log("�p�X���[�h���Z�b�g���[�����M����: " + email);
        }
        catch (FirebaseException e)
        {
            Debug.LogError("�p�X���[�h���Z�b�g���[�����M���s: " + e.Message);
        }
    }



    /// <summary>
    /// �ĔF��
    /// </summary>
    public async Task Reauthenticate(string email, string password)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("���[�U�[���F�؂���Ă��܂���");
            return;
        }

        try
        {
            var credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
            await auth.CurrentUser.ReauthenticateAsync(credential);
            Debug.Log("�ĔF�ؐ���");
        }
        catch (FirebaseException e)
        {
            Debug.LogError("�ĔF�؎��s: " + e.Message);
        }
    }



    /// <summary>
    /// �����A�J�E���g���烁�[���A�h���X�F�؂ɐ؂�ւ��i�K�؂ȍĔF�؂�K�p�j
    /// </summary>
    public async Task ConvertAnonymousToEmail(string email, string password)
    {
        user = auth.CurrentUser; // ���݂̓������[�U�[
        if (user == null || !user.IsAnonymous)
        {
            Debug.LogError("�������[�U�[�ł͂���܂���");
            return;
        }
        

        // �ŐV�̃��[�U�[�f�[�^���擾
        //await user.ReloadAsync();

        // ���[���A�h���X�����F�؂Ȃ�I��
        //if (!user.IsEmailVerified) return;

        try
        {
            // ���[�� & �p�X���[�h�̔F�؏����쐬
            Credential credential = EmailAuthProvider.GetCredential(email, password);

            // �����A�J�E���g�ƃ��[���A�J�E���g�������N
            await user.LinkWithCredentialAsync(credential);
            Debug.Log("�����A�J�E���g�����[���F�؂ɐ؂�ւ�����: " + email);
        }
        catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.RequiresRecentLogin)
        {
            Debug.LogWarning("�ŋ߂̃��O�C�����K�v�ł��B�����A�J�E���g�̔F�؏��ōĔF�؂��s���܂�...");

            try
            {
                // �����A�J�E���g�̔F�؏����擾
                Credential anonCredential = GoogleAuthProvider.GetCredential(null, null);

                // �ĔF�؂����s
                await user.ReauthenticateAsync(anonCredential);
                Debug.Log("�ĔF�ؐ���");

                // �ĔF�،�ɍĎ��s
                await ConvertAnonymousToEmail(email, password);
            }
            catch (FirebaseException reauthError)
            {
                Debug.LogError("�ĔF�؂Ɏ��s���܂���: " + reauthError.Message);
            }
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    Debug.LogError("�����ȃ��[���A�h���X�ł�");
                    break;
                case (int)AuthError.EmailAlreadyInUse:
                    Debug.LogError("���̃��[���A�h���X�͂��łɎg�p����Ă��܂�");
                    break;
                case (int)AuthError.WeakPassword:
                    Debug.LogError("�p�X���[�h���Ǝ�ł��i6�����ȏ㐄���j");
                    break;
                case (int)AuthError.CredentialAlreadyInUse:
                    Debug.LogError("���̔F�؏��͂��łɕʂ̃A�J�E���g�Ƀ����N����Ă��܂�");
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    Debug.LogError("�l�b�g���[�N�G���[���������܂����B�ڑ����m�F���Ă�������");
                    break;
                default:
                    Debug.LogError($"�F�؂̐؂�ւ��Ɏ��s: {e.Message} (�G���[�R�[�h: {e.ErrorCode})");
                    break;
            }
        }
    }



    /// <summary>
    /// ���[���A�h���X�F�؁i�m�F���[���𑗐M�j
    /// </summary>
    public async Task<bool> SendVerificationEmail(string email)
    {
        user = auth.CurrentUser;

        if (user == null || !user.IsAnonymous)
        {
            Debug.LogError("�������[�U�[�ł͂���܂���");
            return false;
        }

        // �@ ���[���A�h���X�̃t�H�[�}�b�g�`�F�b�N
        if (!IsValidEmailFormat(email))
        {
            Debug.LogError("�����ȃ��[���A�h���X�`���ł�");
            return false;
        }

        // �A ���[���A�h���X�̏d���`�F�b�N
        /*
        if (await IsEmailAlreadyRegistered(email))
        {
            Debug.LogError("���̃��[���A�h���X�͂��łɎg�p����Ă��܂�");
            return false;
        }
        */

        try
        {
            // �B ���[���F�؂𑗐M
            //user.SendEmailVerificationBeforeUpdatingEmailAsync
            await user.SendEmailVerificationAsync();
            Debug.Log("�m�F���[���𑗐M���܂����B�F�؂���������ƃA�J�E���g���ϊ�����܂��B");
            return true;
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"�m�F���[�����M�G���[: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ���[���A�h���X�̃t�H�[�}�b�g�`�F�b�N
    /// </summary>
    private bool IsValidEmailFormat(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    /// <summary>
    /// ���[���A�h���X�����ɓo�^����Ă��邩�`�F�b�N
    /// </summary>
    /*
    private async Task<bool> IsEmailAlreadyRegistered(string email)
    {
        try
        {
            var signInMethods = await auth.(email);
            return signInMethods != null && signInMethods.Count > 0;
        }
        catch (FirebaseException e)
        {
            if (e.ErrorCode == AuthErrorCode.UserNotFound)
            {
                return false; // ���o�^�̃��[���A�h���X
            }
            Debug.LogError($"���[���A�h���X�̊m�F�G���[: {e.Message}");
            return true; // �G���[�������͓o�^�������
        }
    }
    */



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
            else
                //�ُ�I���C�G���[�R�[�h9��Ԃ�
                return 9;
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
            //���̑��G���[�̓G���[�R�[�h9��Ԃ�
            return 9;
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
        return user.IsAnonymous;
    }
}
