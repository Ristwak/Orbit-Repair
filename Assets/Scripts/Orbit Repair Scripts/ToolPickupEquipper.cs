using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolPickupEquipper : MonoBehaviour
{
    // ---- Global lock/unlock (default: locked) ----
    private static bool pickupsUnlocked = false;
    public static void UnlockAllPickups() { pickupsUnlocked = true; }
    public static void LockAllPickups() { pickupsUnlocked = false; }

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

    // [Header("Optional SFX")]
    // public AudioSource sfxSource;
    // public AudioClip pickupSfx;

    [Header("XR (optional)")]
    public XRSimpleInteractable xrInteractable;

    private bool done = false;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!xrInteractable) xrInteractable = GetComponent<XRSimpleInteractable>();
        if (equippedTool && equippedTool.activeSelf)
            equippedTool.SetActive(false); // hidden until suit-up allows pickup
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

    private void TryEquip()
    {
        if (done) return;
        if (!pickupsUnlocked) return;

        done = true;

        // --- Force show the in-hand tool and diagnose common issues ---
        if (equippedTool)
        {
            // 1) make sure its whole parent chain is active
            Transform t = equippedTool.transform;
            bool hadInactiveParent = false;
            while (t != null)
            {
                if (!t.gameObject.activeSelf) { hadInactiveParent = true; t.gameObject.SetActive(true); }
                t = t.parent;
            }
            if (hadInactiveParent) Debug.LogWarning("[ToolPickupEquipper] One or more parents were inactive. Activated entire chain.");

            // 2) activate the object itself
            equippedTool.SetActive(true);

            // 3) enable all renderers & colliders in case they were disabled
            foreach (var r in equippedTool.GetComponentsInChildren<Renderer>(true))
                r.enabled = true;
            foreach (var c in equippedTool.GetComponentsInChildren<Collider>(true))
                c.enabled = true;

            // 4) sanity log
            Debug.Log($"[ToolPickupEquipper] Equipped tool now activeInHierarchy={equippedTool.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("[ToolPickupEquipper] 'equippedTool' is NOT assigned in the Inspector.");
        }

        // ✅ reveal/enable the lever
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

        if (destroyWorldTool) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
