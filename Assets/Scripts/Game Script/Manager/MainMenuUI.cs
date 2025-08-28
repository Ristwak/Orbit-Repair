using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI instance;
    [Header("Panels")]
    public GameObject mainMenuPanel;   // Panel with Start, About, Exit buttons
    public GameObject aboutPanel;      // About info panel
    public GameObject gameOverPanel;
    public GameObject[] gameContent;   // All gameplay content (astronaut, suit parts, etc.)
    public GameObject background;      // Background environment/animations

    [Header("Game Settings")]
    public float timeLimit = 240f; // Default 4 minutes (adjustable in Inspector)
    public TextMeshProUGUI timeLimitText;
    public static float globalTimeLimit; // Static reference for other scripts

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Sync global time with inspector value
        globalTimeLimit = timeLimit;

        // On launch: show main menu, hide everything else
        mainMenuPanel.SetActive(true);
        aboutPanel.SetActive(false);

        foreach (var content in gameContent)
            if (content != null)
                content.SetActive(false);

        if (background != null)
            background.SetActive(true); // Background always runs

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Hide at start
        
    }

    public void OnStartButton()
    {
        mainMenuPanel.SetActive(false);
        aboutPanel.SetActive(false);

        if (gameContent != null)
        {
            foreach (var content in gameContent)
                if (content != null)
                    content.SetActive(true);
        }

        if (AudioManager.instance != null)
            AudioManager.instance.PlayNarration(AudioManager.instance.introClip);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayMusic(AudioManager.instance.gameMusic);

        // ✅ Start the mission timer
        SuitMeshAttach.StartMission();
    }

    public void OnAboutButton()
    {
        aboutPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void OnCloseAbout()
    {
        aboutPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Works in build, not in editor
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // ✅ Restart function for "Restart" button on GameOver panel
    public void OnRestartButton()
    {
        // Reloads the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
