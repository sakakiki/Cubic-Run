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
        //自動ログイン
        if (auth.CurrentUser != null)
        {
            Debug.Log("自動ログイン成功: " + auth.CurrentUser.Email);
        }



        /*----開発用----*/
        else if (Application.isEditor)
        {
            SignInAnonymously(); // Unityエディタ用の匿名ログイン
        }
        /*--------------*/
    }

    public async Task Register(string email, string password, string playerName)
    {
        var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        user = result.User;

        //Firestoreに新規データを作成
        await FirestoreManager.Instance.SaveNewPlayerData(playerName);

        Debug.Log("新規登録成功: " + user.Email);
    }

    public async Task Login(string email, string password)
    {
        var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
        user = result.User;
        Debug.Log("ログイン成功: " + user.Email);
    }

    public void Logout()
    {
        auth.SignOut();
        Debug.Log("ログアウトしました");
    }

    private async void SignInAnonymously()
    {
        var result = await auth.SignInAnonymouslyAsync();
        Debug.Log("エディタ用の匿名ログイン成功");
    }
}
