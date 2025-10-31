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
        // Singleton w/ persistence across scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Hard-enforce non-looping for narration/SFX sources
        if (narrationSource) narrationSource.loop = false;
        if (sfxSource)       sfxSource.loop       = false;

        // If musicSource exists but some inspector had loop=true, we'll override in PlayMusic per-clip
    }

    private void Start()
    {
        // Menu music is the only one allowed to loop / restart
        PlayMusic(menuMusic);
    }

    // =========================
    // MUSIC (play-once per clip, EXCEPT menuMusic which loops)
    // =========================
    public void PlayMusic(AudioClip clip)
    {
        if (!musicSource || !clip) return;

        bool isMenu = (clip == menuMusic);

        // prevent replay of the same non-menu track within the session
        if (!isMenu)
        {
            if (_playedNonMenuMusic.Contains(clip)) return;
            _playedNonMenuMusic.Add(clip);
        }

        // If the requested clip is already playing, do nothing (prevents restart “loop”)
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = musicVolume;

        // ONLY menu music loops
        musicSource.loop = isMenu;

        musicSource.Play();
    }

    // =========================
    // SFX (fire-and-forget)
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

        // stop repeats: narration plays once per session
        if (_playedNarration.Contains(clip)) return;
        _playedNarration.Add(clip);

        // prepare ducking
        if (duckMusicDuringNarration && musicSource)
        {
            if (duckRoutine != null) StopCoroutine(duckRoutine);
            duckRoutine = StartCoroutine(DuckMusicWhileNarration());
        }

        // play narration once, no loop
        narrationSource.loop = false;
        narrationSource.Stop();
        narrationSource.clip = clip;
        narrationSource.Play();
    }

    // Force version (e.g., final Game Over VO must override anything)
    public void PlayNarrationForce(AudioClip clip)
    {
        if (!narrationSource || !clip) return;

        // Stop everything underneath so this takes priority
        if (musicSource && musicSource.isPlaying) musicSource.Stop();
        if (sfxSource   && sfxSource.isPlaying)   sfxSource.Stop();
        if (narrationSource.isPlaying)            narrationSource.Stop();

        // We STILL respect “play once per session” to avoid repeated GOs if called twice
        if (!_playedNarration.Contains(clip)) _playedNarration.Add(clip);

        narrationSource.loop = false;

        if (duckMusicDuringNarration && musicSource)
        {
            if (duckRoutine != null) StopCoroutine(duckRoutine);
            duckRoutine = StartCoroutine(DuckMusicWhileNarration());
        }

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

    // Optional: reset locks for testing (e.g., in editor)
    public void ResetPlayOnceLocks(bool resetNarration = true, bool resetNonMenuMusic = true)
    {
        if (resetNarration)   _playedNarration.Clear();
        if (resetNonMenuMusic) _playedNonMenuMusic.Clear();
    }
}
