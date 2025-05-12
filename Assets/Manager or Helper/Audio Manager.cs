using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private const string PitchShifterParam = "PitchShifterParam_Pitch"; // Audio Mixer の Pitch パラメータ

    [SerializeField] private AudioSource audioSource_BGM;
    [SerializeField] private AudioSource audioSource_SE;

    [SerializeField] private Slider slider_BGM;
    [SerializeField] private Slider slider_SE;

    private float volume_BGM = 1;
    public float volume_SE {private set; get;} = 1;


    [SerializeField] private AudioClip BGM_Menu;
    [SerializeField] private AudioClip BGM_Play_1;
    [SerializeField] private AudioClip BGM_Play_2;
    [SerializeField] private AudioClip BGM_Play_3;
    [SerializeField] private AudioClip BGM_Play_4;
    [SerializeField] private AudioClip BGM_Play_5;
    [SerializeField] private AudioClip BGM_Result;
    public enum BGM{
        Menu,
        Play_1,
        Play_2,
        Play_3,
        Play_4,
        Play_5,
        Result
    }
    public BGM playngBGM {private set; get;} = BGM.Menu;
    private float[] clipVolume_BGM = new float[]{
        0.6f,
        1.0f,
        0.6f,
        0.4f,
        0.4f,
        0.4f,
        0.6f
    };

    [SerializeField] private AudioClip SE_Button;
    [SerializeField] private AudioClip SE_Player_Jump;
    [SerializeField] private AudioClip SE_Player_Squat;
    [SerializeField] private AudioClip SE_Player_Attack;
    [SerializeField] private AudioClip SE_Player_Hold;
    [SerializeField] private AudioClip SE_GameOver;
    [SerializeField] private AudioClip SE_Enemy;
    [SerializeField] private AudioClip SE_Close;
    [SerializeField] private AudioClip SE_SkinSelecter;
    [SerializeField] private AudioClip SE_SkinUnlock;
    [SerializeField] private AudioClip SE_Panel;
    [SerializeField] private AudioClip SE_GetExp;
    [SerializeField] private AudioClip SE_LevelUp;
    [SerializeField] private AudioClip SE_UseStama;
    public enum SE{
        Button,
        Player_Jump,
        Player_Squat,
        Player_Attack,
        Player_Hold,
        GameOver,
        Enemy,
        Close,
        SkinSelecter,
        SkinUnlock,
        Panel,
        GetExp,
        LevelUp,
        UseStama
    }
    private float[] clipVolume_SE = new float[]{
        1.0f,
        1.0f,
        1.0f,
        1.0f,
        0.4f,
        1.0f,
        1.0f,
        1.0f,
        1.0f,
        1.0f,
        1.0f,
        0.6f,
        1.0f,
        1.0f,
    };

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SetBGMSpeed(float speed)
    {
        audioSource_BGM.pitch = speed;
        audioSource_BGM.outputAudioMixerGroup.audioMixer.SetFloat(PitchShifterParam, 1/speed);
    }

    public void SetBGMPlayVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        audioSource_BGM.volume = volume * volume_BGM * clipVolume_BGM[(int)playngBGM];
    }

    public void PlayBGM(BGM clip_BGM)
    {
        AudioClip playClip = null;
        switch (clip_BGM)
        {
            case BGM.Menu:
                playClip = BGM_Menu;
                break;
            case BGM.Play_1:
                playClip = BGM_Play_1;
                break;
            case BGM.Play_2:
                playClip = BGM_Play_2;
                break;
            case BGM.Play_3:
                playClip = BGM_Play_3;
                break;
            case BGM.Play_4:
                playClip = BGM_Play_4;
                break;
            case BGM.Play_5:
                playClip = BGM_Play_5;
                break;
            case BGM.Result:
                playClip = BGM_Result;
                break;

        }
        audioSource_BGM.clip = playClip;
        audioSource_BGM.volume = volume_BGM * clipVolume_BGM[(int)clip_BGM];
        audioSource_BGM.Play();
        playngBGM = clip_BGM;
    }

    public void StopBGM()
    {
        audioSource_BGM.Stop();
    }

    public void SetVolumeBGM()
    {
        volume_BGM = slider_BGM.value;
        audioSource_BGM.volume = volume_BGM;
        VolumeBGMManager.Save(volume_BGM);
    }

    public void SetVolumeBGM(float volume)
    {
        volume_BGM = volume;
        slider_BGM.value = volume;
        audioSource_BGM.volume = volume;
    }

    public void PlaySE(SE clip_SE)
    {
        AudioClip playClip = null;
        switch (clip_SE)
        {
            case SE.Button:
                playClip = SE_Button;
                break;
            case SE.Player_Jump:
                playClip = SE_Player_Jump;
                break;
            case SE.Player_Squat:
                playClip = SE_Player_Squat;
                break;
            case SE.Player_Attack:
                playClip = SE_Player_Attack;
                break;
            case SE.Player_Hold:
                playClip = SE_Player_Hold;
                break;
            case SE.GameOver:
                playClip = SE_GameOver;
                break;
            case SE.Enemy:
                playClip = SE_Enemy;
                break;
            case SE.Close:
                playClip = SE_Close;
                break;
            case SE.SkinSelecter:
                playClip = SE_SkinSelecter;
                break;
            case SE.SkinUnlock:
                playClip = SE_SkinUnlock;
                break;
            case SE.Panel:
                playClip = SE_Panel;
                break;
            case SE.GetExp:
                playClip = SE_GetExp;
                break;
            case SE.LevelUp:
                playClip = SE_LevelUp;
                break;
            case SE.UseStama:
                playClip = SE_UseStama;
                break;

        }
        audioSource_SE.PlayOneShot(playClip, volume_SE * clipVolume_SE[(int)clip_SE]);
    }

    public void StopSE()
    {
        audioSource_SE.Stop();
    }

    public void SetVolumeSE()
    {
        volume_SE = slider_SE.value;
        VolumeSEManager.Save(volume_SE);
    }

    public void SetVolumeSE(float volume)
    {
        volume_SE = volume;
        slider_SE.value = volume;
    }
}
