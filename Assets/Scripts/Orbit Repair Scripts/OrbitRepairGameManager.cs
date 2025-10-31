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
    // Timer is driven by GameTimer; we don’t count here anymore.
    private bool ended = false;

    [Header("UI")]
    [Tooltip("Game Over / Mission Complete panel shown after narration ends.")]
    public GameObject gameOverPanel;
    [Tooltip("Delay (sec) after narration finishes before showing the panel.")]
    public float panelDelay = 0.2f;

    [Header("Hide When Mission Ends (timeout or success)")]
    [Tooltip("Assign objects that must disappear the moment the mission ends (timeout/success). " +
             "E.g., SuitUp button, world/tool on table, hatch/lever root, spark root, etc.")]
    public GameObject[] hideOnEnd;

    [Header("Restart (for the UI button)")]
    [Tooltip("If empty, RestartMission() reloads the current active scene.")]
    public string restartSceneName = "Orbit Repair";

    private Phase phase = Phase.PullLever;

    private void Awake()
    {
        Instance = this;
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Subscribe to GameTimer timeout (safe even if Instance isn’t created yet; we try again at Start).
        TryHookTimer();
    }

    private void Start()
    {
        // In case GameTimer spawned after us.
        TryHookTimer();
    }

    private void OnDestroy()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTimeUp -= HandleTimeUp;
    }

    private void TryHookTimer()
    {
        if (GameTimer.Instance != null)
        {
            // Avoid double-subscribe
            GameTimer.Instance.OnTimeUp -= HandleTimeUp;
            GameTimer.Instance.OnTimeUp += HandleTimeUp;
        }
    }

    private void HandleTimeUp()
    {
        // Called by GameTimer when countdown hits zero
        MissionFail();
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
        SetPhase(Phase.Complete);

        // Immediately hide gameplay interactables
        HideGameplayObjects();

        // Force game-over narration on top
        ForcePlayGameOverNarration(success: true);

        StartCoroutine(WaitThenShowPanelOrRestart());
        Debug.Log("[OrbitRepairGameManager] Mission Complete!");
    }

    public void MissionFail()
    {
        if (ended) return;
        ended = true;

        // Immediately hide gameplay interactables
        HideGameplayObjects();

        // Force game-over narration on top
        ForcePlayGameOverNarration(success: false);

        StartCoroutine(WaitThenShowPanelOrRestart());
        Debug.Log("[OrbitRepairGameManager] Mission Failed — Time Over!");
    }

    private void HideGameplayObjects()
    {
        if (hideOnEnd == null) return;

        foreach (var go in hideOnEnd)
        {
            if (!go) continue;

            // Stop any sparks if this object (or a child) has a ParticleSystem
            foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            go.SetActive(false);
        }
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

        // If there is a GameOver panel in this scene, show it; else restart to Orbit Repair scene.
        if (gameOverPanel != null && gameOverPanel.scene.IsValid() &&
            gameOverPanel.scene == SceneManager.GetActiveScene())
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
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
