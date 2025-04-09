using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private NativeOverlayAd _nativeOverlayAd; //�l�C�e�B�u�L���{��
    private RewardedAd _rewardedAd; //�����[�h�L���{��

    // �L�����j�b�g
#if UNITY_ANDROID
    private string _adUnitId_nativeOverlay = "ca-app-pub-3940256099942544/2247696110"; //�e�X�g�p
    //private string _adUnitId_nativeOverlay = "ca-app-pub-3396760301690878/9424445321";
    private string _adUnitId_reward = "ca-app-pub-3940256099942544/5224354917"; //�e�X�g�p
    //private string _adUnitId_reward = "ca-app-pub-3396760301690878/6470855214";
#elif UNITY_IPHONE
    private string _adUnitId_nativeOverlay = "ca-app-pub-3940256099942544/3986624511"; //�e�X�g�p
    private string _adUnitId_reward = "ca-app-pub-3940256099942544/1712485313"; //�e�X�g�p
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
        //����������
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initializeStatus =>
        {
            isReady = true;
            Debug.Log(initializeStatus);
        });
    }



    /// <summary>
    /// �l�C�e�B�u�L���̃��[�h�ƕ\��
    /// </summary>
    public void LoadAndRenderNativeAd(RectTransform targetRect)
    {
        // �V�����L�������[�h����O�ɌÂ��L�����N���[���A�b�v
        if (_nativeOverlayAd != null)
        {
            DestroyNativeAd();
        }

        Debug.Log("�L�����[�h�J�n");

        // �L����ǂݍ��ނ��߂̃��N�G�X�g���쐬
        var adRequest = new AdRequest();
        Debug.Log("���N�G�X�g���쐬");

        // �I�v�V�����F�l�C�e�B�u�L���̃I�v�V�������`
        // �A�X�y�N�g��ɉ����čL���̌`��ύX
        var options = new NativeAdOptions { };
        float targetAspect = targetRect.localScale.x / targetRect.localScale.y;
        switch (targetAspect)
        {
            case < 0.8f: options.MediaAspectRatio = MediaAspectRatio.Portrait; break; //�c���̍L��
            case > 1.2f: options.MediaAspectRatio = MediaAspectRatio.Landscape; break; //�����̍L��
            default: options.MediaAspectRatio = MediaAspectRatio.Square; break;
        }
        Debug.Log("�I�v�V�������`");

        // �L����ǂݍ��ރ��N�G�X�g�𑗐M
        NativeOverlayAd.Load(_adUnitId_nativeOverlay, adRequest, options,
            (NativeOverlayAd ad, LoadAdError error) =>
            {
                Debug.Log("�R�[���o�b�N���B");

                if (error != null)
                {
                    Debug.LogError("Native Overlay ad failed to load an ad " +
                               " with error: " + error);
                    return;
                }

                // �G���[��null�̏ꍇ�A�L���͏��null�ł͂Ȃ��͂�����
                // �N���b�V��������邽�߂Ƀ_�u���`�F�b�N���s��
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Native Overlay ad load event " +
                               " fired with null ad and null error.");
                    return;
                }

                // ����ɏI��
                Debug.Log("Native Overlay ad loaded with response : " +
                       ad.GetResponseInfo());
                _nativeOverlayAd = ad;

                // �L����\��
                RenderNativeAd(targetRect);
            });
    }

    /// <summary>
    /// �l�C�e�B�u�L���̃����_�����O
    /// </summary>
    private void RenderNativeAd(RectTransform targetRect)
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("�L���̃����_�����O");

            // �J�X�^���X�^�C���Ńl�C�e�B�u�e���v���[�g�X�^�C�����`����
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

            // �L���`��Ώۂ̎l�p�`�̎l���̍��W���擾
            Vector3[] corners = new Vector3[4];
            targetRect.GetWorldCorners(corners);

            // �����E�E��̃X�N���[�����W���擾
            Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector3 topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

            // ���ƍ������v�Z�i�s�N�Z���j
            int adWidth = Mathf.RoundToInt(topRight.x - bottomLeft.x);
            int adHeight = Mathf.RoundToInt(topRight.y - bottomLeft.y);

            // ������W���v�Z�iRenderTemplate�̌��_�͍���j
            int adPosX = Mathf.RoundToInt(bottomLeft.x);
            int adPosY = Mathf.RoundToInt(Screen.height - topRight.y); // Y�����]�ɒ���

            // �l�C�e�B�u�I�[�o�[���C�L�����w��ꏊ�Ƀ����_�����O
            _nativeOverlayAd.RenderTemplate(style, new AdSize(adWidth, adHeight), adPosX, adPosY);
        }
    }

    /// <summary>
    /// �l�C�e�B�u�L����j��
    /// </summary>
    public void DestroyNativeAd()
    {
        Debug.Log("�L����j��");
        if (_nativeOverlayAd != null)
        {
            Debug.Log("�L����j��");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }
    }



    /// <summary>
    /// �����[�h�L���̃��[�h�ƍĐ�
    /// </summary>
    public void LoadAndShowRewardedAd()
    {
        // �V�����L�������[�h����O�ɌÂ��L�����N���[���A�b�v
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("�L�����[�h�J�n");

        // �L����ǂݍ��ނ��߂̃��N�G�X�g���쐬
        var adRequest = new AdRequest();
        Debug.Log("���N�G�X�g���쐬");

        // �L�������[�h���郊�N�G�X�g�𑗐M
        RewardedAd.Load(_adUnitId_reward, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                Debug.Log("�R�[���o�b�N���B");

                // �G���[��null�łȂ����L����null�̏ꍇ�A���[�h���s
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    //���j���[��ʂɖ߂��ďI��
                    GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                //�L���̍Đ�
                ShowRewardedAd();
            });

        //���j���[��ʂɖ߂��ďI��
        GameManager.Instance.gameStateMachine.ChangeState(GameManager.Instance.gameStateMachine.state_Menu);
    }



    /// <summary>
    /// �����[�h�L���̍Đ�
    /// </summary>
    private void ShowRewardedAd()
    {
        // �L�����Đ��\���m�F
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(async (Reward reward) =>
            {
                // ��V�̕t�^
                Debug.Log("Reward�F" + reward.Type + " �~ " + reward.Amount);

                //�񕜌�̃X�^�~�i�c�ʂ��擾
                int remainingStamina = await FirestoreManager.Instance.AddStamina(2);

                //�ʐM�G���[�ʒm
                if (remainingStamina == -2)
                {
                    PopupUIManager.Instance.SetupMessageBand("�ʐM�G���[���������܂����B", 2);
                }

                else
                {
                    //�X�^�~�i�\���̍X�V
                    GameManager.Instance.UpdateStamina(remainingStamina);

                    //���߃X�^�~�i����̏ꍇ�̏���
                    if (remainingStamina >= 3)
                        GameManager.Instance.UpdateOverStamina(remainingStamina);
                }

                // �X�^�~�i�񕜂̒ʒm
                PopupUIManager.Instance.SetupMessageBand("�X�^�~�i��2�񕜂��܂����B", 2);
            });
        }
        // �L�����Đ��ł��Ȃ��ꍇ
        else
        {
            // �L���������ł��Ă��Ȃ����Ƃ̒ʒm
            PopupUIManager.Instance.SetupMessageBand("�L���̎擾�Ɏ��s���܂����B", 2);
        }
    }
}