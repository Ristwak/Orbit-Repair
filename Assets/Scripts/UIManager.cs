using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text objectiveText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateObjective(string newObjective)
    {
        if (objectiveText != null)
            objectiveText.text = "Task: " + newObjective;
    }
}
