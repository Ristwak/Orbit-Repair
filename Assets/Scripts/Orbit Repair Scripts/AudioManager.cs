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

    // --- INTERNAL STATE FOR "PLAY-ONCE" BEHAVIOR ---
    private HashSet<AudioClip> _playedNarration = new HashSet<AudioClip>();     // narration one-shots
    private HashSet<AudioClip> _playedNonMenuMusic = new HashSet<AudioClip>();  // any non-menu music one-shots
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
        // DontDestroyOnLoad(gameObject); // enable if you want persistence
    }

    private void Start()
    {
        // Menu music is allowed to loop / restart
        PlayMusic(menuMusic);
    }

    // =========================
    // MUSIC (play-once per clip, EXCEPT menuMusic)
    // =========================
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        // If this is NOT menu music, ensure it only plays once per session
        bool isMenu = (clip == menuMusic);

        if (!isMenu)
        {
            // If we already played this non-menu track once, ignore further calls
            if (_playedNonMenuMusic.Contains(clip)) return;
            _playedNonMenuMusic.Add(clip);
        }

        // Configure looping: ONLY menu music loops
        musicSource.loop = isMenu;

        // If the requested clip is already playing, do nothing (prevents restarts)
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    // =========================
    // SFX (no special restrictions; plays as requested)
    // =========================
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip) sfxSource.PlayOneShot(clip);
    }

    // =========================
    // NARRATION (each clip plays once per session)
    // =========================
    public void PlayNarration(AudioClip clip)
    {
        if (!narrationSource || !clip) return;

        // Prevent re-playing the same narration more than once
        if (_playedNarration.Contains(clip)) return;
        _playedNarration.Add(clip);

        // Prepare music ducking
        if (duckMusicDuringNarration && musicSource)
        {
            if (duckRoutine != null) StopCoroutine(duckRoutine);
            duckRoutine = StartCoroutine(DuckMusicWhileNarration());
        }

        narrationSource.Stop();
        narrationSource.clip = clip;
        narrationSource.Play();
    }

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

    // Music-ducking while narration plays
    private IEnumerator DuckMusicWhileNarration()
    {
        if (!musicSource) yield break;

        // Fade down
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
        if (narrationSource)
        {
            while (narrationSource.isPlaying) yield return null;
        }

        // Fade up
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

    // =========================
    // OPTIONAL: Reset locks (e.g., for testing)
    // =========================
    public void ResetPlayOnceLocks(bool resetNarration = true, bool resetNonMenuMusic = true)
    {
        if (resetNarration) _playedNarration.Clear();
        if (resetNonMenuMusic) _playedNonMenuMusic.Clear();
    }
}
