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

    //外部からの状態確認用
    public enum LoginState
    {
        Unchecked,
        Login,
        NoAccount
    }
    public LoginState loginState {  get; private set; }



    /// <summary>
    /// 起動後の初期設定
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        auth = FirebaseAuth.DefaultInstance;

        // 認証状態の変更を監視
        auth.StateChanged += AuthStateChanged;

        //未チェック状態で初期化
        loginState = LoginState.Unchecked;


        //auth.SignOut();

    }



    /// <summary>
    /// イベントハンドラの解除（メモリリーク防止）
    /// </summary>
    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }



    /// <summary>
    /// 認証情報が取得できれば自動ログインを実行
    /// </summary>
    private async void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;


        if (user != null)
        {

            // 最新のユーザーデータを取得
            await UpdateUserData();

            Debug.Log("ログイン済み: " + user.Email + ",Email:" + user.IsEmailVerified);
            loginState = LoginState.Login;
        }
        else
        {
            Debug.Log("ログインしていません");
            loginState = LoginState.NoAccount;
        }
    }



    /// <summary>
    /// メールアドレスとパスワードでのログイン
    /// </summary>
    public async Task<string> Login(string email, string password)
    {
        try
        {
            //ログイン前のアカウントを記憶
            FirebaseUser oldUser = auth.CurrentUser;

            //メールアドレスとパスワードでログイン
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            //アカウント情報更新
            user = result.User;
            Debug.Log("更新完了");

            //ログイン前が匿名アカウントなら削除
            if (oldUser.IsAnonymous)
            {
                //await oldUser.DeleteAsync();
                Debug.Log("匿名アカウントを削除しました。");
            }

            return "正常終了";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.MissingEmail:
                    return"メールアドレスを入力してください。";

                case (int)AuthError.MissingPassword:
                    return"パスワードを入力してください。";

                case (int)AuthError.InvalidEmail:
                    return"メールアドレスの形式が正しくありません。";

                case (int)AuthError.WrongPassword:
                    return"パスワードが間違っています。";

                case (int)AuthError.UserNotFound:
                    return"このメールアドレスのユーザーは登録されていません。";

                case (int)AuthError.NetworkRequestFailed:
                    return "ネットワークエラー";

                case (int)AuthError.TooManyRequests:
                    return"試行回数が多すぎます。しばらく待ってから再試行してください。";

                case (int)AuthError.UserDisabled:
                    return"このアカウントは無効化されています。";

                default:
                    return "異常終了";
            }
        }
        catch (Exception ex)
        {
            return "異常終了";
        }
    }



    /// <summary>
    /// メールアドレス変更
    /// </summary>
    public async Task UpdateUserEmail(string newEmail)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("ユーザーが認証されていません");
            return;
        }

        try
        {
            await auth.CurrentUser.SendEmailVerificationBeforeUpdatingEmailAsync(newEmail);
            Debug.Log("確認メールを送信しました: " + newEmail);
        }
        catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.RequiresRecentLogin)
        {
            Debug.LogWarning("最近のログインが必要です。匿名アカウントの認証情報で再認証を行います...");

            try
            {
                // 匿名アカウントの認証情報を取得
                Credential anonCredential = GoogleAuthProvider.GetCredential(user.Email, null);

                // 再認証を実行
                await user.ReauthenticateAsync(anonCredential);
                Debug.Log("再認証成功");

                // 再認証後に再試行
                await UpdateUserEmail(newEmail);
            }
            catch (FirebaseException reauthError)
            {
                Debug.LogError("再認証に失敗しました: " + reauthError.Message);
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError("メールアドレスの更新に失敗しました: " + e.Message);
        }
    }



    /// <summary>
    /// パスワード変更
    /// </summary>
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



    /// <summary>
    /// パスワードのリセット
    /// </summary>
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



    /// <summary>
    /// 再認証
    /// </summary>
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



    /// <summary>
    /// 匿名アカウントからメールアドレス認証に切り替え
    /// </summary>
    public async Task<string> ConvertAnonymousToEmail(string email, string password)
    {
        user = auth.CurrentUser; // 現在の匿名ユーザー

        if (user == null || !user.IsAnonymous)
        {
            //匿名ユーザーでない
            return "異常終了";
        }

        try
        {
            // メール & パスワードの認証情報を作成
            Credential credential = EmailAuthProvider.GetCredential(email, password);

            // 匿名アカウントとメールアカウントをリンク
            await user.LinkWithCredentialAsync(credential);
            Debug.Log("匿名アカウントをメール認証に切り替え成功: " + email);

            // 認証メール送信
            await user.SendEmailVerificationAsync();

            return "正常終了";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    return "無効なメールアドレスです";

                case (int)AuthError.EmailAlreadyInUse:
                    return "このメールアドレスはすでに使用されています";

                case (int)AuthError.WeakPassword:
                    return
                        "パスワードが脆弱です\n\n" +
                        "パスワードは6文字以上で、英数字を含めてください。\n" +
                        "8文字以上、英字・数字・記号を組み合わせることで安全性が向上します。";

                case (int)AuthError.CredentialAlreadyInUse:
                    return "この認証情報はすでに別のアカウントにリンクされています";

                case (int)AuthError.NetworkRequestFailed:
                    return "ネットワークエラー";

                default:
                    return "異常終了";
            }
        }
        catch (Exception ex)
        {
            return "異常終了";
        }
    }



    /// <summary>
    /// 匿名ログイン
    /// </summary>
    public async Task<int> SignInAnonymously()
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            Debug.Log("匿名ログイン成功"); 
            
            //外部からの確認用
            loginState = LoginState.Login;

            // Firestoreに新規データを作成
            if (await FirestoreManager.Instance.SaveNewPlayerData())
                //正常終了
                return 0;
        }
        catch (FirebaseException e)
        {
            //ネットワークエラーはエラーコード1を返す
            if (e.ErrorCode == (int)AuthError.NetworkRequestFailed)
                return 1;

            //その他エラーはエラーコード9を返す
            else return 9;
        }
        catch (Exception ex)
        {

        }
        //その他エラーはエラーコード9を返す
        return 9;
    }




    /// <summary>
    /// ログイン状態かどうかを取得
    /// </summary>
    public LoginState GetLoginState()
    {
        return loginState;
    }



    /// <summary>
    /// 匿名ログインかどうかを取得
    /// </summary>
    public bool GetIsAnonymous()
    {
        user = auth.CurrentUser;
        return user.IsAnonymous;
    }



    /// <summary>
    /// メールアドレスが認証済かどうかを取得
    /// </summary>
    public bool GetIsEmailVerified()
    {
        user = auth.CurrentUser;
        return user.IsEmailVerified;
    }



    /// <summary>
    /// メールアドレスを取得
    /// </summary>
    public string GetEmail()
    {
        user = auth.CurrentUser;
        return user.Email;
    }



    /// <summary>
    /// 最新のユーザーデータを取得
    /// </summary>
    public async Task<bool> UpdateUserData()
    {
        try
        {
            await user.ReloadAsync();
            return true; // 正常終了
        }
        catch (Exception ex)
        {
            return false; // 異常終了
        }
    }
}
