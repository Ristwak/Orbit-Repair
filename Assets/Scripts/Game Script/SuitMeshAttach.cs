using UnityEngine;
using UnityEngine.UI; // For timer UI if needed

public class SuitMeshAttach : MonoBehaviour
{
    [Header("References")]
    public string partName;            // e.g. "Helmet", "Bagpack"
    private MeshRenderer meshRenderer; // MeshRenderer on astronaut body slot

    [Header("Timer Settings")]
    private static float timeRemaining;
    private static bool isTimerRunning = false;


    [Header("UI References")]

    // Define the correct order
    private static readonly string[] suitOrder =
    {
        "Skin Cooling suit",
        "Main Suit",
        "Shoes",
        "left Hand Gloves",
        "Right Hand Gloves",
        "Bagpack",
        "Helmet"
    };

    private static int currentStep = 0; // Tracks what part is next
    private static bool missionComplete = false;

    private void Awake()
    {
        if (string.IsNullOrEmpty(partName))
            partName = gameObject.name;

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false; // Hide at start

        // Start timer only once
        if (!isTimerRunning)
        {
            timeRemaining = MainMenuUI.globalTimeLimit;
            isTimerRunning = true;
        }
    }

    private void Update()
    {
        if (isTimerRunning && !missionComplete)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                OnTimeUp();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (missionComplete || !isTimerRunning) return;

        Debug.Log($"Trigger entered by: {other.gameObject.name}");

        SuitItem item = other.GetComponent<SuitItem>();
        if (item != null && item.partName == partName)
        {
            if (suitOrder[currentStep] == partName)
            {
                if (meshRenderer != null)
                    meshRenderer.enabled = true;

                Destroy(item.gameObject);

                Debug.Log($"{partName} equipped!");
                PlayNarrationForPart(partName);
                currentStep++;

                // If last step completed
                if (currentStep >= suitOrder.Length)
                {
                    missionComplete = true;
                    isTimerRunning = false;

                    // ✅ Play mission complete narration
                    AudioManager.instance.PlayNarration(AudioManager.instance.missionCompleteClip);

                    // ✅ Open GameOver panel
                    MainMenuUI.instance.ShowGameOverPanel();

                    Debug.Log("All suit parts equipped! Mission Complete!");
                }
            }
            else
            {
                Debug.LogWarning($"Can't equip {partName} yet! Next required: {suitOrder[currentStep]}");
            }
        }
    }

    private void PlayNarrationForPart(string part)
    {
        switch (part)
        {
            case "Skin Cooling suit":
                AudioManager.instance.PlayNarration(AudioManager.instance.skinCoolingClip);
                break;
            case "Main Suit":
                AudioManager.instance.PlayNarration(AudioManager.instance.mainSuitClip);
                break;
            case "Shoes":
                AudioManager.instance.PlayNarration(AudioManager.instance.shoesClip);
                break;
            case "left Hand Gloves":
                AudioManager.instance.PlayNarration(AudioManager.instance.leftGloveClip);
                break;
            case "Right Hand Gloves":
                AudioManager.instance.PlayNarration(AudioManager.instance.rightGloveClip);
                break;
            case "Bagpack":
                AudioManager.instance.PlayNarration(AudioManager.instance.backpackClip);
                break;
            case "Helmet":
                AudioManager.instance.PlayNarration(AudioManager.instance.helmetClip);
                break;
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (MainMenuUI.instance.timeLimitText == null) return;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        MainMenuUI.instance.timeLimitText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimeUp()
    {
        Debug.Log("Time is up! Mission failed.");
        if (MainMenuUI.instance.timeLimitText != null)
            MainMenuUI.instance.timeLimitText.text = "00:00";

        // Optional: play fail narration or load fail screen
        // AudioManager.instance.PlayNarration(AudioManager.instance.failClip);
    }
}
