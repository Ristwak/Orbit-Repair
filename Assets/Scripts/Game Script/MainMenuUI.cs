using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;   // Panel with Start, About, Exit buttons
    public GameObject aboutPanel;      // About info panel
    public GameObject[] gameContent;     // All gameplay content (astronaut, suit parts, etc.)
    public GameObject background;      // Background environment/animations

    private void Start()
    {
        // On launch: show main menu, hide everything else
        mainMenuPanel.SetActive(true);
        aboutPanel.SetActive(false);
        // gameContent.SetActive(false);
        foreach (var content in gameContent) 
            if (content != null) 
                content.SetActive(false);

        if (background != null)
            background.SetActive(true); // Background always runs
    }

    // Called when "Start" button is pressed
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
    }

    // Called when "About" button is pressed
    public void OnAboutButton()
    {
        aboutPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        // Background stays active, gameContent stays inactive
    }

    // Called when closing About panel (Back button inside About)
    public void OnCloseAbout()
    {
        aboutPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Called when "Exit" button is pressed
    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Works in build, not in editor
    }
}
