using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetIfMisplaced : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private bool isBeingHeld = false;
    private bool isEquipped = false; // ✅ Added: track if this part was successfully equipped

    void Start()
    {
        // Save original spawn position & rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isBeingHeld = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isBeingHeld = false;

        // ✅ Only reset if not already equipped to astronaut
        if (!isEquipped)
        {
            ResetToOriginalPosition();
        }
    }

    public void ResetToOriginalPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    // ✅ Call this from SuitMeshAttach when the part is correctly used
    public void MarkAsEquipped()
    {
        isEquipped = true;
        // Disable grab so it can’t be moved again
        if (grabInteractable != null)
            grabInteractable.enabled = false;
        if (rb != null)
            rb.isKinematic = true;
    }
}
