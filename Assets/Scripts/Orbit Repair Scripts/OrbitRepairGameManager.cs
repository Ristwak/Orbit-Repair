using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OrbitRepairGameManager : MonoBehaviour
{
    public static OrbitRepairGameManager Instance { get; private set; }

    public enum Phase
    {
        PullLever,
        SuitUp,
        GrabTool,
        ExitToSpace,
        Repair,
        PowerOn,
        Complete
    }

    [Header("Mission Timer")]
    public float missionDuration = 180f; // 3 minutes
    private float timer = 0f;
    private bool timerRunning = false;

    [Header("Restart")]
    [Tooltip("Scene to load after win/lose narration finishes. Leave empty to reload the current active scene.")]
    public string restartSceneName = "Orbit Repair";
    [Tooltip("Extra delay after narration finishes before reloading (seconds).")]
    public float reloadDelay = 0.2f;

    private Phase phase = Phase.PullLever;
    private bool ended = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (timerRunning && !ended)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                MissionFail();
            }
        }
    }

    public void SetPhase(Phase p)
    {
        if (ended) return;
        phase = p;

        switch (p)
        {
            case Phase.SuitUp:
                // Start mission, play music + welcome narration
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlayMusic(AudioManager.instance.gameMusic);
                    AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.WelcomeSuitUp);
                }
                // Start mission timer
                timer = missionDuration;
                timerRunning = true;
                break;

            case Phase.GrabTool:
                if (AudioManager.instance)
                    AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.AfterSuitUp);
                break;

            case Phase.ExitToSpace:
                if (AudioManager.instance)
                    AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.AfterToolPickup);
                break;

            case Phase.Complete:
                // handled by MissionSuccess()
                break;
        }
    }

    public void MissionSuccess()
    {
        if (ended) return;
        ended = true;
        timerRunning = false;
        SetPhase(Phase.Complete);

        if (AudioManager.instance)
            AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.MissionComplete);

        StartCoroutine(WaitForNarrationThenReload());
        Debug.Log("[OrbitRepairGameManager] Mission Complete!");
    }

    public void MissionFail()
    {
        if (ended) return;
        ended = true;
        timerRunning = false;

        if (AudioManager.instance)
            AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.MissionFail);

        StartCoroutine(WaitForNarrationThenReload());
        Debug.Log("[OrbitRepairGameManager] Mission Failed — Time Over!");
    }

    private IEnumerator WaitForNarrationThenReload()
    {
        // If we have a narration source, wait for it to finish
        var am = AudioManager.instance;
        if (am != null && am.narrationSource != null)
        {
            // Wait while a clip is playing
            while (am.narrationSource.isPlaying)
                yield return null;
        }

        if (reloadDelay > 0f)
            yield return new WaitForSeconds(reloadDelay);

        // Reload the target scene (fallback to current scene if name is empty or invalid)
        string sceneToLoad = restartSceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
            sceneToLoad = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneToLoad);
    }
}
