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
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;
        if (user != null)
        {
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
    /// メールアドレスによる新規データ作成
    /// </summary>
    public async Task<bool> Register(string email, string password)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = result.User;

            // Firestoreに新規データを作成
            if (await FirestoreManager.Instance.SaveNewPlayerData())
                //正常終了
                return true;
            else
                //異常終了
                return false;
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



    /// <summary>
    /// メールアドレスとパスワードでのログイン
    /// </summary>
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



    /// <summary>
    /// ログアウト：匿名ユーザーなら削除、それ以外は通常のログアウト
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
                    Debug.Log("匿名アカウントを削除しました。");
                }
                else
                {
                    auth.SignOut();
                    Debug.Log("通常アカウントでログアウトしました。");
                }
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"アカウント削除エラー: {e.Message}");
            }
        }
        else
        {
            Debug.Log("ログイン中のユーザーがいません。");
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
    /// 匿名アカウントからメールアドレス認証に切り替え（適切な再認証を適用）
    /// </summary>
    public async Task ConvertAnonymousToEmail(string email, string password)
    {
        user = auth.CurrentUser; // 現在の匿名ユーザー
        if (user == null || !user.IsAnonymous)
        {
            Debug.LogError("匿名ユーザーではありません");
            return;
        }
        

        // 最新のユーザーデータを取得
        //await user.ReloadAsync();

        // メールアドレスが未認証なら終了
        //if (!user.IsEmailVerified) return;

        try
        {
            // メール & パスワードの認証情報を作成
            Credential credential = EmailAuthProvider.GetCredential(email, password);

            // 匿名アカウントとメールアカウントをリンク
            await user.LinkWithCredentialAsync(credential);
            Debug.Log("匿名アカウントをメール認証に切り替え成功: " + email);
        }
        catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.RequiresRecentLogin)
        {
            Debug.LogWarning("最近のログインが必要です。匿名アカウントの認証情報で再認証を行います...");

            try
            {
                // 匿名アカウントの認証情報を取得
                Credential anonCredential = GoogleAuthProvider.GetCredential(null, null);

                // 再認証を実行
                await user.ReauthenticateAsync(anonCredential);
                Debug.Log("再認証成功");

                // 再認証後に再試行
                await ConvertAnonymousToEmail(email, password);
            }
            catch (FirebaseException reauthError)
            {
                Debug.LogError("再認証に失敗しました: " + reauthError.Message);
            }
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    Debug.LogError("無効なメールアドレスです");
                    break;
                case (int)AuthError.EmailAlreadyInUse:
                    Debug.LogError("このメールアドレスはすでに使用されています");
                    break;
                case (int)AuthError.WeakPassword:
                    Debug.LogError("パスワードが脆弱です（6文字以上推奨）");
                    break;
                case (int)AuthError.CredentialAlreadyInUse:
                    Debug.LogError("この認証情報はすでに別のアカウントにリンクされています");
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    Debug.LogError("ネットワークエラーが発生しました。接続を確認してください");
                    break;
                default:
                    Debug.LogError($"認証の切り替えに失敗: {e.Message} (エラーコード: {e.ErrorCode})");
                    break;
            }
        }
    }



    /// <summary>
    /// メールアドレス認証（確認メールを送信）
    /// </summary>
    public async Task<bool> SendVerificationEmail(string email)
    {
        user = auth.CurrentUser;

        if (user == null || !user.IsAnonymous)
        {
            Debug.LogError("匿名ユーザーではありません");
            return false;
        }

        // ① メールアドレスのフォーマットチェック
        if (!IsValidEmailFormat(email))
        {
            Debug.LogError("無効なメールアドレス形式です");
            return false;
        }

        // ② メールアドレスの重複チェック
        /*
        if (await IsEmailAlreadyRegistered(email))
        {
            Debug.LogError("このメールアドレスはすでに使用されています");
            return false;
        }
        */

        try
        {
            // ③ メール認証を送信
            //user.SendEmailVerificationBeforeUpdatingEmailAsync
            await user.SendEmailVerificationAsync();
            Debug.Log("確認メールを送信しました。認証を完了するとアカウントが変換されます。");
            return true;
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"確認メール送信エラー: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// メールアドレスのフォーマットチェック
    /// </summary>
    private bool IsValidEmailFormat(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    /// <summary>
    /// メールアドレスが既に登録されているかチェック
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
                return false; // 未登録のメールアドレス
            }
            Debug.LogError($"メールアドレスの確認エラー: {e.Message}");
            return true; // エラー発生時は登録を避ける
        }
    }
    */



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
            else
                //異常終了，エラーコード9を返す
                return 9;
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
            //その他エラーはエラーコード9を返す
            return 9;
        }
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
        return user.IsAnonymous;
    }
}
