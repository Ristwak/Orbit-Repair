using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;      // Background music
    public AudioSource sfxSource;        // For UI clicks, lever, etc.
    public AudioSource narrationSource;  // For narrator voice lines

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float duckedMusicVolume = 0.25f; // Music lowered during narration
    public bool duckMusicDuringNarration = true;

    [Header("Narration Clips (Orbit Repair)")]
    public AudioClip welcomeSuitupClip;      // 1) At game start
    public AudioClip afterSuitClip;          // 2) After suit-up
    public AudioClip afterToolPickupClip;    // 3) After tool pickup
    public AudioClip missionCompleteClip;    // 4) Satellite fixed
    public AudioClip missionFailClip;        // 5) Player failed / time over

    // Internal coroutine handle used to fade music volume down/up
    private Coroutine duckRoutine;

    public enum NarrationCue
    {
        WelcomeSuitUp,
        AfterSuitUp,
        AfterToolPickup,
        MissionComplete,
        MissionFail
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // Optional: keep across scenes
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(menuMusic);
    }

    // --- Music ---
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    // --- SFX ---
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip) sfxSource.PlayOneShot(clip);
    }

    // --- Narration (direct clip) ---
    public void PlayNarration(AudioClip clip)
    {
        if (!narrationSource || !clip) return;

        narrationSource.Stop();
        narrationSource.clip = clip;

        // Lower background music while narration plays
        if (duckMusicDuringNarration && musicSource)
        {
            if (duckRoutine != null) StopCoroutine(duckRoutine);
            duckRoutine = StartCoroutine(DuckMusicWhileNarration());
        }

        narrationSource.Play();
    }

    // --- Narration (by cue) ---
    public void PlayNarrationCue(NarrationCue cue)
    {
        switch (cue)
        {
            case NarrationCue.WelcomeSuitUp:
                PlayNarration(welcomeSuitupClip);
                break;
            case NarrationCue.AfterSuitUp:
                PlayNarration(afterSuitClip);
                break;
            case NarrationCue.AfterToolPickup:
                PlayNarration(afterToolPickupClip);
                break;
            case NarrationCue.MissionComplete:
                PlayNarration(missionCompleteClip);
                break;
            case NarrationCue.MissionFail:
                PlayNarration(missionFailClip);
                break;
        }
    }

    // --- Music-ducking coroutine ---
    private IEnumerator DuckMusicWhileNarration()
    {
        // Fade music down
        float t = 0f;
        float start = musicSource.volume;
        float target = duckedMusicVolume;

        while (t < 0.12f)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, target, t / 0.12f);
            yield return null;
        }

        musicSource.volume = target;

        // Wait for narration to finish
        while (narrationSource.isPlaying)
            yield return null;

        // Fade music back up
        t = 0f;
        start = musicSource.volume;
        target = musicVolume;

        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, target, t / 0.15f);
            yield return null;
        }

        musicSource.volume = target;
        duckRoutine = null;
    }
}
