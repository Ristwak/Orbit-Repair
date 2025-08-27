using UnityEngine;

public class SuitMeshAttach : MonoBehaviour
{
    [Header("References")]
    public string partName;           // e.g. "helmet", "bag_pack"
    private MeshRenderer meshRenderer; // MeshRenderer on astronaut body slot

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

    private void Awake()
    {
        if (string.IsNullOrEmpty(partName))
            partName = gameObject.name;

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false; // Hide at start
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}");

        SuitItem item = other.GetComponent<SuitItem>();
        if (item != null && item.partName == partName)
        {
            // ✅ Check if this is the correct step in the sequence
            if (suitOrder[currentStep] == partName)
            {
                if (meshRenderer != null)
                    meshRenderer.enabled = true;

                Destroy(item.gameObject);

                Debug.Log($"{partName} equipped!");
                currentStep++; // Move to next step
            }
            else
            {
                Debug.LogWarning($"Can't equip {partName} yet! Next required: {suitOrder[currentStep]}");
            }
        }
    }
}
