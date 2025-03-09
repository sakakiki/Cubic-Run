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

            Debug.Log("ログイン済み　　アドレス登録： " + !string.IsNullOrEmpty(user.Email) + "　認証：" + user.IsEmailVerified);
            loginState = LoginState.Login;
        }
        else
        {
            Debug.Log("ログインしていません");
            loginState = LoginState.NoAccount;
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
    /// メールアドレスとパスワードでのログイン
    /// ログイン前が匿名アカウントだった場合はアカウント削除
    /// </summary>
    public async Task<string> Login(string email, string password)
    {
        try
        {
            //ログイン前が匿名アカウントなら削除
            if (user.IsAnonymous)
            {
                await FirestoreManager.Instance.DeleteDocument(user.UserId);
                await user.DeleteAsync();
                Debug.Log("匿名アカウントを削除しました。");
                await UpdateUserData();
            }

            //メールアドレスとパスワードでログイン
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            //アカウント情報更新
            user = result.User;

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
    /// 再認証
    /// </summary>
    public async Task<string> Reauthenticate(string email, string password)
    {
        if (auth.CurrentUser == null)
        {
            return "ユーザーが認証されていません。";
        }

        try
        {
            var credential = EmailAuthProvider.GetCredential(email, password);
            await auth.CurrentUser.ReauthenticateAsync(credential);
            return "正常終了";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    return "無効なメールアドレスです。";

                case (int)AuthError.WrongPassword:
                    return "パスワードが間違っています。";

                case (int)AuthError.UserNotFound:
                    return "このメールアドレスのアカウントは存在しません。";

                case (int)AuthError.UserDisabled:
                    return "このアカウントは無効化されています。";

                case (int)AuthError.NetworkRequestFailed:
                    return "ネットワークエラー";

                default:
                    return "異常終了";
            }
        }
        catch (Exception e)
        {
            return "異常終了";
        }
    }



    /// <summary>
    /// メールアドレス変更
    /// </summary>
    public async Task<string> UpdateUserEmail(string newEmail)
    {
        if (auth.CurrentUser == null)
        {
            return "ユーザーが認証されていません。";
        }

        try
        {
            await auth.CurrentUser.SendEmailVerificationBeforeUpdatingEmailAsync(newEmail);
            return "正常終了";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.InvalidEmail:
                    return "無効なメールアドレスです。";

                case (int)AuthError.EmailAlreadyInUse:
                    return "このメールアドレスはすでに使用されています。";

                case (int)AuthError.NetworkRequestFailed:
                    return "ネットワークエラー";

                default:
                    return "異常終了";
            }
        }
        catch (Exception e)
        {
            return "異常終了";
        }
    }



    /// <summary>
    /// パスワード変更
    /// </summary>
    public async Task<string> UpdatePassword(string newPassword)
    {
        if (auth.CurrentUser == null)
        {
            return "ユーザーが認証されていません。";
        }

        try
        {
            await auth.CurrentUser.UpdatePasswordAsync(newPassword);
            return "正常終了";
        }

        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.WeakPassword:
                    return
                        "パスワードが脆弱です\n\n" +
                        "パスワードは6文字以上で、英数字を含めてください。\n" +
                        "8文字以上、英字・数字・記号を組み合わせることで安全性が向上します。";

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
    /// パスワードのリセット
    /// </summary>
    public async Task<string> SendPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return "メールアドレス情報が見つかりませんでした。";
        }

        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            return "正常終了";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.TooManyRequests:
                    return "短時間にリクエストが多すぎます。しばらく待ってから再試行してください。";

                case (int)AuthError.NetworkRequestFailed:
                    return "ネットワークエラー";

                default:
                    return "異常終了";
            }
        }
        catch (Exception e)
        {
            return "異常終了";
        }
    }



    /// <summary>
    /// 認証メールの再送
    /// </summary>
    public async Task<string> SendEmailVerification()
    {
        try
        {
            await user.SendEmailVerificationAsync();
            return "正常終了";
        }
        catch (FirebaseException e)
        {
            switch (e.ErrorCode)
            {
                case (int)AuthError.NetworkRequestFailed:
                    return "ネットワークエラー";

                default:
                    return "異常終了";
            }
        }
        catch (Exception e)
        {
            return "異常終了";
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
        try
        {
            user = auth.CurrentUser;
            string result = user.Email;
            return result; // 正常終了
        }
        catch (Exception ex)
        {
            return "未設定"; // 異常終了
        }
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
