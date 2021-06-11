using UnityEngine;

public class AudioManager : MonoBehaviour
{
   [SerializeField]
   private AudioSource sfxSource = default;
   [SerializeField]
   private AudioSource ambienceSource = default;
   private static AudioManager _instance;

    void Awake()
    {
        _instance = this;
        ambienceSource.loop = true;
    }
    
    public static void PlaySFX(AudioClip audioClip)
    {
        _instance.sfxSource.PlayOneShot(audioClip);
    }

    public static void StopSFX()
    {
        _instance.sfxSource.Stop();
    }

    public static void SetAmbience(AudioClip audioClip)
    {  
        _instance.ambienceSource.Stop();
        _instance.ambienceSource.clip = audioClip;
    }

    public static void PlayAbience()
    {  
        _instance.ambienceSource.Play();
    }

    public static void StopAmbience()
    {  
        _instance.ambienceSource.Stop();
    }

    public static void PauseAmbience()
    {
        _instance.ambienceSource.Pause();
    }
}