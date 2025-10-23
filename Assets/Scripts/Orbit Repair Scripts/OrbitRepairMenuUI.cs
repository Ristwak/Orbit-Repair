using UnityEngine;
// using TMPro; // Uncomment if you enable the optional timer UI

/// <summary>
/// Menu controller for Orbit Repair:
/// - Hides menu panels
/// - Enables gameplay content
/// - Starts mission (internal StartMission)
/// - Plays Welcome narration
/// </summary>
public class OrbitRepairMenuUI : MonoBehaviour
{
    public static OrbitRepairMenuUI instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;   // Panel with Start/About/Exit buttons
    public GameObject aboutPanel;      // About info panel
    public GameObject[] gameContent;   // All gameplay content (astronaut, suit parts, etc.)
    public GameObject background;      // Background environment/animations (optional)

    [Header("Audio (Optional)")]
    public bool playMenuMusicOnStart = true;

    // ----------------------------------------------------------------
    // Optional Timer (commented out by default). Uncomment to expose a
    // global time limit from the menu.
    // ----------------------------------------------------------------
    /*
    [Header("Optional Timer (Commented Out)")]
    public float timeLimit = 180f; // 3 minutes
    public TextMeshProUGUI timeLimitText;
    public static float globalTimeLimit; // Static reference for other scripts

    private static float timeRemaining;
    private static bool isTimerRunning = false;
    */

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

        if (background) background.SetActive(true);

        // Optional: play menu music
        if (playMenuMusicOnStart && AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);
        }

        // Optional timer init (commented out)
        /*
        globalTimeLimit = timeLimit;
        if (timeLimitText != null)
            timeLimitText.text = Mathf.CeilToInt(timeLimit).ToString();
        */
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

    // ✅ New: Internal StartMission copied & simplified from SuitMeshAttach
    private void StartMission()
    {
        Debug.Log("Mission started!");

        // Optional timer logic (commented)
        /*
        timeRemaining = timeLimit;
        isTimerRunning = true;
        Debug.Log("Mission started! Time limit: " + timeRemaining + " seconds.");
        */

        // Play welcome narration
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayNarration(AudioManager.instance.welcomeSuitupClip);
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
