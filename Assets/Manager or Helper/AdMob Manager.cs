using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private InterstitialAd _interstitialAd; //インタースティシャル広告本体
    private RewardedAd _rewardedAd; //リワード広告本体
    private bool isGetRewarded = false; //リワード広告の報酬を取得したかどうか

    // 広告ユニット
#if UNITY_ANDROID
    private string _adUnitId_interstitial = "ca-app-pub-3396760301690878/8715908439"; //テスト用
    //private string _adUnitId_interstitial = "ca-app-pub-3396760301690878/9424445321";
    private string _adUnitId_reward = "ca-app-pub-3940256099942544/5224354917"; //テスト用
    //private string _adUnitId_reward = "ca-app-pub-3396760301690878/6470855214";
#elif UNITY_IPHONE
    private string _adUnitId_interstitial = "ca-app-pub-3940256099942544/4411468910"; //テスト用
    //private string _adUnitId_interstitial = "ca-app-pub-3396760301690878/1028990103"
    private string _adUnitId_reward = "ca-app-pub-3940256099942544/1712485313"; //テスト用
    //private string _adUnitId_reward = "ca-app-pub-3396760301690878/4206835717";
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
    /// リワード広告のロードと再生
    /// </summary>
    public void LoadAndShowRewardedAd_Stamina()
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
                                   
                    // 広告が準備できていないことの通知
                    PopupUIManager.Instance.SetupMessageBand("広告の取得に失敗しました。", 2);

                    //メニュー画面に戻して終了
                    GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                // イベント登録を追加
                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    // 広告が閉じられた後に音を復旧
                    AudioSettings.Reset(AudioSettings.GetConfiguration());
                    AudioManager.Instance.PlayBGM(AudioManager.BGM.Menu);

                    if (isGetRewarded)
                    {
                        // スタミナ回復の通知
                        PopupUIManager.Instance.SetupMessageBand("スタミナを2回復しました。", 2);
                    }

                    //メニュー画面に戻して終了
                    GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
                };

                //広告の再生
                ShowRewardedAd_Stamina();
            });
    }

    /// <summary>
    /// リワード広告の再生
    /// </summary>
    private void ShowRewardedAd_Stamina()
    {
        // フラグのリセット
        isGetRewarded = false;

        // 広告が再生可能か確認
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            // オーディオの停止
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.StopSE();

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



    /// <summary>
    /// リワード広告のロードと再生
    /// </summary>
    public void LoadAndShowRewardedAd_StopAd()
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

                    // 広告が準備できていないことの通知
                    PopupUIManager.Instance.SetupMessageBand("広告の取得に失敗しました。", 2);

                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                // イベント登録を追加
                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    // 広告が閉じられた後に音を復旧
                    AudioSettings.Reset(AudioSettings.GetConfiguration());
                    AudioManager.Instance.PlayBGM(AudioManager.BGM.Menu);

                    if (isGetRewarded)
                    {
                        // 広告停止
                        AdEnableTimeManager.Save(DateTime.Now.AddHours(1));

                        // 広告停止の通知
                        PopupUIManager.Instance.SetupMessageBand("全画面広告の表示を1時間停止しました。", 2);
                    }
                };

                //広告の再生
                ShowRewardedAd_StopAd();
            });
    }

    /// <summary>
    /// リワード広告の再生
    /// </summary>
    private void ShowRewardedAd_StopAd()
    {
        // フラグのリセット
        isGetRewarded = false;

        // 広告が再生可能か確認
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            // オーディオの停止
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.StopSE();

            _rewardedAd.Show((Reward reward) =>
            {
                //報酬の付与フラグをtrueに
                isGetRewarded = true;
            });
        }
        // 広告が再生できない場合
        else
        {
            // 広告が準備できていないことの通知
            PopupUIManager.Instance.SetupMessageBand("広告の取得に失敗しました。", 2);
        }
    }

    public void StopAdButton()
    {
        //広告が配信状態なら広告視聴を提案
        if (DateTime.Now > AdEnableTimeManager.Load())
        {
            PopupUIManager.Instance.SetupPopup(
                "広告の停止",
                "短い広告を見て全画面広告の\n表示を1時間停止しますか？\n\n※スタミナを回復する場合の\n広告の視聴は必要です。",
                LoadAndShowRewardedAd_StopAd);
        }

        //広告が停止中なら通知
        else
        {
            PopupUIManager.Instance.SetupPopupMessage(
                "広告停止中",
                "全画面広告は停止中です。\n\n残り時間："　+ (int)(AdEnableTimeManager.Load() - DateTime.Now).TotalMinutes + "分");
        }
    }



    /// <summary>
    /// インタースティシャル広告のロードと再生
    /// </summary>
    public void LoadAndShowInterstitial()
    {
        // 新しい広告をロードする前に古い広告をクリーンアップ
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        // 広告を読み込むためのリクエストを作成
        var adRequest = new AdRequest();

        // 広告をロードするリクエストを送信
        InterstitialAd.Load(_adUnitId_interstitial, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                Debug.Log("コールバック到達");

                // エラーがnullでないか広告がnullの場合、ロード失敗
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    // リザルト画面のUIを有効化して終了
                    InputManager.Instance.InputUISetActive_Result(true);

                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;

                // イベント登録を追加
                _interstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    // 広告が閉じられた後に音を復旧
                    AudioSettings.Reset(AudioSettings.GetConfiguration());
                    AudioManager.Instance.PlayBGM(AudioManager.BGM.Result);

                    // GameManagerの広告関連の数値をリセット
                    GameManager.Instance.adScore = 0;
                    GameManager.Instance.adCount = 0;

                    // リザルト画面のUIを有効化して終了
                    InputManager.Instance.InputUISetActive_Result(true);
                };

                //広告の再生
                ShowInterstitial();
            });
    }

    /// <summary>
    /// インタースティシャル広告の再生
    /// </summary>
    private void ShowInterstitial()
    {
        // 広告が再生可能か確認
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            // オーディオの停止
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.StopSE();

            // 広告の再生
            _interstitialAd.Show();
        }
        // 広告が再生できない場合
        else
        {
            // リザルト画面のUIを有効化して終了
            InputManager.Instance.InputUISetActive_Result(true);
        }
    }
}