using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;

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
    }




    public void Start()
    {
        //�������O�C��
        if (auth.CurrentUser != null)
        {
            Debug.Log("�������O�C������: " + auth.CurrentUser.Email);
        }



        /*----�J���p----*/
        else if (Application.isEditor)
        {
            SignInAnonymously(); // Unity�G�f�B�^�p�̓������O�C��
        }
        /*--------------*/
    }

    public async Task Register(string email, string password, string playerName)
    {
        var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        user = result.User;

        //Firestore�ɐV�K�f�[�^���쐬
        await FirestoreManager.Instance.SaveNewPlayerData(playerName);

        Debug.Log("�V�K�o�^����: " + user.Email);
    }

    public async Task Login(string email, string password)
    {
        var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
        user = result.User;
        Debug.Log("���O�C������: " + user.Email);
    }

    public void Logout()
    {
        auth.SignOut();
        Debug.Log("���O�A�E�g���܂���");
    }

    private async void SignInAnonymously()
    {
        var result = await auth.SignInAnonymouslyAsync();
        Debug.Log("�G�f�B�^�p�̓������O�C������");
    }
}
