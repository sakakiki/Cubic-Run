using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private FirebaseAuth auth;
    private FirebaseUser user;



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
            Debug.Log($"���O�C���ς�: {user.Email}");
        }
        else
        {
            Debug.Log("���O�C�����Ă��܂���");
        }
    }



    /// <summary>
    /// ���[���A�h���X�ɂ��V�K�f�[�^�쐬
    /// </summary>
    public async Task<bool> Register(string email, string password, string playerName)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = result.User;

            // Firestore�ɐV�K�f�[�^���쐬
            await FirestoreManager.Instance.SaveNewPlayerData(playerName);

            Debug.Log("�V�K�o�^����: " + user.Email);
            return true; // �o�^����
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
    /// ���O�A�E�g
    /// </summary>
    public void Logout()
    {
        auth.SignOut();
        Debug.Log("���O�A�E�g���܂���");
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
            await auth.CurrentUser.SendEmailVerificationAsync();
            Debug.Log("�m�F���[���𑗐M���܂���: " + newEmail);
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

        try
        {
            // ���[�� & �p�X���[�h�̔F�؏����쐬
            Credential credential = EmailAuthProvider.GetCredential(email, password);

            // �����A�J�E���g�ƃ��[���A�J�E���g�������N
            await user.LinkWithCredentialAsync(credential);
            Debug.Log("�����A�J�E���g�����[���F�؂ɐ؂�ւ�����: " + email);

            // ���[���F�؂𑗐M
            await user.SendEmailVerificationAsync();
            Debug.Log("�m�F���[���𑗐M���܂���");
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
    /// �������O�C��
    /// </summary>
    private async Task SignInAnonymously()
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            Debug.Log("�������O�C������");
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"�������O�C���G���[: {e.Message}");
        }
    }



    /// <summary>
    /// ���O�C����Ԃ��ǂ������擾
    /// </summary>
    public bool GetIsLogin()
    {
        return user != null;
    }
}
