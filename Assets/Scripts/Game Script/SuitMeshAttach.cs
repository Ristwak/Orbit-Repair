using UnityEngine;

public class SuitMeshAttach : MonoBehaviour
{
    [Header("References")]
    public string partName;           // e.g. "Helmet", "Bagpack"
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

        meshRenderer = GetComponent<MeshRenderer>();
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
                PlayNarrationForPart(partName); // ✅ play audio line
                currentStep++; // Move to next step

                // If last step completed
                if (currentStep >= suitOrder.Length)
                {
                    AudioManager.instance.PlayNarration(AudioManager.instance.missionCompleteClip);
                    Debug.Log("All suit parts equipped! Mission Complete!");
                }
            }
            else
            {
                Debug.LogWarning($"Can't equip {partName} yet! Next required: {suitOrder[currentStep]}");
            }
        }
    }

    // ✅ Plays the right narrator clip based on the part name
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
}
