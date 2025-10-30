// File: SparkController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SparkController : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Only objects with this tag can fix the box (e.g., your screwdriver).")]
    public string toolTag = "Tool";

    [Header("Effects")]
    public ParticleSystem sparkEffect;      // assign your sparks here
    public AudioSource audioSource;         // optional local source for fallback

    [Header("Timing")]
    [Tooltip("Seconds after the tool touches before the sparks stop.")]
    public float turnOffDelay = 2f;
    [Tooltip("Extra seconds to wait after the win audio finishes, before restart.")]
    public float restartDelay = 2f;

    [Header("Restart")]
    [Tooltip("Scene to load after success (your main menu / start scene).")]
    public string restartSceneName = "Orbit Repair";  // change if different

    private bool fixedOnce = false;

    private void Reset()
    {
        // make the collider a trigger so collisions can fire without physics push
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fixedOnce) return;
        if (!other.CompareTag(toolTag)) return;

        fixedOnce = true;
        StartCoroutine(FixRoutine());
    }

    private IEnumerator FixRoutine()
    {
        // small delay while the tool "works"
        if (turnOffDelay > 0f)
            yield return new WaitForSeconds(turnOffDelay);

        // stop sparks
        if (sparkEffect)
        {
            sparkEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            // optionally hide immediately:
            // var em = sparkEffect.emission; em.enabled = false;
        }

        // play win/mission-complete audio
        float clipLen = 0f;
        bool played = false;

        if (AudioManager.instance && AudioManager.instance.missionCompleteClip)
        {
            AudioManager.instance.PlayNarration(AudioManager.instance.missionCompleteClip);
            clipLen = AudioManager.instance.missionCompleteClip.length;
            played = true;
        }
        else if (audioSource && audioSource.clip)
        {
            audioSource.Stop();
            audioSource.Play();
            clipLen = audioSource.clip.length;
            played = true;
        }

        // wait for audio (if any)
        if (played && clipLen > 0f)
            yield return new WaitForSeconds(clipLen);

        if (restartDelay > 0f)
            yield return new WaitForSeconds(restartDelay);

        // restart / go back
        if (!string.IsNullOrEmpty(restartSceneName))
            SceneManager.LoadScene(restartSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // fallback
    }
}