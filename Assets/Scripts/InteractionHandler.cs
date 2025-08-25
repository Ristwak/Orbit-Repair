using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public SuitStep linkedStep;

    private void OnTriggerEnter(Collider other)
    {
        // Placeholder condition: object tagged "PlayerHand" touches correct part
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("Interacted with " + linkedStep.stepName);
            linkedStep.MarkComplete();
        }
    }
}
