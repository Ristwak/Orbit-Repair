using UnityEngine;
using TMPro;

/// <summary>
/// Menu controller for Orbit Repair:
/// - Hides menu panels
/// - Enables gameplay content
/// - Starts mission with optional countdown timer
/// - Plays Welcome narration
/// </summary>
public class OrbitRepairMenuUI : MonoBehaviour
{
    public static OrbitRepairMenuUI instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;   // Panel with Start/About/Exit buttons
    public GameObject aboutPanel;      // About info panel
    public GameObject[] gameContent;   // All gameplay content (astronaut, suit parts, etc.)

    [Header("Audio (Optional)")]
    public bool playMenuMusicOnStart = true;

    [Header("Timer (Optional)")]
    public float timeLimit = 180f; // 3 minutes
    public TextMeshProUGUI timeLimitText;
    public static float globalTimeLimit; // Static reference for other scripts

    private static float timeRemaining;
    private static bool isTimerRunning = false;

    private void Awake()
    {
        instance = this;
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
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);
        }

        // Initialize timer display
        globalTimeLimit = timeLimit;
        timeRemaining = timeLimit;
        UpdateTimerUI();
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

        // Start mission
        StartMission();
    }

    // ✅ Mission start logic
    private void StartMission()
    {
        Debug.Log("Mission started!");

        // Start timer
        timeRemaining = timeLimit;
        isTimerRunning = true;
        Debug.Log("Mission started! Time limit: " + timeRemaining + " seconds.");

        // Play welcome narration
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayNarration(AudioManager.instance.welcomeSuitupClip);
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                // Time's up
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerUI();
                Debug.Log("Time’s up!");
            }
        }
    }

    // ✅ Timer formatted as 00:00 (MM:SS)
    private void UpdateTimerUI()
    {
        if (timeLimitText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeLimitText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
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
