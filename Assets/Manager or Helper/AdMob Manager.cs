using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private NativeOverlayAd _nativeOverlayAd; //ネイティブ広告本体
    private RewardedAd _rewardedAd; //リワード広告本体

    // 広告ユニット
#if UNITY_ANDROID
    private string _adUnitId_nativeOverlay = "ca-app-pub-3940256099942544/2247696110"; //テスト用
    //private string _adUnitId_nativeOverlay = "ca-app-pub-3396760301690878/9424445321";
    private string _adUnitId_reward = "ca-app-pub-3940256099942544/5224354917"; //テスト用
    //private string _adUnitId_reward = "ca-app-pub-3396760301690878/6470855214";
#elif UNITY_IPHONE
    private string _adUnitId_nativeOverlay = "ca-app-pub-3940256099942544/3986624511"; //テスト用
    private string _adUnitId_reward = "ca-app-pub-3940256099942544/1712485313"; //テスト用
#else
    private string _adUnitId_nativeOverlay = "unused";
    private string _adUnitId_reward = "unused";
#endif



    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }



    void Start()
    {
        //初期化処理
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initializeStatus =>
        {
            isReady = true;
            Debug.Log(initializeStatus);
        });
    }



    /// <summary>
    /// ネイティブ広告のロードと表示
    /// </summary>
    public void LoadAndRenderNativeAd(RectTransform targetRect)
    {
        // 新しい広告をロードする前に古い広告をクリーンアップ
        if (_nativeOverlayAd != null)
        {
            DestroyNativeAd();
        }

        Debug.Log("広告ロード開始");

        // 広告を読み込むためのリクエストを作成
        var adRequest = new AdRequest();
        Debug.Log("リクエストを作成");

        // オプション：ネイティブ広告のオプションを定義
        // アスペクト比に応じて広告の形を変更
        var options = new NativeAdOptions { };
        float targetAspect = targetRect.localScale.x / targetRect.localScale.y;
        switch (targetAspect)
        {
            case < 0.8f: options.MediaAspectRatio = MediaAspectRatio.Portrait; break; //縦長の広告
            case > 1.2f: options.MediaAspectRatio = MediaAspectRatio.Landscape; break; //横長の広告
            default: options.MediaAspectRatio = MediaAspectRatio.Square; break;
        }
        Debug.Log("オプションを定義");

        // 広告を読み込むリクエストを送信
        NativeOverlayAd.Load(_adUnitId_nativeOverlay, adRequest, options,
            (NativeOverlayAd ad, LoadAdError error) =>
            {
                Debug.Log("コールバック到達");

                if (error != null)
                {
                    Debug.LogError("Native Overlay ad failed to load an ad " +
                               " with error: " + error);
                    return;
                }

                // エラーがnullの場合、広告は常にnullではないはずだが
                // クラッシュを避けるためにダブルチェックを行う
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Native Overlay ad load event " +
                               " fired with null ad and null error.");
                    return;
                }

                // 正常に終了
                Debug.Log("Native Overlay ad loaded with response : " +
                       ad.GetResponseInfo());
                _nativeOverlayAd = ad;

                // 広告を表示
                RenderNativeAd(targetRect);
            });
    }

    /// <summary>
    /// ネイティブ広告のレンダリング
    /// </summary>
    private void RenderNativeAd(RectTransform targetRect)
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("広告のレンダリング");

            // カスタムスタイルでネイティブテンプレートスタイルを定義する
            var style = new NativeTemplateStyle
            {
                TemplateId = NativeTemplateId.Medium,
                MainBackgroundColor = Color.red,
                CallToActionText = new NativeTemplateTextStyle
                {
                    BackgroundColor = Color.green,
                    TextColor = Color.white,
                    FontSize = 9,
                    Style = NativeTemplateFontStyle.Bold
                }
            };

            // 広告描画対象の四角形の四隅の座標を取得
            Vector3[] corners = new Vector3[4];
            targetRect.GetWorldCorners(corners);

            // 左下・右上のスクリーン座標を取得
            Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector3 topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

            // 幅と高さを計算（ピクセル）
            int adWidth = Mathf.RoundToInt(topRight.x - bottomLeft.x);
            int adHeight = Mathf.RoundToInt(topRight.y - bottomLeft.y);

            // 左上座標を計算（RenderTemplateの原点は左上）
            int adPosX = Mathf.RoundToInt(bottomLeft.x);
            int adPosY = Mathf.RoundToInt(Screen.height - topRight.y); // Y軸反転に注意

            // ネイティブオーバーレイ広告を指定場所にレンダリング
            _nativeOverlayAd.RenderTemplate(style, new AdSize(adWidth, adHeight), adPosX, adPosY);
        }
    }

    /// <summary>
    /// ネイティブ広告を破棄
    /// </summary>
    public void DestroyNativeAd()
    {
        Debug.Log("広告を破棄");
        if (_nativeOverlayAd != null)
        {
            Debug.Log("広告を破棄");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }
    }



    /// <summary>
    /// リワード広告のロードと再生
    /// </summary>
    public void LoadAndShowRewardedAd()
    {
        // 新しい広告をロードする前に古い広告をクリーンアップ
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("広告ロード開始");

        // 広告を読み込むためのリクエストを作成
        var adRequest = new AdRequest();
        Debug.Log("リクエストを作成");

        // 広告をロードするリクエストを送信
        RewardedAd.Load(_adUnitId_reward, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                Debug.Log("コールバック到達");

                // エラーがnullでないか広告がnullの場合、ロード失敗
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    //メニュー画面に戻して終了
                    GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                //広告の再生
                ShowRewardedAd();
            });

        //メニュー画面に戻して終了
        GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
    }



    /// <summary>
    /// リワード広告の再生
    /// </summary>
    private void ShowRewardedAd()
    {
        // 広告が再生可能か確認
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(async (Reward reward) =>
            {
                // 報酬の付与
                Debug.Log("Reward：" + reward.Type + " × " + reward.Amount);

                //回復後のスタミナ残量を取得
                int remainingStamina = await FirestoreManager.Instance.AddStamina(2);

                //通信エラー通知
                if (remainingStamina == -2)
                {
                    PopupUIManager.Instance.SetupMessageBand("通信エラーが発生しました。", 2);
                }

                else
                {
                    //スタミナ表示の更新
                    GameManager.Instance.UpdateStamina(remainingStamina);

                    //超過スタミナ消費の場合の処理
                    if (remainingStamina >= 3)
                        GameManager.Instance.UpdateOverStamina(remainingStamina);
                }

                // スタミナ回復の通知
                PopupUIManager.Instance.SetupMessageBand("スタミナを2回復しました。", 2);
            });
        }
        // 広告が再生できない場合
        else
        {
            // 広告が準備できていないことの通知
            PopupUIManager.Instance.SetupMessageBand("広告の取得に失敗しました。", 2);
        }
    }
}