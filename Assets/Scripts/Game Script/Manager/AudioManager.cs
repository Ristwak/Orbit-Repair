using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;      // Background music
    public AudioSource sfxSource;        // For UI clicks, equip sounds
    public AudioSource narrationSource;  // For narrator voice lines

    [Header("Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Narration Clips")]
    public AudioClip introClip;
    public AudioClip skinCoolingClip;
    public AudioClip mainSuitClip;
    public AudioClip shoesClip;
    public AudioClip leftGloveClip;
    public AudioClip rightGloveClip;
    public AudioClip backpackClip;
    public AudioClip helmetClip;
    public AudioClip missionCompleteClip;
    public AudioClip timeOverClip;

    private void Awake()
    {
        // Singleton pattern (only one AudioManager in scene)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(menuMusic);
    }

    // --- Music ---
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // --- SFX ---
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // --- Narration ---
    public void PlayNarration(AudioClip clip)
    {
        if (narrationSource != null && clip != null)
        {
            narrationSource.Stop(); // Stop previous narration if any
            narrationSource.clip = clip;
            narrationSource.Play();
        }
    }
}
