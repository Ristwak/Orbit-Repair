// File: OrbitRepairGameManager.cs
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

    [Header("Mission Timer (optional)")]
    // public float missionDuration = 180f; // 3 minutes
    private float timer = 0f;
    private bool timerRunning = false;

    [Header("UI")]
    [Tooltip("Game Over / Mission Complete panel shown after narration ends.")]
    public GameObject gameOverPanel;
    [Tooltip("Delay (sec) after narration finishes before showing the panel.")]
    public float panelDelay = 0.2f;

    [Header("Restart (for the UI button)")]
    [Tooltip("If empty, RestartMission() reloads the current active scene.")]
    public string restartSceneName = "Orbit Repair";

    private Phase phase = Phase.PullLever;
    private bool ended = false;

    void Awake()
    {
        Instance = this;
        if (gameOverPanel) gameOverPanel.SetActive(false);
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
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlayMusic(AudioManager.instance.gameMusic);
                    AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.WelcomeSuitUp);
                }
                // Optional timer start
                timer = OrbitRepairMenuUI.globalTimeLimit; // set by menu
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

        StartCoroutine(WaitForNarrationThenShowPanel());
        Debug.Log("[OrbitRepairGameManager] Mission Complete!");
    }

    public void MissionFail()
    {
        if (ended) return;
        ended = true;
        timerRunning = false;

        if (AudioManager.instance)
            AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.MissionFail);

        StartCoroutine(WaitForNarrationThenShowPanel());
        Debug.Log("[OrbitRepairGameManager] Mission Failed — Time Over!");
    }

    private IEnumerator WaitForNarrationThenShowPanel()
    {
        // wait for narration (if any)
        var am = AudioManager.instance;
        if (am != null && am.narrationSource != null)
        {
            while (am.narrationSource.isPlaying)
                yield return null;
        }

        if (panelDelay > 0f)
            yield return new WaitForSeconds(panelDelay);

        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    // === UI Button Hooks ===
    public void RestartMission()
    {
        string sceneToLoad = string.IsNullOrEmpty(restartSceneName)
            ? SceneManager.GetActiveScene().name
            : restartSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitMission()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
