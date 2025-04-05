using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private NativeOverlayAd _nativeOverlayAd; //広告本体
    private GameObject dummyAdUI; // エディタ用のダミー広告表示

    // これらの広告ユニットは、常にテスト広告を配信するように設定されています。
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/3986624511";
#else
    private string _adUnitId = "unused";
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
    /// 広告のロード
    /// </summary>
    public void LoadAd()
    {
        // 新しい広告をロードする前に古い広告をクリーンアップ
        if (_nativeOverlayAd != null)
        {
            DestroyAd();
        }

        Debug.Log("広告ロード開始");

        // 広告を読み込むためのリクエストを作成
        var adRequest = new AdRequest();
        Debug.Log("リクエストを作成");

        // オプション：ネイティブ広告のオプションを定義
        var options = new NativeAdOptions
        {
            MediaAspectRatio = MediaAspectRatio.Landscape, //横長の広告
        };
        Debug.Log("オプションを定義");

        // 広告を読み込むリクエストを送信
        NativeOverlayAd.Load(_adUnitId, adRequest, options,
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

                // 広告イベントに登録して機能を拡張
                //RegisterEventHandlers(ad);
            });
    }
    
    
    
    /// <summary>
    /// 広告のレンダリング
    /// </summary>
    public void RenderAd(RectTransform targetRect)
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
    /// 非表示広告の再表示
    /// </summary>
    public void ShowAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("非表示広告の再表示");
            _nativeOverlayAd.Show();
        }

#if UNITY_EDITOR
        if (dummyAdUI != null)
        {
            dummyAdUI.SetActive(true);
        }
#endif
    }


    /// <summary>
    /// 広告を非表示
    /// </summary>
    public void HideAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("広告を非表示");
            _nativeOverlayAd.Hide();
        }

#if UNITY_EDITOR
        if (dummyAdUI != null)
        {
            dummyAdUI.SetActive(false);
        }
#endif
    }



    /// <summary>
    /// 広告を破棄
    /// </summary>
    public void DestroyAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("広告を破棄");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }

#if UNITY_EDITOR
        if (dummyAdUI != null)
        {
            Destroy(dummyAdUI);
        }
#endif
    }
}