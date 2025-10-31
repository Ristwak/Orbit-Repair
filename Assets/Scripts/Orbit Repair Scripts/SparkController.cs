// File: SparkController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SparkController : MonoBehaviour
{
    [Header("Detection")]
    public string toolTag = "Tool";

    [Header("Effects")]
    public ParticleSystem sparkEffect;
    public AudioSource audioSource; // fallback local source (set ignoreListenerPause in Awake)

    [Header("Timing")]
    public float turnOffDelay = 2f;
    public float restartDelay = 2f;
    [Tooltip("Hard timeout (seconds) if we never detect audio has finished.")]
    public float narrationWaitTimeout = 20f;

    [Header("Restart")]
    public string restartSceneName = "Orbit Repair";

    private bool fixedOnce = false;

    void Awake()
    {
        // Make sure our fallback source can play even if someone pauses AudioListener.
        if (audioSource) audioSource.ignoreListenerPause = true;
    }

    void Start()
    {
        Debug.Log("[SparkController] Active.");
    }

    void Reset()
    {
        // Ensure trigger + a kinematic RB so Unity will send trigger events
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (fixedOnce) return;
        if (!other.CompareTag(toolTag)) return;

        Debug.Log("[SparkController] Tool collision: " + other.name);
        fixedOnce = true;
        StartCoroutine(FixRoutine());
    }

    IEnumerator FixRoutine()
    {
        if (turnOffDelay > 0f) yield return new WaitForSeconds(turnOffDelay);

        // Stop sparks
        if (sparkEffect)
        {
            sparkEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Debug.Log("[SparkController] Sparks stopped.");
        }

        // ---- Play Mission Complete VO ----
        float waited = 0f;
        bool played = false;

        var am = AudioManager.instance;
        if (am && am.missionCompleteClip)
        {
            // Give the VO priority
            if (am.musicSource && am.musicSource.isPlaying) am.musicSource.Stop();
            if (am.sfxSource && am.sfxSource.isPlaying) am.sfxSource.Stop();
            if (am.narrationSource)
            {
                am.narrationSource.ignoreListenerPause = true; // ensure it plays even if the game gets paused elsewhere
                if (am.narrationSource.isPlaying) am.narrationSource.Stop();
            }

            am.PlayNarration(am.missionCompleteClip);
            played = true;

            // Wait while narration actually plays (with timeout)
            if (am.narrationSource)
            {
                while (am.narrationSource.isPlaying && waited < narrationWaitTimeout)
                {
                    waited += Time.unscaledDeltaTime; // don’t be affected by timeScale
                    yield return null;
                }
            }
            else
            {
                // If there’s no narrationSource, fall back to clip length
                yield return new WaitForSeconds(am.missionCompleteClip.length);
            }
        }
        else if (audioSource && audioSource.clip)
        {
            audioSource.Stop();
            audioSource.Play();
            played = true;

            // Wait for local clip
            waited = 0f;
            while (audioSource.isPlaying && waited < narrationWaitTimeout)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // Optional small delay after audio
        if (restartDelay > 0f) yield return new WaitForSeconds(restartDelay);

        // Restart
        var sceneName = string.IsNullOrEmpty(restartSceneName)
            ? SceneManager.GetActiveScene().name
            : restartSceneName;

        Debug.Log("[SparkController] Restarting scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
