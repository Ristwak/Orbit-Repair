using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;      // Background music
    public AudioSource sfxSource;        // For UI clicks, lever, etc.
    public AudioSource narrationSource;  // For narrator voice lines

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float duckedMusicVolume = 0.25f; // music lowered during narration
    public bool duckMusicDuringNarration = true;

    [Header("Narration Clips (Orbit Repair)")]
    public AudioClip welcomeSuitupClip;      // 1) At game start
    public AudioClip afterSuitClip;          // 2) After suit-up
    public AudioClip afterToolPickupClip;    // 3) After tool pickup
    public AudioClip missionCompleteClip;    // 4) Satellite fixed
    public AudioClip missionFailClip;        // 5) Player failed / time over

    // internal coroutine handle used to fade music volume down/up
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
        // Optionally persist across scenes:
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

    // --- Narration ---
    public void PlayNarration(AudioClip clip)
    {
        if (!narrationSource || !clip) return;

        narrationSource.Stop();
        narrationSource.clip = clip;

        // lower background music during narration if enabled
        if (duckMusicDuringNarration && musicSource)
        {
            if (duckRoutine != null) StopCoroutine(duckRoutine);
            duckRoutine = StartCoroutine(DuckMusicWhileNarration());
        }

        narrationSource.Play();
    }

    // --- Narration by cue ---
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
        // Fades music down, waits while narration plays, then fades back up.
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
        while (narrationSource.isPlaying) yield return null;

        // fade music back up
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
        duckRoutine = null;  // reset handle
    }
}
