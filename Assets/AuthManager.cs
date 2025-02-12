using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private FirebaseAuth auth;
    private FirebaseUser user;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        auth = FirebaseAuth.DefaultInstance;

        // �F�؏�Ԃ̕ύX���Ď�
        auth.StateChanged += AuthStateChanged;
    }



    private void OnDestroy()
    {
        // �C�x���g�n���h���̉����i���������[�N�h�~�j
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }



    private void Start()
    {
        // `auth.CurrentUser` �̃`�F�b�N�͂����AFirebase �� `AuthStateChanged` ��҂�
        Debug.Log("AuthManager initialized. Waiting for authentication state...");
    }



    private async void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log($"���O�C���ς�: {user.Email}");
        }
        else
        {
            Debug.Log("���O�C�����Ă��܂���");

            // Unity�G�f�B�^�̏ꍇ�̂ݓ������O�C��
            if (Application.isEditor)
            {
                await SignInAnonymously();
            }
        }
    }



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



    public void Logout()
    {
        auth.SignOut();
        Debug.Log("���O�A�E�g���܂���");
    }



    private async Task SignInAnonymously()
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            Debug.Log("�G�f�B�^�p�̓������O�C������");
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"�������O�C���G���[: {e.Message}");
        }
    }

}
