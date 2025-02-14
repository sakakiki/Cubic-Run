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

        // 認証状態の変更を監視
        auth.StateChanged += AuthStateChanged;
    }



    private void OnDestroy()
    {
        // イベントハンドラの解除（メモリリーク防止）
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }



    private void Start()
    {
        // `auth.CurrentUser` のチェックはせず、Firebase の `AuthStateChanged` を待つ
        Debug.Log("AuthManager initialized. Waiting for authentication state...");
    }



    private async void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log($"ログイン済み: {user.Email}");
        }
        else
        {
            Debug.Log("ログインしていません");

            // Unityエディタの場合のみ匿名ログイン
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

            // Firestoreに新規データを作成
            await FirestoreManager.Instance.SaveNewPlayerData(playerName);

            Debug.Log("新規登録成功: " + user.Email);
            return true; // 登録成功
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"登録エラー: {e.Message}");

            // エラーコードごとのメッセージ処理
            switch (e.ErrorCode)
            {
                case (int)AuthError.MissingEmail:
                    Debug.LogError("メールアドレスを入力してください。");
                    break;
                case (int)AuthError.MissingPassword:
                    Debug.LogError("パスワードを入力してください。");
                    break;
                case (int)AuthError.WeakPassword:
                    Debug.LogError("パスワードが短すぎます。より強力なパスワードを設定してください。");
                    break;
                case (int)AuthError.InvalidEmail:
                    Debug.LogError("メールアドレスの形式が正しくありません。");
                    break;
                case (int)AuthError.EmailAlreadyInUse:
                    Debug.LogError("このメールアドレスは既に登録されています。");
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    Debug.LogError("ネットワークエラーが発生しました。インターネット接続を確認してください。");
                    break;
                case (int)AuthError.TooManyRequests:
                    Debug.LogError("試行回数が多すぎます。しばらく待ってから再試行してください。");
                    break;
                default:
                    Debug.LogError("登録に失敗しました。");
                    break;
            }

            return false; // 登録失敗
        }
    }



    public async Task<bool> Login(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = result.User;
            Debug.Log("ログイン成功: " + user.Email);
            return true; // ログイン成功
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"ログインエラー: {e.Message}");

            // エラーコードに応じたメッセージを出力
            switch (e.ErrorCode)
            {
                case (int)AuthError.MissingEmail:
                    Debug.LogError("メールアドレスを入力してください。");
                    break;
                case (int)AuthError.MissingPassword:
                    Debug.LogError("パスワードを入力してください。");
                    break;
                case (int)AuthError.InvalidEmail:
                    Debug.LogError("メールアドレスの形式が正しくありません。");
                    break;
                case (int)AuthError.WrongPassword:
                    Debug.LogError("パスワードが間違っています。");
                    break;
                case (int)AuthError.UserNotFound:
                    Debug.LogError("このメールアドレスのユーザーは登録されていません。");
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    Debug.LogError("ネットワークエラーが発生しました。インターネット接続を確認してください。");
                    break;
                case (int)AuthError.TooManyRequests:
                    Debug.LogError("試行回数が多すぎます。しばらく待ってから再試行してください。");
                    break;
                case (int)AuthError.UserDisabled:
                    Debug.LogError("このアカウントは無効化されています。");
                    break;
                default:
                    Debug.LogError("ログインに失敗しました。");
                    break;
            }

            return false; // ログイン失敗
        }
    }



    public void Logout()
    {
        auth.SignOut();
        Debug.Log("ログアウトしました");
    }



    public async Task UpdateUserEmail(string newEmail)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("ユーザーが認証されていません");
            return;
        }

        try
        {
            await auth.CurrentUser.SendEmailVerificationAsync();
            Debug.Log("確認メールを送信しました: " + newEmail);
        }
        catch (FirebaseException e)
        {
            Debug.LogError("メールアドレスの更新に失敗しました: " + e.Message);
        }
    }




    public async Task UpdatePassword(string newPassword)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("ユーザーが認証されていません");
            return;
        }

        try
        {
            await auth.CurrentUser.UpdatePasswordAsync(newPassword);
            Debug.Log("パスワード変更成功");
        }
        catch (FirebaseException e)
        {
            Debug.LogError("パスワード変更失敗: " + e.Message);
        }
    }



    public async Task SendPasswordResetEmail(string email)
    {
        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            Debug.Log("パスワードリセットメール送信成功: " + email);
        }
        catch (FirebaseException e)
        {
            Debug.LogError("パスワードリセットメール送信失敗: " + e.Message);
        }
    }



    public async Task Reauthenticate(string email, string password)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("ユーザーが認証されていません");
            return;
        }

        try
        {
            var credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
            await auth.CurrentUser.ReauthenticateAsync(credential);
            Debug.Log("再認証成功");
        }
        catch (FirebaseException e)
        {
            Debug.LogError("再認証失敗: " + e.Message);
        }
    }



    private async Task SignInAnonymously()
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            Debug.Log("エディタ用の匿名ログイン成功");
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"匿名ログインエラー: {e.Message}");
        }
    }

}
