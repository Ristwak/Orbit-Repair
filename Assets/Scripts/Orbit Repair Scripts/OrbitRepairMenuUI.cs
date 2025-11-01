using UnityEngine;
using TMPro;

public class OrbitRepairMenuUI : MonoBehaviour
{
    public static OrbitRepairMenuUI instance;

    [Header("Things to Hide/Show")]
    public GameObject suitUpButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;   // Panel with Start/About/Exit buttons
    public GameObject aboutPanel;      // About info panel
    public GameObject[] gameContent;   // All gameplay content (astronaut, suit parts, etc.)
    public GameObject subscriptionPanel; // Subscription Panel

    [Header("Audio (Optional)")]
    public bool playMenuMusicOnStart = true;

    [Header("Timer (UI only)")]
    public float timeLimit = 180f; // seconds
    public TextMeshProUGUI timeLimitText;

    // Back-compat (not used by timer anymore, but kept if other code reads it)
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
        suitUpButton.SetActive(false);
        // Show main menu, hide about, and ensure the subscription panel is hidden initially
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (aboutPanel)    aboutPanel.SetActive(false);
        if (subscriptionPanel) subscriptionPanel.SetActive(false);

        // Disable game content until Start is pressed
        if (gameContent != null)
            foreach (var content in gameContent)
                if (content) content.SetActive(false);

        // Optionally play menu music
        if (playMenuMusicOnStart && AudioManager.instance != null)
            AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);

        // Initialize the timer display
        globalTimeLimit = timeLimit;
        SetTimerUI(timeLimit);
    }

    // ✅ Start button
    public void OnStartButton()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (aboutPanel)    aboutPanel.SetActive(false);
        if (subscriptionPanel) subscriptionPanel.SetActive(false); // Hide subscription panel if it's active

        if (gameContent != null)
            foreach (var content in gameContent)
                if (content) content.SetActive(true);

        StartMission();
    }

    private void StartMission()
    {
        // Start the shared persistent timer
        if (GameTimer.Instance != null)
            GameTimer.Instance.StartTimer(timeLimit);

        // Welcome VO
        if (AudioManager.instance != null)
            AudioManager.instance.PlayNarration(AudioManager.instance.welcomeSuitupClip);

        suitUpButton.SetActive(true);
    }

    private void HandleTick(float remaining) => SetTimerUI(remaining);

    private void SetTimerUI(float seconds)
    {
        if (!timeLimitText) return;
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        timeLimitText.text = $"{m:00}:{s:00}";
    }

    // ✅ Back Button to return to Main Menu
    public void OnBackButton()
    {
        Debug.Log("Button Clicked");

        // Hide all panels except MainMenu if no subscription is active
        if (subscriptionPanel && subscriptionPanel.activeSelf)
        {
            // Do not deactivate subscription panel if it's active
            return;
        }

        if (aboutPanel)    aboutPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    // ✅ Restart button now loads the "Orbit Repair" scene explicitly
    public void OnRestartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Orbit Repair");
    }

    public void OnAboutButton()
    {
        if (aboutPanel)    aboutPanel.SetActive(true);
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (subscriptionPanel) subscriptionPanel.SetActive(false); // Hide subscription panel if it's visible
    }

    public void OnCloseAbout()
    {
        if (aboutPanel)    aboutPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    // Exit Button Functionality
    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Works in build
    }

    // ✅ Subscription Panel Toggle
    public void OnSubscriptionButton()
    {
        if (subscriptionPanel) subscriptionPanel.SetActive(true);
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (aboutPanel) aboutPanel.SetActive(false);
        if (gameContent != null)
            foreach (var content in gameContent)
                if (content) content.SetActive(false); // Hide gameplay content if subscription is visible
    }

    // Close Subscription Panel (if the user is not subscribing)
    public void OnCloseSubscriptionPanel()
    {
        if (subscriptionPanel) subscriptionPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }
}
