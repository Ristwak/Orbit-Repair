// File: OrbitRepairMenuUI.cs
using UnityEngine;
using TMPro;

public class OrbitRepairMenuUI : MonoBehaviour
{
    public static OrbitRepairMenuUI instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;   // Panel with Start/About/Exit buttons
    public GameObject aboutPanel;      // About info panel
    public GameObject[] gameContent;   // All gameplay content (astronaut, suit parts, etc.)

    [Header("Audio (Optional)")]
    public bool playMenuMusicOnStart = true;

    [Header("Timer (UI only)")]
    public float timeLimit = 180f; // seconds
    public TextMeshProUGUI timeLimitText;

    // For backward-compat with any old code referencing this:
    public static float globalTimeLimit;

    private void Awake() => instance = this;

    private void OnEnable()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTick += HandleTick;
    }

    private void OnDisable()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTick -= HandleTick;
    }

    private void Start()
    {
        // Show main menu, hide about
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (aboutPanel) aboutPanel.SetActive(false);

        // Disable game content until Start is pressed
        if (gameContent != null)
        {
            foreach (var content in gameContent)
                if (content != null) content.SetActive(false);
        }

        // Optional: play menu music
        if (playMenuMusicOnStart && AudioManager.instance != null)
            AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);

        // Init timer display
        globalTimeLimit = timeLimit;
        SetTimerUI(timeLimit);
    }

    // ✅ Start button
    public void OnStartButton()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (aboutPanel) aboutPanel.SetActive(false);

        if (gameContent != null)
        {
            foreach (var content in gameContent)
                if (content != null) content.SetActive(true);
        }

        StartMission();
    }

    private void StartMission()
    {
        Debug.Log("Mission started!");

        // Start the shared timer (countdown)
        if (GameTimer.Instance != null)
            GameTimer.Instance.StartTimer(timeLimit);

        // Play welcome narration
        if (AudioManager.instance != null)
            AudioManager.instance.PlayNarration(AudioManager.instance.welcomeSuitupClip);
    }

    private void HandleTick(float remaining)
    {
        SetTimerUI(remaining);
    }

    // ✅ Timer formatted as 00:00 (MM:SS)
    private void SetTimerUI(float seconds)
    {
        if (timeLimitText == null) return;
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs    = Mathf.FloorToInt(seconds % 60f);
        timeLimitText.text = $"{minutes:00}:{secs:00}";
    }

    public void OnAboutButton()
    {
        if (aboutPanel) aboutPanel.SetActive(true);
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
    }

    public void OnCloseAbout()
    {
        if (aboutPanel) aboutPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Works in build, not in editor
    }
}
