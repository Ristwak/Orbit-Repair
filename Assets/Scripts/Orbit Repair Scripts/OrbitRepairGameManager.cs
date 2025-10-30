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

    [Tooltip("All objects to hide when the Game Over panel appears (button, world tool, lever/hatch, etc.).")]
    public GameObject[] hideOnGameOver;

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

        // Force play the "game-over" win narration over anything
        ForcePlayGameOverNarration(success: true);

        StartCoroutine(WaitThenShowPanelOrRestart());
        Debug.Log("[OrbitRepairGameManager] Mission Complete!");
    }

    public void MissionFail()
    {
        if (ended) return;
        ended = true;
        timerRunning = false;

        // Force play the "game-over" lose narration over anything
        ForcePlayGameOverNarration(success: false);

        StartCoroutine(WaitThenShowPanelOrRestart());
        Debug.Log("[OrbitRepairGameManager] Mission Failed — Time Over!");
    }

    /// <summary>
    /// Interrupt any audio (music/SFX/narration) and play final game-over narration.
    /// </summary>
    private void ForcePlayGameOverNarration(bool success)
    {
        var am = AudioManager.instance;
        if (am == null) return;

        // Stop everything so game-over VO has priority
        if (am.musicSource && am.musicSource.isPlaying) am.musicSource.Stop();
        if (am.sfxSource && am.sfxSource.isPlaying) am.sfxSource.Stop();
        if (am.narrationSource && am.narrationSource.isPlaying) am.narrationSource.Stop();

        // Play correct final VO
        if (success && am.missionCompleteClip)
            am.PlayNarration(am.missionCompleteClip);
        else if (!success && am.missionFailClip)
            am.PlayNarration(am.missionFailClip);
    }

    private IEnumerator WaitThenShowPanelOrRestart()
    {
        // Wait for the final narration to finish if any
        var am = AudioManager.instance;
        if (am != null && am.narrationSource != null)
        {
            while (am.narrationSource.isPlaying)
                yield return null;
        }

        if (panelDelay > 0f)
            yield return new WaitForSeconds(panelDelay);

        // If there is a GameOver panel in this scene, show it and hide gameplay objects.
        if (gameOverPanel != null && gameOverPanel.scene.IsValid() && gameOverPanel.scene == SceneManager.GetActiveScene())
        {
            // Hide interactive objects
            if (hideOnGameOver != null)
            {
                foreach (var go in hideOnGameOver)
                    if (go) go.SetActive(false);
            }

            gameOverPanel.SetActive(true);
        }
        else
        {
            // No panel available here — restart immediately to the Orbit Repair scene
            RestartMission();
        }
    }

    // === UI Button Hooks (wire these to the panel buttons) ===
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
