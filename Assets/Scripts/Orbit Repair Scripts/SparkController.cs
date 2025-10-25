using UnityEngine;
using UnityEngine.SceneManagement;

public class SparkController : MonoBehaviour
{
    public ParticleSystem sparkEffect;     // Assign your SparkFX here
    public AudioSource audioSource;        // Assign an AudioSource
    public AudioClip gameWinClip;          // Assign your “game win” sound
    public float turnOffDelay = 2f;        // Seconds before spark stops
    public float restartDelay = 5f;        // Seconds before restart

    private bool repaired = false;

    private void Start()
    {
        // Make sure the spark is running initially
        if (sparkEffect != null && !sparkEffect.isPlaying)
            sparkEffect.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (repaired) return; // Prevent double triggers

        if (collision.gameObject.CompareTag("Tool"))
        {
            repaired = true;
            StartCoroutine(RepairSequence());
        }
    }

    private System.Collections.IEnumerator RepairSequence()
    {
        // Wait before turning spark off
        yield return new WaitForSeconds(turnOffDelay);

        if (sparkEffect != null)
            sparkEffect.Stop();

        // Play win sound
        if (audioSource != null && gameWinClip != null)
            audioSource.PlayOneShot(gameWinClip);

        // Wait, then restart the scene
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
