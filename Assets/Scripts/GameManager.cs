using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SuitStep[] steps;
    private int currentStepIndex = 0;

    void Start()
    {
        ShowCurrentStep();
    }

    public void CompleteStep()
    {
        steps[currentStepIndex].MarkComplete();
        currentStepIndex++;

        if (currentStepIndex < steps.Length)
            ShowCurrentStep();
        else
            Debug.Log("All steps completed! 🎉 Ready for mission simulation!");
    }

    private void ShowCurrentStep()
    {
        Debug.Log("Current Step: " + steps[currentStepIndex].stepName);
        UIManager.Instance.UpdateObjective(steps[currentStepIndex].stepName);
        steps[currentStepIndex].EnableStep(true);
    }
}
