using UnityEngine;

public class SuitStep : MonoBehaviour
{
    public string stepName;
    private bool isComplete = false;

    public void EnableStep(bool enable)
    {
        // Placeholder: enable object highlight or make it interactable
        gameObject.SetActive(enable);
    }

    public void MarkComplete()
    {
        if (!isComplete)
        {
            isComplete = true;
            Debug.Log(stepName + " completed.");
            GameManager manager = FindObjectOfType<GameManager>();
            manager.CompleteStep();
        }
    }
}
