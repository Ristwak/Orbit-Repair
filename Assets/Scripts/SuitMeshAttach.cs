using UnityEngine;

public class SuitMeshAttach : MonoBehaviour
{
    [Header("References")]
    public string partName;                     // e.g. "helmet", "bag_pack"
    public MeshFilter astronautMeshFilter;      // MeshFilter on astronaut’s hidden part
    public MeshRenderer astronautMeshRenderer;  // MeshRenderer on astronaut’s hidden part

    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        // Optional: auto-set partName based on GameObject name
        if (string.IsNullOrEmpty(partName))
            partName = gameObject.name;

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false; // Hide the collider object
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}");

        SuitItem item = other.GetComponent<SuitItem>();
        if (item != null && item.partName == partName)
        {
            MeshFilter itemMeshFilter = item.GetComponent<MeshFilter>();
            MeshRenderer itemMeshRenderer = item.GetComponent<MeshRenderer>();

            meshRenderer.enabled = true; // Hide the collider object

            /*
            if (itemMeshFilter != null && astronautMeshFilter != null)
            {
                // ✅ Copy mesh from collided item
                astronautMeshFilter.sharedMesh = itemMeshFilter.sharedMesh;
            }

            if (itemMeshRenderer != null && astronautMeshRenderer != null)
            {
                // ✅ Copy materials from collided item
                astronautMeshRenderer.sharedMaterials = itemMeshRenderer.sharedMaterials;
            }
            */

            // Optionally remove the item after attaching
            Destroy(item.gameObject);

            Debug.Log($"{partName} mesh applied to astronaut!");
        }
    }
}
