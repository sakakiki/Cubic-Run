using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource_BGM;
    public AudioSource audioSource_SE;

    public float volume_BGM = 1;
    public float volume_SE = 1;

    public AudioClip BGM_Menu;
    public AudioClip BGM_Play;
    public AudioClip BGM_Result;

    public AudioClip SE_Button;
    public AudioClip SE_Player;
    public AudioClip SE_Enemy;
    public AudioClip SE_Close;
    public AudioClip SE_SkinSelecter;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySE(AudioClip clip)
    {
        audioSource_SE.PlayOneShot(clip, volume_SE);
    }
}
