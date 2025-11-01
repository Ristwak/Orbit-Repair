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

    [Header("Audio Clips")]
    public AudioClip missionCompleteClip;  // For mission success (win)
    public AudioClip missionFailClip;      // For mission failure (lose)

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
            PlayAudio(missionCompleteClip);
            Debug.Log("[SparkController] Sparks stopped.");
        }

        // Wait for audio to finish
        yield return new WaitForSeconds(audioSource.clip.length);

        // Optional small delay after audio
        if (restartDelay > 0f) yield return new WaitForSeconds(restartDelay);

        // Restart the scene
        var sceneName = string.IsNullOrEmpty(restartSceneName)
            ? SceneManager.GetActiveScene().name
            : restartSceneName;

        Debug.Log("[SparkController] Restarting scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    // Called when game time runs out (or you decide the player loses)
    public void TriggerFail()
    {
        // Check remaining time from GameTimer
        if (GameTimer.Instance != null && GameTimer.Instance.GetRemaining() <= 0)
        {
            // Play Mission Fail audio and restart the game
            var am = AudioManager.instance;
            if (am && missionFailClip)
            {
                am.PlayNarration(missionFailClip); // Play the fail audio

                // Wait for the fail audio to finish before restarting
                StartCoroutine(RestartAfterAudio(missionFailClip.length));
            }
        }
    }

    // Helper to wait for the mission fail audio to finish before restarting the game
    private IEnumerator RestartAfterAudio(float audioDuration)
    {
        yield return new WaitForSeconds(audioDuration + restartDelay);
        SceneManager.LoadScene(restartSceneName); // Restart the game after the audio is played
    }
}
