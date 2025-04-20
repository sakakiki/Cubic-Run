using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private NativeOverlayAd _nativeOverlayAd; //ネイティブ広告本体
    private RewardedAd _rewardedAd; //リワード広告本体
    private bool isGetRewarded = false; //リワード広告の報酬を取得したかどうか

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
                MainBackgroundColor = Color.white,
                CallToActionText = new NativeTemplateTextStyle
                {
                    BackgroundColor = Color.gray,
                    TextColor = Color.white,
                    FontSize = 9,
                    Style = NativeTemplateFontStyle.Bold
                }
            };
            /*
            // 広告描画対象の四角形の四隅の座標を取得
            Vector3[] corners = new Vector3[4];
            targetRect.GetWorldCorners(corners);

            // 左下・右上のスクリーン座標を取得
            Vector3 bottomLeft = Camera.main.WorldToScreenPoint(corners[0]);
            Vector3 topRight = Camera.main.WorldToScreenPoint(corners[2]);

            // 幅と高さを計算（ピクセル）
            int adWidth = Mathf.RoundToInt((topRight.x - bottomLeft.x)/3);
            int adHeight = Mathf.RoundToInt((topRight.y - bottomLeft.y)/3);

            // 左上座標を計算（RenderTemplateの原点は左上）
            int adPosX = Mathf.RoundToInt(bottomLeft.x/2);
            int adPosY = Mathf.RoundToInt(topRight.y/2);


            // ネイティブオーバーレイ広告を指定場所にレンダリング
            _nativeOverlayAd.RenderTemplate(style, new AdSize(adWidth, adHeight), adPosX, adPosY);
            //_nativeOverlayAd.RenderTemplate(style, new AdSize(adWidth, adHeight), 50, 10);

            Debug.Log($"Ad position (top-left): x={adPosX}, y={adPosY}, width={adWidth}, height={adHeight}");
            */

            Vector3[] corners = new Vector3[4];
            targetRect.GetWorldCorners(corners);

            // 左上と右下（Unityの座標系：左下原点）
            Vector3 screenTopLeft = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[1]);
            Vector3 screenBottomRight = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[3]);

            // 座標（ピクセル）
            float pxX = screenTopLeft.x;
            float pxY = Screen.height - screenTopLeft.y; // 上からの距離に変換
            float pxWidth = screenBottomRight.x - screenTopLeft.x;
            float pxHeight = screenTopLeft.y - screenBottomRight.y;

            // dpi → dp変換
            float dpi = Screen.dpi;
            if (dpi == 0) dpi = 320f; // fallback。端末によって0になることがある

            float dpX = pxX / (dpi / 160f);
            float dpY = pxY / (dpi / 160f);
            float dpWidth = pxWidth / (dpi / 160f);
            float dpHeight = pxHeight / (dpi / 160f);
            int minDpSize = 120;
            int maxDpX = Mathf.RoundToInt(Screen.width / (dpi / 160f));
            int maxDpY = Mathf.RoundToInt(Screen.height / (dpi / 160f));

            // サイズ制限
            int adDpWidth = Mathf.Max(Mathf.RoundToInt(dpWidth), minDpSize);
            int adDpHeight = Mathf.Max(Mathf.RoundToInt(dpHeight), minDpSize);

            // 位置制限（画面外に出ないように補正）
            int adDpX = Mathf.Clamp(Mathf.RoundToInt(dpX), 0, maxDpX - adDpWidth);
            int adDpY = Mathf.Clamp(Mathf.RoundToInt(dpY), 0, maxDpY - adDpHeight);

            // AdMob描画
            /*
            // 表示
            _nativeOverlayAd.RenderTemplate(
                style,
                new AdSize(adDpWidth, adDpHeight),
                adDpX,
                adDpY
            );
            _nativeOverlayAd.RenderTemplate(
                style,
                new AdSize(adDpWidth, adDpHeight),
                0,
                0
            );
            */

            Debug.Log($"Ad in dp → pos:({dpX}, {dpY}) size:({dpWidth}, {dpHeight}) | dpi: {dpi}");
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
    }



    /// <summary>
    /// リワード広告の再生
    /// </summary>
    private void ShowRewardedAd()
    {
        // フラグのリセット
        isGetRewarded = false;

        // 広告が再生可能か確認
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            // オーディオの停止
            AudioManager.Instance.audioSource_BGM.Stop();
            AudioManager.Instance.audioSource_SE.Stop();

            // ShowRewardedAd の中で、イベント登録を追加
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                // 広告が閉じられた後に音を復旧
                AudioSettings.Reset(AudioSettings.GetConfiguration());
                AudioManager.Instance.audioSource_BGM.Play();
                AudioManager.Instance.audioSource_SE.Play();

                if (isGetRewarded)
                {
                    // スタミナ回復の通知
                    PopupUIManager.Instance.SetupMessageBand("スタミナを2回復しました。", 2);
                }

                //メニュー画面に戻して終了
                GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
            };

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

                    //報酬の付与フラグをtrueに
                    isGetRewarded = true;
                }
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