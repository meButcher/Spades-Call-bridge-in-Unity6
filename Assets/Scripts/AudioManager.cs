using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource; 
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmMusic;
    public AudioClip buttonClickSFX;
    public AudioClip cardHoverSFX;
    public AudioClip cardPlaceSFX;
    public AudioClip trickWinSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }
    
    private void PlayBGM()
    {
        bgmSource.clip = bgmMusic;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopSfx()
    {
        sfxSource.Stop();
    }

    private void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    
    public void PlayButtonClick()
    {
        PlaySfx(buttonClickSFX);
    }

    public void PlayCardHover()
    {
        PlaySfx(cardHoverSFX);
    }

    public void PlayCardPlace()
    {
        PlaySfx(cardPlaceSFX);
    }

    public void PlayTrickWin()
    {
        PlaySfx(trickWinSFX);
    }


}