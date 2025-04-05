using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool isReady { private set; get; } = false;
    private NativeOverlayAd _nativeOverlayAd; //�L���{��
    private GameObject dummyAdUI; // �G�f�B�^�p�̃_�~�[�L���\��

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

        Debug.Log("�L�����[�h�J�n");

        // �L����ǂݍ��ނ��߂̃��N�G�X�g���쐬
        var adRequest = new AdRequest();
        Debug.Log("���N�G�X�g���쐬");

        // �I�v�V�����F�l�C�e�B�u�L���̃I�v�V�������`
        var options = new NativeAdOptions
        {
            MediaAspectRatio = MediaAspectRatio.Landscape, //�����̍L��
        };
        Debug.Log("�I�v�V�������`");

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
            });
    }
    
    
    
    /// <summary>
    /// �L���̃����_�����O
    /// </summary>
    public void RenderAd(RectTransform targetRect)
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
    /// ��\���L���̍ĕ\��
    /// </summary>
    public void ShowAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("��\���L���̍ĕ\��");
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
    /// �L�����\��
    /// </summary>
    public void HideAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("�L�����\��");
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

#if UNITY_EDITOR
        if (dummyAdUI != null)
        {
            Destroy(dummyAdUI);
        }
#endif
    }
}