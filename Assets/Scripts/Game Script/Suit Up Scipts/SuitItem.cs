using UnityEngine;

public class SuitItem : MonoBehaviour
{
    public string partName;   // must match SuitMeshAttach partName

    private void Awake()
    {
        partName = gameObject.name;
    }
}
