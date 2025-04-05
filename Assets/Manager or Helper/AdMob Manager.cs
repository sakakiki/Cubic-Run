using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private NativeOverlayAd _nativeOverlayAd; //�L���{��

    // �����̍L�����j�b�g�́A��Ƀe�X�g�L����z�M����悤�ɐݒ肳��Ă��܂��B
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
        //����������
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initializeStatus =>
        {
            isReady = true;
            Debug.Log(initializeStatus);
        });

        LoadAd();
    }



    /// <summary>
    /// �L���̃��[�h
    /// </summary>
    public void LoadAd()
    {
        // �V�����L�������[�h����O�ɌÂ��L�����N���[���A�b�v
        if (_nativeOverlayAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading native overlay ad.");

        // �L����ǂݍ��ނ��߂̃��N�G�X�g���쐬
        var adRequest = new AdRequest();
        Debug.Log("���N�G�X�g���쐬");

        // �I�v�V�����F�l�C�e�B�u�L���̃I�v�V�������`
        var options = new NativeAdOptions
        {
            MediaAspectRatio = MediaAspectRatio.Landscape, //�����̍L��
        };
        Debug.Log("�I�v�V�������`");

        _adUnitId = "ca-app-pub-3940256099942544/2247696110";

        // �L����ǂݍ��ރ��N�G�X�g�𑗐M
        NativeOverlayAd.Load(_adUnitId, adRequest, options,
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

                // �L���C�x���g�ɓo�^���ċ@�\���g��
                //RegisterEventHandlers(ad);
                RenderAd(0, 0, 1000, 500);
            });
    }
    
    
    
    /// <summary>
    /// �L���̃����_�����O
    /// </summary>
    public void RenderAd(int posX, int posY, int width, int height)
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

            AdSize adSize = new AdSize(width, height);

            // �l�C�e�B�u�I�[�o�[���C�L�����w��ꏊ�Ɏw��T�C�Y�Ń����_�����O
            _nativeOverlayAd.RenderTemplate(style, adSize, posX, posY);
        }
    }



    /// <summary>
    /// ��\���L���̍ĕ\��
    /// </summary>
    public void ShowAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("��\���L���̍ĕ\��");
            _nativeOverlayAd.Show();
        }
    }


    /// <summary>
    /// �L�����\��
    /// </summary>
    public void HideAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("�L�����\��");
            _nativeOverlayAd.Hide();
        }
    }



    /// <summary>
    /// �L����j��
    /// </summary>
    public void DestroyAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("�L����j��");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }
    }
}