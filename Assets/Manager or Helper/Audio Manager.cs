using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource_BGM;
    [SerializeField] private AudioSource audioSource_SE;

    public Slider slider_BGM;
    public Slider slider_SE;

    public float volume_BGM = 1;
    public float volume_SE = 1;

    public AudioClip BGM_Menu;
    public AudioClip BGM_Play;
    public AudioClip BGM_Result;

    public AudioClip SE_Button;
    public AudioClip SE_Player;
    public AudioClip SE_GameOver;
    public AudioClip SE_Enemy;
    public AudioClip SE_Close;
    public AudioClip SE_SkinSelecter;
    public AudioClip SE_SkinUnlock;
    public AudioClip SE_Panel;
    public AudioClip SE_GetExp;
    public AudioClip SE_LevelUp;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySE(AudioClip clip)
    {
        audioSource_SE.PlayOneShot(clip, volume_SE);
    }

    public void StopSE()
    {
        audioSource_SE.Stop();
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
