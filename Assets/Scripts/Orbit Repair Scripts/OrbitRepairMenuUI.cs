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
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (aboutPanel)    aboutPanel.SetActive(false);

        if (gameContent != null)
            foreach (var content in gameContent)
                if (content) content.SetActive(false);

        if (playMenuMusicOnStart && AudioManager.instance != null)
            AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);

        globalTimeLimit = timeLimit;
        SetTimerUI(timeLimit);
    }

    // ✅ Start button
    public void OnStartButton()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (aboutPanel)    aboutPanel.SetActive(false);

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
    }

    private void HandleTick(float remaining) => SetTimerUI(remaining);

    private void SetTimerUI(float seconds)
    {
        if (!timeLimitText) return;
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        timeLimitText.text = $"{m:00}:{s:00}";
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
    }

    public void OnCloseAbout()
    {
        if (aboutPanel)    aboutPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Works in build
    }
}