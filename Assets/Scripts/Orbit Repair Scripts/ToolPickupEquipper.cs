// File: ToolPickupEquipper.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolPickupEquipper : MonoBehaviour
{
    // ---- Global lock/unlock (default: locked) ----
    private static bool pickupsUnlocked = false;
    public static void UnlockAllPickups() { pickupsUnlocked = true; }
    public static void LockAllPickups()  { pickupsUnlocked = false; }

    [Header("Who can trigger pickup")]
    public string requiredTag = "Player";

    [Header("Equip on pickup")]
    [Tooltip("In-hand tool (disabled at start) parented to player's hand.")]
    public GameObject equippedTool;
    public bool destroyWorldTool = true;

    [Header("What should appear after pickup")]
    [Tooltip("Lever root to enable only after tool pickup. Keep INACTIVE at start.")]
    public GameObject leverRootToEnable;
    public LeverToSceneLoader lever;

    //[Header("Optional SFX")]
    //public AudioSource sfxSource;
    //public AudioClip pickupSfx;

    [Header("XR (optional)")]
    public XRSimpleInteractable xrInteractable;

    [Header("Mouse Testing")]
    public bool enableMouseTest = true;            // allow desktop testing
    public float mouseRayDistance = 6f;            // how far the ray can click
    public LayerMask mouseRayMask = ~0;            // layers to hit (default: everything)
    public Camera testCamera;                      // if null, uses Camera.main

    private Collider myCollider;
    private bool done = false;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!xrInteractable) xrInteractable = GetComponent<XRSimpleInteractable>();
        myCollider = GetComponent<Collider>();

        if (equippedTool && equippedTool.activeSelf)
            equippedTool.SetActive(false); // keep hidden until suit-up unlocks
    }

    void OnEnable()
    {
        if (xrInteractable)
            xrInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        if (xrInteractable)
            xrInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs _) => TryEquip();

    private void OnTriggerEnter(Collider other)
    {
        if (done) return;
        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag) || other.CompareTag("Hand"))
            TryEquip();
    }

    void Update()
    {
        if (!enableMouseTest || done) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = testCamera ? testCamera : Camera.main;
        if (!cam) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, mouseRayDistance, mouseRayMask, QueryTriggerInteraction.Collide))
        {
            // Accept the click if it hits this object or any of its children
            if (hit.collider &&
                (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform) ||
                 (myCollider && hit.collider == myCollider)))
            {
                TryEquip();
            }
        }
    }

    private void TryEquip()
    {
        if (done) return;
        if (!pickupsUnlocked) return;

        done = true;

        // --- Force show the in-hand tool and diagnose common issues ---
        if (equippedTool)
        {
            Transform t = equippedTool.transform;
            bool hadInactiveParent = false;
            while (t != null)
            {
                if (!t.gameObject.activeSelf) { hadInactiveParent = true; t.gameObject.SetActive(true); }
                t = t.parent;
            }
            if (hadInactiveParent)
                Debug.LogWarning("[ToolPickupEquipper] One or more parents were inactive. Activated entire chain.");

            equippedTool.SetActive(true);

            foreach (var r in equippedTool.GetComponentsInChildren<Renderer>(true)) r.enabled = true;
            foreach (var c in equippedTool.GetComponentsInChildren<Collider>(true)) c.enabled = true;

            Debug.Log($"[ToolPickupEquipper] Equipped tool now activeInHierarchy={equippedTool.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("[ToolPickupEquipper] 'equippedTool' is NOT assigned in the Inspector.");
        }

        // Reveal/enable the lever
        if (leverRootToEnable)
        {
            if (!leverRootToEnable.activeSelf) leverRootToEnable.SetActive(true);
            if (lever) lever.UnlockLever();
            var leverLoader = leverRootToEnable.GetComponent<LeverToSceneLoader>();
            if (leverLoader && !leverLoader.enabled) leverLoader.enabled = true;
        }

        OrbitRepairSequenceDirector.Instance?.NotifyToolPicked();
        OrbitRepairGameManager.Instance?.SetPhase(OrbitRepairGameManager.Phase.ExitToSpace);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.AfterToolPickup);

        //if (sfxSource && pickupSfx) sfxSource.PlayOneShot(pickupSfx);

        if (destroyWorldTool) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
